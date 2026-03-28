using EduManager.Domain.Entities;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduManager.Infrastructure.Repositories;

public class UserRoleRepository(EduDbContext context)
    : BaseRepository<UserRole, EduDbContext>(context), IUserRoleRepository
{
    public async Task<UserRole?> GetByUserIdAndRoleIdAsync(long userId, long roleId, CancellationToken ct = default)
        => await DbSet
            .FirstOrDefaultAsync(ur =>
                ur.UserId == userId &&
                ur.RoleId == roleId &&
                !ur.IsDelete, ct);

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(long userId, CancellationToken ct = default)
        => await DbSet
            .Where(ur => ur.UserId == userId && !ur.IsDelete)
            .ToListAsync(ct);
}
