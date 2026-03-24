using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.RoleFeature;

public record RemoveUserRoleCommand(long UserId, long RoleId)
    : IRequest<Result<object>>;
