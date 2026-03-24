using EduManager.Domain.Entities;

namespace EduManager.Domain.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdWithPermissionAsync(long id, CancellationToken ct = default);
    Task<RolePermission?> GetRolePermissionAsync(long roleId, string permission, CancellationToken ct = default);
    void UpdateRolePermission(RolePermission rolePermission);
}
