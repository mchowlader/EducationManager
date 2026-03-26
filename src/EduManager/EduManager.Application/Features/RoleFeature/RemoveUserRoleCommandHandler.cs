using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;

namespace EduManager.Application.Features.RoleFeature;

public class RemoveUserRoleCommandHandler(
    IUserRoleRepository userRoleRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveUserRoleCommand, Result>
{
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
    {
        var userRole = await _userRoleRepository.GetByUserIdAndRoleIdAsync(request.UserId, request.RoleId, cancellationToken);

        if (userRole is null)
            return Result.Failure("User role not found.");

        userRole.IsDelete = true;
        _userRoleRepository.Update(userRole);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Role removed successfully.");
    }
}
