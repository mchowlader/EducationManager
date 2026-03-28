using EduManager.Application.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace EduManager.Infrastructure.Persistence;

public class EfTransaction(IDbContextTransaction transaction) : ITransaction
{
    public Task CommitAsync(CancellationToken ct = default)
        => transaction.CommitAsync(ct);

    public ValueTask DisposeAsync()
        => transaction.DisposeAsync();

    public Task RollbackAsync(CancellationToken ct = default)
        => transaction.RollbackAsync(ct);
}
