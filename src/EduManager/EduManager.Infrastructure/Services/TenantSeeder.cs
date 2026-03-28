using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Constants;
using EduManager.Domain.Entities;
using EduManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduManager.Infrastructure.Services
{
    public class TenantSeeder : ITenantSeeder
    {
        public async Task SeedAsync(string tenantDbconnectionString, string email = "", string password = "")
        {
            var optionBuilder = new DbContextOptionsBuilder<EduDbContext>();
            optionBuilder.UseNpgsql(tenantDbconnectionString);

            // ✅ শুধু options parameter
            await using var context = new EduDbContext(optionBuilder.Options);
            await SeedRoleAsync(context);
            await SeedRolePermissionAsync(context);
            await SeedTenantAdminAsync(context, email, password);
        }

        private static async Task SeedRoleAsync(EduDbContext context)
        {
            if (await context.Roles.AnyAsync()) return;

            var roles = new List<Role>()
            {
                new (){Name = "TenantAdmin", IsDefault = true},
                new (){Name = "Teacher", IsDefault = true},
                new (){Name = "Student", IsDefault = true}
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        private static async Task SeedRolePermissionAsync(EduDbContext context)
        {
            if (await context.RolePermissions.AnyAsync()) return;

            var tenandAdminRole = await context.Roles.FirstAsync(r => r.Name == "TenantAdmin");
            var teacherRole = await context.Roles.FirstAsync(r => r.Name == "Teacher");
            var studentRole = await context.Roles.FirstAsync(r => r.Name == "Student");

            //tenant admin all permission
            var entities = typeof(BaseEntity).Assembly
                .GetTypes()
                .Where(t =>
                    t.IsClass
                    && !t.IsAbstract
                    && typeof(ITenantEntity)
                .IsAssignableFrom(t))
                .Select(t => t.Name)
                .ToArray();

            var actions = new[]
            {
                Permissions.View,
                Permissions.Create,
                Permissions.Update,
                Permissions.Delete
            };

            var tenantAdminPermissions = entities
            .SelectMany(e => actions
            .Select(a => new RolePermission
            {
                RoleId = tenandAdminRole.Id,
                Permission = Permissions.For(e, a)
            }));

            // Teacher — default permissions
            var teacherPermissions = new List<RolePermission>
            {
                new() { RoleId = teacherRole.Id, Permission = Permissions.For("Students", Permissions.View) },
                new() { RoleId = teacherRole.Id, Permission = Permissions.For("Students", Permissions.Create) },
                new() { RoleId = teacherRole.Id, Permission = Permissions.For("Students", Permissions.Update) },
                new() { RoleId = teacherRole.Id, Permission = Permissions.For("Classes", Permissions.View) },
                new() { RoleId = teacherRole.Id, Permission = Permissions.For("Sections", Permissions.View) },
                new() { RoleId = teacherRole.Id, Permission = Permissions.For("Teacher", Permissions.View) },   
                new() { RoleId = teacherRole.Id, Permission = Permissions.For("Teacher", Permissions.Update) },
            };

            // Student — minimum permission
            var studentPermissions = new List<RolePermission>
            {
                new() { RoleId = studentRole.Id, Permission = Permissions.For("Students", Permissions.View) },
                new() { RoleId = studentRole.Id, Permission = Permissions.For("Students", Permissions.Update) },
            };

            await context.RolePermissions.AddRangeAsync(tenantAdminPermissions);
            await context.RolePermissions.AddRangeAsync(teacherPermissions);
            await context.RolePermissions.AddRangeAsync(studentPermissions);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTenantAdminAsync(
        EduDbContext context, string email, string password)
        {
            if (await context.Users.AnyAsync()) return;

            var tenantAdminRole = await context.Roles
                .FirstAsync(r => r.Name == "TenantAdmin");

            var TenantAdminUser = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsDelete = false
            };

            await context.Users.AddAsync(TenantAdminUser);
            await context.SaveChangesAsync();

            var userProfile = new UserProfile
            {
                FullName = "Tenant Admin",
                UserId = TenantAdminUser.Id
            };

            await context.AddAsync(userProfile);

            var userRole = new UserRole
            {
                UserId = TenantAdminUser.Id,
                RoleId = tenantAdminRole.Id
            };

            await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();
        }
    }
}