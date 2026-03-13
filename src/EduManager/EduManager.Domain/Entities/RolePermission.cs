using EduManager.Domain.Common;

namespace EduManager.Domain.Entities;

public class RolePermission : BaseEntity, ITenantEntity
{
    public long RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public string Permission { get; set; } = string.Empty;

}
