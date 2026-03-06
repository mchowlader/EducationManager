using EduManager.Application.Interfaces;
using EduManager.Domain.Entities;
using EduManager.Domain.Enums;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace EduManager.Infrastructure.Jobs;

public class TenantCreationJob(
    ITenantRepository repository
    , IMasterUnitOfWork unitOfWork
    , IConfiguration configuration
    , IEncryptionService encryption) 
    : ITenantCreationJob
{
    private readonly ITenantRepository _repository = repository;
    private readonly IMasterUnitOfWork _unitOfWork = unitOfWork;
    private IConfiguration _configuration = configuration;
    private readonly IEncryptionService _encryption = encryption;

    public async Task ExecutionAsync(long tenantId)
    {
        var tenant = await _repository.GetByIdAsync(tenantId);

        if (tenant is null) return;

        await CreateDatabaseAsync(tenant.Slug);
        
        var superAdminConn = BuildSuperAdminConnectionString(tenant.Slug);

        var migrationSuccess = await ApplyMigrationsAsync(superAdminConn);

        if (!migrationSuccess)
        {
            await UpdateStatusAsync(tenant, TenantStatus.Failed);
            return;
        }

        var userResult = await CreateDatabaseUserAsync(tenant.Slug, superAdminConn);

        if (!userResult.IsSuccess)
        {
            await UpdateStatusAsync(tenant, TenantStatus.Failed);
            return;
        }

        var plainConnectionString = BuildIsolatedConnectionString(tenant.Slug, userResult.UserName, userResult.Password);
        tenant.ConnectionString = _encryption.Encrypt(plainConnectionString, tenant.Slug, tenant.EncryptionSalt);
        tenant.Status = TenantStatus.Active;

        _repository.Update(tenant);

        await _unitOfWork.SaveChangesAsync();
    }
    private async Task<bool> CreateDatabaseAsync(string slug)
    {
        var dbName = $"EduManager_{slug}";
        var masterConnection = _configuration.GetConnectionString("MasterDBConnection");

        await using var conn = new SqlConnection(masterConnection);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            $"""
            IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{dbName}')
            CREATE DATABASE [{dbName}]
            """, conn);

        await cmd.ExecuteNonQueryAsync();
        return true;
    }
    private async Task<bool> ApplyMigrationsAsync(string connectionString)
    {
        var optionBuilder = new DbContextOptionsBuilder<EduDbContext>();
        optionBuilder.UseSqlServer(connectionString);

        await using var context = new EduDbContext(optionBuilder.Options);
        await context.Database.MigrateAsync();
        return true;
    }
    private async Task<DbUserResult> CreateDatabaseUserAsync(string slug, string connectionString)
    {
        var userName = $"edu_{slug}_user";
        var password = GeneratePassword();

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(

            $"""
                IF NOT EXISTS(SELECT * FROM sys.server_principals WHERE name = '{userName}')
                BEGIN
                    CREATE LOGIN [{userName}] WITH PASSWORD = '{password}'
                END

                IF NOT EXISTS(SELECT * FROM sys.database_principals WHERE name = '{userName}')
                BEGIN
                    CREATE USER [{userName}] FOR LOGIN [{userName}]
                    GRANT SELECT, INSERT, UPDATE, DELETE TO [{userName}]
                END
            """, conn
        );

        await cmd.ExecuteNonQueryAsync();

        return new DbUserResult(true, userName, password);
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
        var builder = new SqlConnectionStringBuilder(masterConn);
        builder.InitialCatalog = $"EduManager_{slug}";
        return builder.ConnectionString;
    }
    private string BuildIsolatedConnectionString(string slug, string user, string password) =>
         $"Server = .; Database=EduManager_{slug}; user={user}; password={password}; Trusted_Connection=True; TrustServerCertificate=True ";
    private static string GeneratePassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var bytes = RandomNumberGenerator.GetBytes(32);
        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }
}
