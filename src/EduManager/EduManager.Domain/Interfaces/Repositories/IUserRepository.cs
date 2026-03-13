using EduManager.Domain.Entities;

namespace EduManager.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailOrCodeAsync(string emailOrCode, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    void UpdateUser(User user);
}
