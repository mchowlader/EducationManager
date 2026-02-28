using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace EduManager.Infrastructure.Persistence;

public class EduDbContext : DbContext
{
    public EduDbContext(DbContextOptions<EduDbContext> options) : base(options) { }

    protected EduDbContext() { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach(var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach(var entity in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Entity.CreatedAt = DateTime.UtcNow;
                    entity.Entity.IsDelete = false;
                    break;

                case EntityState.Modified:
                    entity.Entity.UpdatedAt = DateTime.UtcNow;
                    break;  
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static LambdaExpression? BuildSoftDeleteFilter(Type entityType)
    {
        var param = Expression.Parameter(entityType, "e");
        var body = Expression.Equal(
            Expression.Property(param, nameof(BaseEntity.IsDelete)),
            Expression.Constant(false)
        );

        return Expression.Lambda(body, param);
    }

    public DbSet<Classes> Classes { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
}
