using EduManager.Domain.Common;

namespace EduManager.Domain.Entities;

public class User : BaseEntity, ITenantEntity
{
    public string UserCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public UserProfile Profile { get; set; } = null!;
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
