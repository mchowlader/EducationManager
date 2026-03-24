using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;

namespace EduManager.Application.Features.RoleFeature;

public class AddRolePermissionCommandHandler(
    IRoleRepository repository,
    IRepository<RolePermission> rolePermissionRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddRolePermissionCommand, Result<object>>
{
    private readonly IRoleRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    IRepository<RolePermission> _rolePermissionRepository = rolePermissionRepository;

    public async Task<Result<object>> Handle(AddRolePermissionCommand request, CancellationToken cancellationToken)
    {
        var role = await _repository.GetByIdWithPermissionAsync(request.RoleId, cancellationToken);

        if (role is null)
            return Result<object>.Failure("Role not found.");

        var exists = role.RolePermissions
            .Any(rp => rp.Permission == request.Dto.Permission);

        if (exists)
            return Result<object>.Failure("Permission already assigned to the role.");

        var rolePermission = new RolePermission
        {
            RoleId = request.RoleId,
            Permission = request.Dto.Permission,
        };

        await _rolePermissionRepository.AddAsync(rolePermission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(true, "Permission added successfully.");
    }
}