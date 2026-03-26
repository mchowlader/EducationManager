using EduManager.Domain.Entities;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduManager.Infrastructure.Repositories;

public class UserRepository(EduDbContext context)
    : BaseRepository<User, EduDbContext>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) 
        => await DbSet.Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
        .FirstOrDefaultAsync(u => u.Email == email && !u.IsDelete, ct);

    public async Task<User?> GetByEmailOrCodeAsync(string emailOrCode, CancellationToken ct = default)
    => await DbSet
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
        .AsSplitQuery()
        .FirstOrDefaultAsync(u =>
            (u.Email == emailOrCode || u.UserCode == emailOrCode)
            && !u.IsDelete, ct);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    => await DbSet
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
        .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDelete, ct);



    public void UpdateUser(User user)
        => DbSet.Update(user);
}
