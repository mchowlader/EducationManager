//using EduManager.Application.Interfaces;
//using EduManager.Domain.Enums;
//using EduManager.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Npgsql;

//namespace EduManager.Infrastructure.Services;

//public class TenantMigrationService(
//    MasterDbContext context,
//    IEncryptionService encryption,
//    ITenantSeeder seeder,
//    IConfiguration configuration,
//    ILogger<TenantMigrationService> logger)
//    : ITenantMigrationService
//{
//    private readonly ILogger<TenantMigrationService> _logger = logger;
//    private readonly IEncryptionService _encryption = encryption;
//    private readonly MasterDbContext _context = context;
//    private readonly ITenantSeeder _seeder = seeder;
//    private readonly IConfiguration _configuration = configuration;
//    public async Task MigrateAllTenantsAsync()
//    {
//        var tenants = await _context.Tenants
//            .Where(t => !t.IsDelete && t.Status == TenantStatus.Active)
//            .ToListAsync();

//        foreach (var tenant in tenants)
//        {
//            try
//            {
//                var masterConn = _configuration.GetConnectionString("MasterDBConnection")!;
//                var superAdminConn = new NpgsqlConnectionStringBuilder(masterConn)
//                {
//                    Database = $"Edumanager_{tenant.Slug}"
//                }.ConnectionString;

//                await MigrateSingleTenantAsync(superAdminConn);

//                _logger.LogInformation("Migration applied: {Slug}", tenant.Slug);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Migration failed: {Slug}", tenant.Slug);
//            }
//        }
//    }

//    public async Task MigrateOnlyAsync(string tenantDbconnectionString)
//    {
//        var optionsBuilder = new DbContextOptionsBuilder<EduDbContext>();
//        optionsBuilder.UseNpgsql(tenantDbconnectionString);

//        await using var context = new EduDbContext(optionsBuilder.Options, null!);
//        await context.Database.MigrateAsync();
//    }

//    public async Task MigrateSingleTenantAsync(string connectionString, string email = "", string password = "")
//    {
//        var optionsBuilder = new DbContextOptionsBuilder<EduDbContext>();
//        optionsBuilder.UseNpgsql(connectionString);

//        await using var context = new EduDbContext(optionsBuilder.Options, null!);
//        await context.Database.MigrateAsync();
//        await _seeder.SeedAsync(connectionString, email, password);
//    }

//    public async Task SeedOnlyAsync(string tenantDbconnectionString, string email, string password)
//    {
//        await seeder.SeedAsync(tenantDbconnectionString, email, password);
//    }
//}

using EduManager.Application.Interfaces;
using EduManager.Domain.Enums;
using EduManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EduManager.Infrastructure.Services;

public class TenantMigrationService(
    MasterDbContext context,
    IEncryptionService encryption,
    ITenantSeeder seeder,
    IConfiguration configuration,
    ILogger<TenantMigrationService> logger)
    : ITenantMigrationService
{
    private readonly ILogger<TenantMigrationService> _logger = logger;
    private readonly MasterDbContext _context = context;
    private readonly ITenantSeeder _seeder = seeder;
    private readonly IConfiguration _configuration = configuration;
    private readonly IEncryptionService _encryption = encryption;

    public async Task MigrateAllTenantsAsync()
    {
        var tenants = await _context.Tenants
            .Where(t => !t.IsDelete && t.Status == TenantStatus.Active)
            .ToListAsync();

        foreach (var tenant in tenants)
        {
            try
            {
                var masterConn = _configuration.GetConnectionString("MasterDBConnection")!;
                var superAdminConn = new NpgsqlConnectionStringBuilder(masterConn)
                {
                    Database = $"Edumanager_{tenant.Slug}"
                }.ConnectionString;

                await MigrateSingleTenantAsync(superAdminConn);

                _logger.LogInformation("Migration applied: {Slug}", tenant.Slug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Migration failed: {Slug}", tenant.Slug);
            }
        }
    }

    public async Task MigrateOnlyAsync(string tenantDbconnectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EduDbContext>();
        optionsBuilder.UseNpgsql(tenantDbconnectionString);

        // ✅ শুধু options parameter
        await using var context = new EduDbContext(optionsBuilder.Options);
        await context.Database.MigrateAsync();
    }

    public async Task MigrateSingleTenantAsync(string connectionString, string email = "", string password = "")
    {
        var optionsBuilder = new DbContextOptionsBuilder<EduDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // ✅ শুধু options parameter
        await using var context = new EduDbContext(optionsBuilder.Options);
        await context.Database.MigrateAsync();
        await _seeder.SeedAsync(connectionString, email, password);
    }

    public async Task SeedOnlyAsync(string tenantDbconnectionString, string email, string password)
    {
        await _seeder.SeedAsync(tenantDbconnectionString, email, password);
    }
}