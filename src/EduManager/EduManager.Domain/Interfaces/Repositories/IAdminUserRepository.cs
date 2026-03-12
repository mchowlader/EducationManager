using EduManager.Domain.Entities.Master;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManager.Domain.Interfaces.Repositories;

public interface IAdminUserRepository : IRepository<AdminUser>
{
    Task<AdminUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<AdminUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
