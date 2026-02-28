using EduManager.Domain.Interfaces;

namespace EduManager.Infrastructure.Persistence;

public class UnitOfWork(EduDbContext eduDbContext) : IUnitOfWork
{
    private readonly EduDbContext _eduDbContext = eduDbContext;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) 
        =>  _eduDbContext.SaveChangesAsync(cancellationToken);

    public void Dispose() => _eduDbContext.Dispose();
}
