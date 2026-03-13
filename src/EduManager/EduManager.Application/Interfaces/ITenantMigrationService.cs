namespace EduManager.Application.Interfaces;

public interface ITenantMigrationService
{
    Task MigrateAllTenantsAsync();
    Task MigrateOnlyAsync(string tenantDbconnectionString);
    Task SeedOnlyAsync(string connectionString, string email, string password);
}
