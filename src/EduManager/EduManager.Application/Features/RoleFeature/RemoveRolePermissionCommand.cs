using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.RoleFeature
{
    public record RemoveRolePermissionCommand(long RoleId, string Permission)
        : IRequest<Result<object>>;
}
