using EduManager.Application.Interfaces;
using EduManager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManager.Infrastructure.Services;

public class TenantService(IHttpContextAccessor contextAccessor, MasterDbContext masterDbContext) 
    : ITenantService
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;
    private readonly MasterDbContext _masterDbContext = masterDbContext;

    public async Task<string?> GetConnectionStringAsync(string slug)
    {
        var connectionString = await _masterDbContext.Tenants
            .Where(t => t.Slug == slug && t.IsActive)
            .Select(t => t.ConnectionString)
            .FirstOrDefaultAsync();

        return connectionString ?? string.Empty;
    }

    public string GetCurrentTenantSlug()
    {
        var host = _contextAccessor?.HttpContext?.Request?.Host.Host;
        return host?.Split('.')[0] ?? string.Empty;
    }
}
