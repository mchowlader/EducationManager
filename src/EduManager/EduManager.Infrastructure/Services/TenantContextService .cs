using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace EduManager.Infrastructure.Services;

public class TenantContextService(IHttpContextAccessor httpContextAccessor) : ITenantContext
{
    private TenantContext? context =>
        httpContextAccessor.HttpContext?.Items["TenantContext"] as TenantContext;

    public long TenantId => context?.TenantId ?? 0;

    public string? Slug => context?.Slug ?? string.Empty;

    public string? ConnectionString => context?.ConnectionString ?? string.Empty;

    public bool HasTenant => context is not null;
}
