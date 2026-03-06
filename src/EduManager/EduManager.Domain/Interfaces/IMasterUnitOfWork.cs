namespace EduManager.Domain.Interfaces;

public interface IMasterUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
