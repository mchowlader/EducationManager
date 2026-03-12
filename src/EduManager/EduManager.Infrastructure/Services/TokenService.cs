using EduManager.Application.DTOs.Feature.Auth;
using EduManager.Domain.Entities;
using EduManager.Domain.Entities.Master;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EduManager.Infrastructure.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public TokenResponseDto GenerateSuperAdminToken(AdminUser admin)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, admin.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, admin.Email),
            new(ClaimTypes.Name, admin.FullName),
            new(ClaimTypes.Role, admin.Role.ToString()),  // Owner / SuperAdmin / Support
            new("tenantId", string.Empty)
        };
            
        return GenerateToken(claims);
    }
    private TokenResponseDto GenerateToken(List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(
             Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(30);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: credentials);

        return new TokenResponseDto(
            AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken: GenerateRefreshToken(),
            TokenType: "Bearer",
            ExpiresIn: (int)(expiry - DateTime.UtcNow).TotalSeconds);
    }
    public string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public string GenerateToken(User user, IEnumerable<string> permissions)
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            ..permissions.Select(p => new Claim("permission", p))
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
