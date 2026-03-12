using EduManager.Domain.Common;

namespace EduManager.Domain.Entities;

public class User : BaseEntity
{
    public string UserCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public Address Address { get; set; } = new();  // ← Address এখানে

    public ICollection<UserRole> UserRoles { get; set; } = [];
}
