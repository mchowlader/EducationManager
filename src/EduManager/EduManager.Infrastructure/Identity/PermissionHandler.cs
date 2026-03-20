using Microsoft.AspNetCore.Authorization;

namespace EduManager.Infrastructure.Identity;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permission = context.User
            .FindAll("permission")
            .Select(c => c.Value);

        if(permission.Contains(requirement.Permission))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
