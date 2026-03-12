using EduManager.Domain.Entities.Master;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduManager.Infrastructure.Repositories;

public class TenantRepository(MasterDbContext context) 
    : BaseRepository<Tenant, MasterDbContext>(context), ITenantRepository
{
    private readonly MasterDbContext _context = context;

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) =>
        _context.Tenants.AnyAsync(e => e.Email == email.ToLower(), cancellationToken);

    public async Task<IEnumerable<string>> GetAllSlugCodesAsync(CancellationToken cancellationToken) =>
        await _context.Tenants
            .Where(x => x.Slug != null)
            .Select(x => x.Slug)
            .ToListAsync(cancellationToken);
    

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default) =>
        _context.Tenants.AnyAsync(s => s.Slug == slug.ToLower(), cancellationToken);
}
