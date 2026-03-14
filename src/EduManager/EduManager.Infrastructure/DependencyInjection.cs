using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Constants;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Identity;
using EduManager.Infrastructure.Jobs;
using EduManager.Infrastructure.Persistence;
using EduManager.Infrastructure.Repositories;
using EduManager.Infrastructure.Services;
using Hangfire;
using Hangfire.PostgreSql;
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
         .UsePostgreSqlStorage(c =>
             c.UseNpgsqlConnection(configuration.GetConnectionString("MasterDBConnection")))
        );

        services.AddHangfireServer();

        services.AddDbContext<MasterDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("MasterDBConnection")));

        services.AddHttpContextAccessor();

        services.AddScoped<EduDbContext>(sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var tenantContext = httpContextAccessor.HttpContext?.Items["TenantContext"] as TenantContext;

            var optionsBuilder = new DbContextOptionsBuilder<EduDbContext>();

            if (tenantContext?.ConnectionString != null)
                optionsBuilder.UseNpgsql(tenantContext.ConnectionString);
            else
                optionsBuilder.UseNpgsql("Host=localhost;Database=placeholder;");

            return new EduDbContext(optionsBuilder.Options);
        });

        //UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMasterUnitOfWork, MasterUnitOfWork>();

        //Repository
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        //Service
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITenantService, TenantService>();

        //Jobs
        services.AddScoped<ITenantCreationJob, TenantCreationJob>();
        services.AddScoped<ITenantDeletionJob, TenantDeletionJob>();
        services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        services.AddSingleton<IEncryptionService, EncryptionService>();

        //Middleware
        services.AddScoped<ITenantContext, TenantContextService>();

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
            // Master Admin Policies
            opt.AddPolicy("MasterOwnerOnly", policy =>
                policy.RequireRole("Owner"));

            opt.AddPolicy("MasterAdminAccess", policy =>
                policy.RequireRole("Owner", "SuperAdmin"));

            opt.AddPolicy("MasterViewAccess", policy =>
                policy.RequireRole("Owner", "SuperAdmin", "Support"));

            // Tenant Permission Policies — BaseEndpoints er jonno
            var permissions = typeof(Permissions)
                .GetFields()
                .Where(f => f.IsLiteral)
                .Select(f => f.GetValue(null)?.ToString())
                .Where(p => p is not null);

            foreach (var permission in permissions)
            {
                opt.AddPolicy(permission!, policy =>
                    policy.Requirements.Add(new PermissionRequirement(permission!)));
            }
        });

        services.AddScoped<ITenantSeeder, TenantSeeder>();
        services.AddScoped<ITenantMigrationService, TenantMigrationService>();

        return services;
    }
}
