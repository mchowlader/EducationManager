using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;

namespace EduManager.Application.Features.RoleFeature;

public class AssignUserRoleCommandHandler(
    IUserRepository userRepository,
    IRepository<Role> roleRepository,
    IRepository<UserRole> userRoleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AssignUserRoleCommand, Result<Object>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRepository<Role> _roleRepository = roleRepository;
    private IRepository<UserRole> _userRoleRepository = userRoleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<object>> Handle(AssignUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if(user is null)
            return Result<object>.Failure("User not found.");

        var role = await _roleRepository.GetByIdAsync(request.Dto.RoleId, cancellationToken);

        if (role is null)
            return Result<object>.Failure("Role not found.");

        var userRole = new UserRole
        {
            UserId = request.UserId,
            RoleId = request.Dto.RoleId,
        };

        await _userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(true, "Role assigned successfully.");
    }
}
