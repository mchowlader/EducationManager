using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;

namespace EduManager.Application.Features.RoleFeature;

public class RemoveRolePermissionCommandHandler(
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveRolePermissionCommand, Result<object>>
{
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<object>> Handle(RemoveRolePermissionCommand request, CancellationToken cancellationToken)
    {
        var rolePermission = await _roleRepository.GetRolePermissionAsync(request.RoleId, request.Permission, cancellationToken);

        if (rolePermission is null)
            return Result<object>.Failure("Permission not found.");

        rolePermission.IsDelete = true;
        _roleRepository.UpdateRolePermission(rolePermission);
        await _unitOfWork.SaveChangesAsync();

        return Result<object>.Success(true, "Permission removed successfully.");
    }
}
