using EduManager.Application.DTOs.Feature.Auth;
using EduManager.Domain.Entities;
using EduManager.Domain.Entities.Master;

namespace EduManager.Infrastructure.Services;

public interface ITokenService
{
    TokenResponseDto GenerateSuperAdminToken(AdminUser admin);
    string GenerateToken(User user, IEnumerable<string> permissions);
}