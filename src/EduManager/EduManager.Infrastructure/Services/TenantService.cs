using EduManager.Application.Interfaces;
using EduManager.Domain.Enums;
using EduManager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EduManager.Infrastructure.Services;

public class TenantService(IHttpContextAccessor contextAccessor, MasterDbContext masterDbContext, IEncryptionService encryption)
    : ITenantService
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    private readonly MasterDbContext _masterDbContext = masterDbContext;
    private readonly IEncryptionService _encryption = encryption;


    public async Task<string?> GetConnectionStringAsync(string slug)
    {
        var data = await _masterDbContext.Tenants
            .Where(t => t.Slug == slug && t.Status == TenantStatus.Active)
            .Select(t => new { t.ConnectionString, t.Slug, t.EncryptionSalt })
            .FirstOrDefaultAsync();

        string plainConnectionString = data is not null
            ? _encryption.Decrypt(data.ConnectionString, data.Slug, data.EncryptionSalt)
            : string.Empty;

        return plainConnectionString ?? string.Empty;
    }

    public string GetCurrentTenantSlug()
    {
        var host = _contextAccessor?.HttpContext?.Request?.Host.Host;
        return host?.Split('.')[0] ?? string.Empty;
    }
}
