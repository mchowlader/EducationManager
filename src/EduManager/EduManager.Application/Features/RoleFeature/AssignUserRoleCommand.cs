using EduManager.Application.DTOs.Feature.RoleFeature;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.RoleFeature;

public record AssignUserRoleCommand(long UserId, AssignUserRoleDto Dto)
    : IRequest<Result<UserRoleDto>>;