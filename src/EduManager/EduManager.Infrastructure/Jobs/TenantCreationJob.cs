using EduManager.Application.Interfaces;
using EduManager.Domain.Entities.Master;
using EduManager.Domain.Enums;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Security.Cryptography;

namespace EduManager.Infrastructure.Jobs;

public class TenantCreationJob(
    ITenantRepository repository
    , IMasterUnitOfWork unitOfWork
    , IConfiguration configuration
    , IEncryptionService encryption
    , ITenantMigrationService migrationService)
    : ITenantCreationJob
{
    private readonly ITenantRepository _repository = repository;
    private readonly IMasterUnitOfWork _unitOfWork = unitOfWork;
    private IConfiguration _configuration = configuration;
    private readonly IEncryptionService _encryption = encryption;
    private readonly ITenantMigrationService _migrationService = migrationService;

    public async Task ExecutionAsync(long tenantId, string password)
    {
        var tenant = await _repository.GetByIdAsync(tenantId);

        if (tenant is null) return;

        await CreateDatabaseAsync(tenant.Slug);

        var superAdminConn = BuildSuperAdminConnectionString(tenant.Slug);

        // Step 1: Migration only
        try
        {
            await _migrationService.MigrateOnlyAsync(superAdminConn);
        }
        catch
        {
            await UpdateStatusAsync(tenant, TenantStatus.Failed);
            return;
        }

        // Step 2: tenant_config insert — trigger এর জন্য লাগবে
        await InsertTenantConfigAsync(superAdminConn, tenant.SlugCode);

        // Step 3: Seed — এখন trigger কাজ করবে
        try
        {
            await _migrationService.SeedOnlyAsync(superAdminConn, tenant.Email, password);
        }
        catch
        {
            await UpdateStatusAsync(tenant, TenantStatus.Failed);
            return;
        }

        // Step 4: DB User create
        var userResult = await CreateDatabaseUserAsync(tenant.Slug, superAdminConn);

        if (!userResult.IsSuccess)
        {
            await UpdateStatusAsync(tenant, TenantStatus.Failed);
            return;
        }

        var plainConnectionString = BuildIsolatedConnectionString(
            tenant.Slug, userResult.UserName, userResult.Password);
        tenant.ConnectionString = _encryption.Encrypt(
            plainConnectionString, tenant.Slug, tenant.EncryptionSalt);
        tenant.Status = TenantStatus.Active;

        _repository.Update(tenant);
        await _unitOfWork.SaveChangesAsync();
    }
    private async Task<bool> CreateDatabaseAsync(string slug)
    {
        try
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

            // ← Connect to the new database and grant schema permissions
            var tenantConn = new NpgsqlConnectionStringBuilder(masterConnection)
            {
                Database = dbName
            };

            await using var tenantConnection = new NpgsqlConnection(tenantConn.ConnectionString);
            await tenantConnection.OpenAsync();

            await using var grantCmd = new NpgsqlCommand(
                $"GRANT ALL ON SCHEMA public TO \"EduManager\";", tenantConnection);
            await grantCmd.ExecuteNonQueryAsync();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreateDatabaseAsync failed: {ex.Message}");
            return false;
        }
    }
        private async Task<DbUserResult> CreateDatabaseUserAsync(string slug, string connectionString)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"FULL ERROR: {ex}");
                return new DbUserResult(false, string.Empty, string.Empty);
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
