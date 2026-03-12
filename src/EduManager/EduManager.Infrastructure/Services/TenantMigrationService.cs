using EduManager.Application.Interfaces;
using EduManager.Domain.Enums;
using EduManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduManager.Infrastructure.Services;

public class TenantMigrationService(
    MasterDbContext context,
    IEncryptionService encryption,
    ILogger<TenantMigrationService> logger) 
    : ITenantMigrationService
{
    private readonly ILogger<TenantMigrationService> _logger = logger;
    private readonly IEncryptionService _encryption = encryption;
    private readonly MasterDbContext _context = context;
    public async Task MigrateAllTenantsAsync()
    {
        var tenants = await _context.Tenants
            .Where(t => !t.IsDelete && t.Status == TenantStatus.Active)
            .ToListAsync();

        foreach (var tenant in tenants)
        {
            try
            {
                var connectionString = _encryption.Decrypt(
                    tenant.ConnectionString,
                    tenant.Slug,
                    tenant.EncryptionSalt);
                await MigrateSingleTenantAsync(connectionString);
                logger.LogInformation("Migration applied: {Slug}", tenant.Slug);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Migration failed: {Slug}", tenant.Slug);
            }
        }
    }

    public async Task MigrateSingleTenantAsync(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EduDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        await using var context = new EduDbContext(optionsBuilder.Options, null!);
        await context.Database.MigrateAsync();
    }
}