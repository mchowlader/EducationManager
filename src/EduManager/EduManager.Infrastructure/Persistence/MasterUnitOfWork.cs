using EduManager.Domain.Interfaces;

namespace EduManager.Infrastructure.Persistence;

public class MasterUnitOfWork(MasterDbContext masterDbContext) : IMasterUnitOfWork
{
    private readonly MasterDbContext _masterDbContext = masterDbContext;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _masterDbContext.SaveChangesAsync(cancellationToken);

    public void Dispose() => _masterDbContext.Dispose();
}