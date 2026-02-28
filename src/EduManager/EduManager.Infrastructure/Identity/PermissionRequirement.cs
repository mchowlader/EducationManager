using Microsoft.AspNetCore.Authorization;

namespace EduManager.Infrastructure.Identity;

public record PermissionRequirement(string Permission) : IAuthorizationRequirement;