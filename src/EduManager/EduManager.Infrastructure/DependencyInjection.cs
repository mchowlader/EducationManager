using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Constants;
using EduManager.Domain.Entities;
using EduManager.Domain.Exceptions;
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
        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
           "Missing configuration: Jwt:Key");

        var masterSecret = configuration["Encryption:MasterSecret"]
            ?? throw new InvalidOperationException(
                "Missing configuration: Encryption:MasterSecret");

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
                throw new TenantContextNotFoundException();

            return new EduDbContext(optionsBuilder.Options);
        });

        //UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMasterUnitOfWork, MasterUnitOfWork>();

        //Repository
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(EduRepository<>));
        //services.AddScoped<IRepository<UserProfile>, EduRepository<UserProfile>>();

        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        //services.AddScoped<IRepository<RolePermission>, EduRepository<RolePermission>>();
        //services.AddScoped<IRepository<UserRole>, EduRepository<UserRole>>();

        //Service
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITeacherService, TeacherService>();

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
                    Encoding.UTF8.GetBytes(jwtKey))
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
            var entities = typeof(ITenantEntity).Assembly
                .GetTypes()
                .Where(f => f.IsClass && !f.IsAbstract && typeof(ITenantEntity).IsAssignableFrom(f))
                .Select(t => t.Name);

            var actions = new[] { Permissions.View, Permissions.Create, Permissions.Update, Permissions.Delete };

            foreach (var entity in entities)
            {
                foreach (var action in actions)
                {
                    var permission = Permissions.For(entity, action);
                    opt.AddPolicy(permission, policy =>
                        policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            }
        });

        services.AddScoped<ITenantSeeder, TenantSeeder>();
        services.AddScoped<ITenantMigrationService, TenantMigrationService>();

        return services;
    }
}
