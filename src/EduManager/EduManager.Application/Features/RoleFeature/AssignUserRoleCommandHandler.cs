using EduManager.Application.DTOs.Feature.RoleFeature;
using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;

namespace EduManager.Application.Features.RoleFeature;

public class AssignUserRoleCommandHandler(
    IUserRoleRepository userRoleRepository,
    IUserRepository userRepository,
    IRepository<Role> roleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AssignUserRoleCommand, Result<UserRoleDto>>
{
    public async Task<Result<UserRoleDto>> Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if(user is null)
            return Result<UserRoleDto>.Failure("User not found.");

        var role = await roleRepository.GetByIdAsync(request.Dto.RoleId, cancellationToken);

        if (role is null)
            return Result<UserRoleDto>.Failure("Role not found.");

        var exitingUserRole = await userRoleRepository.GetByUserIdAndRoleIdAsync(request.UserId, request.Dto.RoleId, cancellationToken);

        if (exitingUserRole is not null)
            return Result<UserRoleDto>.Failure("User already has this role.");

        var userRole = new UserRole
        {
            UserId = request.UserId,
            RoleId = request.Dto.RoleId,
        };

        await userRoleRepository.AddAsync(userRole, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<UserRoleDto>.Success(
        new UserRoleDto(
            userRole.UserId,
            userRole.RoleId,
            role.Name,         
            userRole.CreatedAt
        ),
        "Role assigned successfully.");
    }
}
