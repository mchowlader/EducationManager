using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EduManager.Infrastructure.Persistence;

public class EduDbContextFactory : IDesignTimeDbContextFactory<EduDbContext>
{
    public EduDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultTenantConnection");

        var optionsBuilder = new DbContextOptionsBuilder<EduDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        //  Only call by options parameter
        return new EduDbContext(optionsBuilder.Options);
    }
}
