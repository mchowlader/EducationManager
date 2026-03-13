using EduManager.Domain.Common;

namespace EduManager.Domain.Entities;

public class Role : BaseEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
}
