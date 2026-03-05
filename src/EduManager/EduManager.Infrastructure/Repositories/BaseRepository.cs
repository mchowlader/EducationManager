using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduManager.Infrastructure.Repositories;

public class BaseRepository<T, TContext>(TContext context)
    : IRepository<T>
    where T : BaseEntity
    where TContext : DbContext
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await DbSet.AddAsync(entity, cancellationToken);

    public void Delete(T entity) =>
        DbSet.Remove(entity);

    public async Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) =>
        await DbSet
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        DbSet.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public void Update(T entity) =>
        DbSet.Update(entity);

}