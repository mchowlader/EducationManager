//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;

//namespace EduManager.Infrastructure.Persistence;

//public class EduDbContextFactory : IDesignTimeDbContextFactory<EduDbContext>
//{
//    public EduDbContext CreateDbContext(string[] args)
//    {
//        // Load appsettings.json
//        var configuration = new ConfigurationBuilder()
//            .SetBasePath(Directory.GetCurrentDirectory()) // project root
//            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//            .Build();

//        // Fetch a default/fake tenant connection string from config
//        var connectionString = configuration.GetConnectionString("DefaultTenantConnection");

//        var optionsBuilder = new DbContextOptionsBuilder<EduDbContext>();
//        optionsBuilder.UseNpgsql(connectionString);

//        return new EduDbContext(optionsBuilder.Options, null!);
//    }
//}

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

        // ✅ শুধু options parameter দিয়ে call করো
        return new EduDbContext(optionsBuilder.Options);
    }
}
