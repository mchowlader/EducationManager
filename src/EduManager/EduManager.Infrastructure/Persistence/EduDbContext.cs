using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using EduManager.Infrastructure.Persistence.Configurations.EduTenants;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace EduManager.Infrastructure.Persistence;

public class EduDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    // ✅ Primary constructor - Runtime use
    public EduDbContext(DbContextOptions<EduDbContext> options) : base(options)
    {
    }

    public EduDbContext(DbContextOptions<EduDbContext> options,
        ICurrentUserService? currentUserService = null) 
    : base(options)
    {
        _currentUserService = currentUserService;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            t => t.IsSubclassOf(typeof(EduEntityConfiguration)));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserService?.UserId ?? 0;

        foreach (var entity in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Entity.CreatedAt = DateTime.UtcNow;
                    entity.Entity.IsDelete = false;
                    entity.Entity.CreateBy = userId;  
                    break;

                case EntityState.Modified:
                    entity.Entity.UpdatedAt = DateTime.UtcNow;
                    entity.Entity.UpdateBy = userId;  
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

    public DbSet<Classes> Classes => Set<Classes>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Section> Sections => Set<Section>();
}