using EduManager.Application.Interfaces;
using EduManager.Domain.Attributes;
using EduManager.Domain.Common;
using EduManager.Domain.Enums;
using EduManager.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EduManager.Infrastructure.Middleware;

public class TenantMiddleware(
    RequestDelegate next,
    ILogger<TenantMiddleware> logger)
{
    private static readonly string[] SystemPaths =
    [
        "/hangfire",
        "/swagger",
        "/health",
        "/_framework",
        "/scalar"
    ];

    public async Task InvokeAsync(HttpContext context,
        MasterDbContext masterDb,
        IEncryptionService encryption)
    {
        // System paths skip
        var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
        if (SystemPaths.Any(p => path.StartsWith(p)))
        {
            await next(context);
            return;
        }

        // MasterRoute marked → skip
        var endpoint = context.GetEndpoint();
        var isMasterRoute = endpoint?.Metadata
            .GetMetadata<MasterRouteAttribute>() is not null;

        if (isMasterRoute)
        {
            await next(context);
            return;
        }

        var slug = ExtractSlug(context);

        if (string.IsNullOrEmpty(slug))
        {
            await next(context);
            return;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(slug, "^[a-z0-9-]+$"))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Failure("Invalid Tenant Identifier."));
            return;
        }

        var tenant = await masterDb.Tenants
            .FirstOrDefaultAsync(t => t.Slug == slug);

        if (tenant is null || tenant.IsDelete)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Failure("Tenant not found."));
            return;
        }

        if (tenant.Status != TenantStatus.Active)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Failure("Tenant not found."));
            return;
        }

        var connectionString = encryption.Decrypt(
            tenant.ConnectionString, tenant.Slug, tenant.EncryptionSalt);

        context.Items["TenantContext"] = new TenantContext
        {
            TenantId = tenant.Id,
            Slug = slug,
            ConnectionString = connectionString
        };

        logger.LogInformation(
            "Tenant identified: {Slug} (Id: {TenantId})",
            tenant.Slug, tenant.Id);

        await next(context);
    }

    private static string? ExtractSlug(HttpContext context)
    {
        var host = context.Request.Host.Host;
        var parts = host.Split('.');

        if (parts.Length >= 3)
            return parts[0].ToLower();

        if (context.Request.Headers.TryGetValue("X-Tenant-Slug", out var headerSlug))
            return headerSlug.ToString().ToLower();

        return null;
    }
}
