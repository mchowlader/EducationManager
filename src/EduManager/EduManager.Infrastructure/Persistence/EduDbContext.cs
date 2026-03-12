using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace EduManager.Infrastructure.Persistence;

public class EduDbContext : DbContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public EduDbContext(DbContextOptions<EduDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected EduDbContext()
    {
        _httpContextAccessor = null;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var tenantContext = _httpContextAccessor?.HttpContext?
                .Items["TenantContext"] as TenantContext;

            if(tenantContext?.ConnectionString is not null)
            {
                optionsBuilder.UseNpgsql(tenantContext.ConnectionString);
            }
        }
    }
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

    public DbSet<Classes> Classes => Set<Classes>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Address> Addresss => Set<Address>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
}
