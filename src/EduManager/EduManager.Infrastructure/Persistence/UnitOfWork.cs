using EduManager.Application.Interfaces;
using EduManager.Domain.Interfaces;

namespace EduManager.Infrastructure.Persistence;

public class UnitOfWork(EduDbContext eduDbContext) : IUnitOfWork
{

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => eduDbContext.SaveChangesAsync(cancellationToken);

    public void Dispose() => eduDbContext.Dispose();

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct = default)
        => new EfTransaction(await eduDbContext.Database.BeginTransactionAsync(ct));
}
