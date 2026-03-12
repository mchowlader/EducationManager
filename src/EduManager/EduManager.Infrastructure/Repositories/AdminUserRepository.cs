using EduManager.Domain.Entities.Master;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduManager.Infrastructure.Repositories;

public class AdminUserRepository(MasterDbContext context)
    : BaseRepository<AdminUser, MasterDbContext>(context), IAdminUserRepository
{
    public async Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await DbSet
        .FirstOrDefaultAsync(e => e.Email == email & !e.IsDelete, cancellationToken);

    public async Task<AdminUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        => await DbSet
        .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken && !a.IsDelete, cancellationToken);
}
