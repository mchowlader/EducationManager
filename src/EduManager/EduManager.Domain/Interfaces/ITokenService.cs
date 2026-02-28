using EduManager.Domain.Entities;

namespace EduManager.Infrastructure.Services;

public interface ITokenService
{
    string GenerateToken(User user, IEnumerable<string> permissions);
}