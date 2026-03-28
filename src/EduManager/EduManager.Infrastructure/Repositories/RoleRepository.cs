using EduManager.Domain.Entities;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduManager.Infrastructure.Repositories;

public class RoleRepository(EduDbContext context) :
    BaseRepository<Role, EduDbContext>(context), IRoleRepository
{
    private readonly EduDbContext context = context;
    public async Task<Role?> GetByIdWithPermissionAsync(long id, CancellationToken ct = default)
        => await DbSet
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDelete, ct);

    public async Task<Role?> GetByNameAsync(string name, CancellationToken ct)
        => await DbSet.FirstOrDefaultAsync(r => r.Name == name, ct);

    public async Task<RolePermission?> GetRolePermissionAsync(long roleId, string permission, CancellationToken ct = default)
        => await context.RolePermissions
            .FirstOrDefaultAsync(rp =>
            rp.RoleId == roleId &&
            rp.Permission == permission &&
            !rp.IsDelete, ct);

    public void UpdateRolePermission(RolePermission rolePermission) 
        => context.RolePermissions.Update(rolePermission);
}
