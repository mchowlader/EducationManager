using EduManager.Infrastructure.Persistence;
using EduManager.Infrastructure.Services;
using EduManager.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration
            .GetConnectionString("MasterDBConnection");

        services.AddDbContext<MasterDbContext>(opt =>
            opt.UseNpgsql(connectionString));

        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<ITenantMigrationService, TenantMigrationService>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Tenant migration started...");

    using var scope = host.Services.CreateScope();
    var migrationService = scope.ServiceProvider
        .GetRequiredService<ITenantMigrationService>();

    await migrationService.MigrateAllTenantsAsync();

    logger.LogInformation("All tenants migrated successfully.");
}
catch (Exception ex)
{
    logger.LogError(ex, "Migration failed.");
    Environment.Exit(1);
}