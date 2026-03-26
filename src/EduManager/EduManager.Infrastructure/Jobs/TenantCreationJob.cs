using EduManager.Application.Interfaces;
using EduManager.Domain.Entities.Master;
using EduManager.Domain.Enums;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Diagnostics;
using System.Security.Cryptography;

namespace EduManager.Infrastructure.Jobs;

public class TenantCreationJob(
    ITenantRepository repository
    , IMasterUnitOfWork unitOfWork
    , IConfiguration configuration
    , IEncryptionService encryption
    , ITenantMigrationService migrationService
    , ILogger<TenantCreationJob> logger)
    : ITenantCreationJob
{
    private readonly ITenantRepository _repository = repository;
    private readonly IMasterUnitOfWork _unitOfWork = unitOfWork;
    private IConfiguration _configuration = configuration;
    private readonly IEncryptionService _encryption = encryption;
    private readonly ITenantMigrationService _migrationService = migrationService;
    private readonly ILogger<TenantCreationJob> _logger = logger;

    private const int MaxAttempts = 3;
    private const int RetryDelayMs = 2000;

    public async Task ExecutionAsync(long tenantId, string password)
    {
        var tenant = await _repository.GetByIdAsync(tenantId);
        if (tenant is null) return;

        var dbName = $"Edumanager_{tenant.Slug}";
        var dbCreated = false;
        var currentStep = "CreateDatabase";

        try
        {
            await RetryAsync(() => CreateDatabaseAsync(tenant.Slug), currentStep);
            dbCreated = true;

            var superAdminConn = BuildSuperAdminConnectionString(tenant.Slug);

            currentStep = "Migration";
            await RetryAsync(() => _migrationService.MigrateOnlyAsync(superAdminConn), currentStep);

            currentStep = "InsertTenantConfig";
            await RetryAsync(() => InsertTenantConfigAsync(superAdminConn, tenant.SlugCode), currentStep);

            currentStep = "Seed";
            await RetryAsync(() => _migrationService.SeedOnlyAsync(superAdminConn, tenant.Email, password), currentStep);

            currentStep = "CreateDatabaseUser";
            var userResult = await RetryAsync(() => CreateDatabaseUserAsync(tenant.Slug, superAdminConn), currentStep);

            var plainConnectionString = BuildIsolatedConnectionString(tenant.Slug, userResult.UserName, userResult.Password);
            tenant.ConnectionString = _encryption.Encrypt(plainConnectionString, tenant.Slug, tenant.EncryptionSalt);
            tenant.Status = TenantStatus.Active;

            _repository.Update(tenant);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation(
                "TenantCreationJob: Tenant {TenantId} provisioned successfully. DB: {DbName}",
                tenantId, dbName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "TenantCreationJob: Step '{Step}' failed for tenant {TenantId} after {MaxAttempts} attempts. Aborting.",
                currentStep, tenantId, MaxAttempts);

            if (dbCreated)
            {
                _logger.LogWarning(
                    "TenantCreationJob: Dropping orphan database {DbName} for tenant {TenantId}.",
                    dbName, tenantId);
                await DropOrphanDatabaseAsync(dbName, $"edu_{tenant.Slug}_user");
            }

            await UpdateStatusAsync(tenant, TenantStatus.Failed);
        }
    }
    // Retries a void step; on non-final failure logs a warning and delays.
    // On the final attempt the exception propagates naturally to the caller.
    private async Task RetryAsync(Func<Task> operation, string stepName)
    {
        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                await operation();
                return;
            }
            catch (Exception ex) when (attempt < MaxAttempts)
            {
                _logger.LogWarning(ex,
                    "TenantCreationJob: Step '{Step}' attempt {Attempt}/{Max} failed. Retrying in {Delay}ms.",
                    stepName, attempt, MaxAttempts, RetryDelayMs * attempt);
                await Task.Delay(RetryDelayMs * attempt);
            }
        }
    }
    // Retries a value-returning step with the same policy.
    private async Task<T> RetryAsync<T>(Func<Task<T>> operation, string stepName)
    {
        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (attempt < MaxAttempts)
            {
                _logger.LogWarning(ex,
                    "TenantCreationJob: Step '{Step}' attempt {Attempt}/{Max} failed. Retrying in {Delay}ms.",
                    stepName, attempt, MaxAttempts, RetryDelayMs * attempt);
                await Task.Delay(RetryDelayMs * attempt);
            }
        }
        // Unreachable: on the final attempt the catch filter is false and the
        // exception exits the loop before reaching this line.
        throw new UnreachableException();
    }
    private async Task CreateDatabaseAsync(string slug)
    {
        var dbName = $"Edumanager_{slug}";
        var masterConnection = _configuration.GetConnectionString("MasterDBConnection")!;

        await using var conn = new NpgsqlConnection(masterConnection);
        await conn.OpenAsync();

        await using var checkCmd = new NpgsqlCommand(
            $"SELECT 1 FROM pg_database WHERE datname = '{dbName}'", conn);

        var exists = await checkCmd.ExecuteScalarAsync();

        if (exists is null)
        {
            await using var createCmd = new NpgsqlCommand(
                $"CREATE DATABASE \"{dbName}\"", conn);
            await createCmd.ExecuteNonQueryAsync();
        }

        var tenantConnBuilder = new NpgsqlConnectionStringBuilder(masterConnection) { Database = dbName };
        await using var tenantConnection = new NpgsqlConnection(tenantConnBuilder.ConnectionString);
        await tenantConnection.OpenAsync();

        await using var grantCmd = new NpgsqlCommand(
            "GRANT ALL ON SCHEMA public TO \"EduManager\";", tenantConnection);
        await grantCmd.ExecuteNonQueryAsync();
    }
    private async Task<DbUserResult> CreateDatabaseUserAsync(string slug, string connectionString)
    {
        var userName = $"edu_{slug}_user";
        var password = GeneratePassword();
        var dbName = $"Edumanager_{slug}";

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            $"""
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = '{userName}') THEN
                    CREATE ROLE "{userName}" WITH LOGIN PASSWORD '{password}';
                ELSE
                ALTER ROLE "{userName}" WITH PASSWORD '{password}';
            END IF;
            END
            $$;

            GRANT CONNECT ON DATABASE "{dbName}" TO "{userName}";
            GRANT USAGE ON SCHEMA public TO "{userName}";
            GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO "{userName}";
            ALTER DEFAULT PRIVILEGES IN SCHEMA public
                GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO "{userName}";
            """, conn);

        await cmd.ExecuteNonQueryAsync();

        return new DbUserResult(true, userName, password);
    }
    private async Task DropOrphanDatabaseAsync(string dbName, string userName)
    {
        try
        {
            var masterConn = _configuration.GetConnectionString("MasterDBConnection")!;
            var builder = new NpgsqlConnectionStringBuilder(masterConn) { Database = "postgres" };

            await using var conn = new NpgsqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                $"""
                SELECT pg_terminate_backend(pid)
                FROM pg_stat_activity
                WHERE datname = '{dbName}' AND pid <> pg_backend_pid();

                DROP DATABASE IF EXISTS "{dbName}";

                DROP ROLE IF EXISTS "{userName}";
                """, conn);

            await cmd.ExecuteNonQueryAsync();

            _logger.LogInformation(
                "TenantCreationJob: Orphan database {DbName} and role {UserName} dropped successfully.",
                dbName, userName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "TenantCreationJob: Failed to drop orphan database {DbName}. Manual cleanup required.",
                dbName);
        }
    }
    private async Task UpdateStatusAsync(Tenant tenant, TenantStatus status)
    {
        tenant.Status = status;
        _repository.Update(tenant);
        await _unitOfWork.SaveChangesAsync();
    }
    private string BuildSuperAdminConnectionString(string slug)
    {
        var masterConn = _configuration.GetConnectionString("MasterDBConnection")!;
        var builder = new NpgsqlConnectionStringBuilder(masterConn)
        {
            Database = $"Edumanager_{slug}",
            IncludeErrorDetail = true
        };
        return builder.ConnectionString;
    }
    private string BuildIsolatedConnectionString(string slug, string user, string password)
    {
        var masterConn = _configuration.GetConnectionString("MasterDBConnection")!;
        var builder = new NpgsqlConnectionStringBuilder(masterConn)
        {
            Database = $"Edumanager_{slug}",
            Username = user,
            Password = password
        };
        return builder.ConnectionString;
    }
    private static string GeneratePassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var bytes = RandomNumberGenerator.GetBytes(32);
        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }
    private async Task InsertTenantConfigAsync(string connectionString, string slugCode)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "INSERT INTO tenant_config (key, value) VALUES ('slug_code', @slugCode)", conn);
        cmd.Parameters.AddWithValue("slugCode", slugCode);
        await cmd.ExecuteNonQueryAsync();
    }
}
