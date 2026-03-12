namespace EduManager.Application.Interfaces;

public interface ITenantMigrationService
{
    Task MigrateAllTenantsAsync();
    Task MigrateSingleTenantAsync(string connectionString);
}
