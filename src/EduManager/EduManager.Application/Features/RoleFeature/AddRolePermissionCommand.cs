using EduManager.Application.DTOs.Feature.RoleFeature;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.RoleFeature;

public record AddRolePermissionCommand(long RoleId, AddRolePermissionDto Dto) 
    : IRequest<Result<object>>;
