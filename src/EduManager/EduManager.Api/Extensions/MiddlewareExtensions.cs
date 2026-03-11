using EduManager.Infrastructure.Middleware;

namespace EduManager.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<TenantMiddleware>();
}
