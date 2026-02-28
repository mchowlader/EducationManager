using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace EduManager.Infrastructure.Persistence;

public class MasterDbContext(DbContextOptions<MasterDbContext> options)
    : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken  = default)
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
}
