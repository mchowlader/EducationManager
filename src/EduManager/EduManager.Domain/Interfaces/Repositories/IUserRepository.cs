using EduManager.Domain.Entities;
using EduManager.Domain.Entities.Master;

namespace EduManager.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailOrCodeAsync(string emailOrCode, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    void UpdateUser(User user);
}
