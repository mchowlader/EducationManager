using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Entities.Master;
using EduManager.Infrastructure.Persistence.Configurations.Master;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EduManager.Infrastructure.Persistence;

public class MasterDbContext(DbContextOptions<MasterDbContext> options,
    ICurrentUserService? currentUserService = null)
    : DbContext(options)
{
    private readonly ICurrentUserService? _currentUserService = currentUserService;
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Log> Logs => Set<Log>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly(),
            t => t.IsSubclassOf(typeof(MasterEntityConfiguration)));

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
}
