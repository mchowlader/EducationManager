using EduManager.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EduManager.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor)
    : ICurrentUserService
{
    public long UserId
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return long.TryParse(claim, out var id) ? id : 0;
        }
    }

    public bool IsAuthenticated
        => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
