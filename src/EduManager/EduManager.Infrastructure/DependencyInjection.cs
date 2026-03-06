using EduManager.Application.Interfaces;
using EduManager.Domain.Constants;
using EduManager.Domain.Enums;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Identity;
using EduManager.Infrastructure.Jobs;
using EduManager.Infrastructure.Persistence;
using EduManager.Infrastructure.Repositories;
using EduManager.Infrastructure.Services;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace EduManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("MasterDBConnection"))
        );

        services.AddHangfireServer();

        services.AddDbContext<MasterDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("MasterDBConnection")));

        services.AddScoped<EduDbContext>(provider =>
        {
            var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            var masterDB = provider.GetRequiredService<MasterDbContext>();

            var tenantSlug = httpContext?.Request?.Host.Host.Split(':')[0] ?? string.Empty;
            var tenant = masterDB.Tenants.FirstOrDefault(t => t.Slug == tenantSlug && t.Status == TenantStatus.Active);
            var optionsBuilder = new DbContextOptionsBuilder<EduDbContext>();

            if (tenant is null)
            {
                //var logger = provider.GetRequiredService<ILogger<EduDbContext>>();
                //logger.LogWarning("Tenant '{TenantSlug}' not found.", tenantSlug);

                optionsBuilder.UseInMemoryDatabase("TenantMissing");
                return new EduDbContext(optionsBuilder.Options);
            }

            optionsBuilder.UseSqlServer(tenant.ConnectionString);

            return new EduDbContext(optionsBuilder.Options);
        });

        //UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMasterUnitOfWork, MasterUnitOfWork>();

        //Repository
        services.AddScoped<ITenantRepository, TenantRepository>();

        //Service
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITenantService, TenantService>();
        //Jobs
        services.AddScoped<ITenantCreationJob, TenantCreationJob>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddHttpContextAccessor();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
            };
        });

        services.AddAuthorization(opt =>
        {
            var permissions = typeof(Permissions)
            .GetFields()
            .Where(f => f.IsLiteral)
            .Select(f => f.GetValue(null)?.ToString())
            .Where(p => p is not null);

            foreach (var permission in permissions)
            {
                opt.AddPolicy(permission!, policy => policy.Requirements.Add(new PermissionRequirement(permission!)));
            }
        });

        return services;
    }
}
