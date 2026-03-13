using EduManager.Domain.Common;

namespace EduManager.Domain.Entities;

public class UserProfile : BaseEntity, ITenantEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }

    // FK
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public Address Address { get; set; } = new()!;
}
