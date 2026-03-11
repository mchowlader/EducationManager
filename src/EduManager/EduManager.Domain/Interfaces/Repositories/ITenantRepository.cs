using EduManager.Domain.Entities.Master;

namespace EduManager.Domain.Interfaces.Repositories;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
