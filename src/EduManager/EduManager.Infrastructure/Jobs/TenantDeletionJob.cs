using EduManager.Application.Interfaces;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EduManager.Infrastructure.Jobs;

public class TenantDeletionJob(
    ITenantRepository repository
    , IMasterUnitOfWork unitOfWork
    , IConfiguration configuration
    , ILogger<TenantDeletionJob> logger)
    : ITenantDeletionJob
{
    private readonly ITenantRepository _repository = repository;
    private readonly IMasterUnitOfWork _unitOfWork = unitOfWork;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<TenantDeletionJob> _logger = logger;

    public async Task ExecuteAsync(long tenantId)
    {
        var tenant = await _repository.GetByIdAsync(tenantId);

        if (tenant is null)
        {
            _logger.LogWarning("TenantDeletionJob: Tenant {TenantId} not found", tenantId);
            return;
        }

        var dbName = $"EduManager_{tenant.Slug}";
        var userName = $"edu_{tenant.Slug}_user";

        var dropSuccesses = await DropDatabaseAsync(dbName, userName);

        if (!dropSuccesses)
        {
            _logger.LogError(
                "TenantDeletionJob: Failed to drop database {DbName} for tenant {TenantId}",
                dbName, tenantId);
            return;
        }

        _repository.Delete(tenant);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation(
           "TenantDeletionJob: Tenant {TenantId} fully deleted. DB: {DbName}",
           tenantId, dbName);
    }

    private async Task<bool> DropDatabaseAsync(string dbName, string userName)
    {
        try
        {
            var masterConn = _configuration.GetConnectionString("MasterDBConnection")!;
            var builder = new NpgsqlConnectionStringBuilder(masterConn)
            {
                Database = "postgres"  // master DB
            };

            await using var conn = new NpgsqlConnection(builder.ConnectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                $"""
            -- Terminate active connections
            SELECT pg_terminate_backend(pid)
            FROM pg_stat_activity
            WHERE datname = '{dbName}' AND pid <> pg_backend_pid();

            -- Drop database
            DROP DATABASE IF EXISTS "{dbName}";

            -- Drop role
            DROP ROLE IF EXISTS "{userName}";
            """, conn);

            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to drop database: {DbName}", dbName);
            return false;
        }
    }
}
