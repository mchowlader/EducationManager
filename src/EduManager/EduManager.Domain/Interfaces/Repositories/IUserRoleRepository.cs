using EduManager.Domain.Entities;

namespace EduManager.Domain.Interfaces.Repositories;

public interface IUserRoleRepository : IRepository<UserRole>
{
    Task<UserRole?> GetByUserIdAndRoleIdAsync(long userId, long roleId, CancellationToken ct = default);
    Task<IEnumerable<UserRole>> GetByUserIdAsync(long userId, CancellationToken ct = default);
}