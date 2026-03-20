using EduManager.Application.Features.Base.Commands;
using EduManager.Domain.Common;
using EduManager.Domain.Entities;
using EduManager.Domain.Interfaces;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public class RoleDeleteCommandHandler(
    IRepository<Role> repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<BaseDeleteCommand<Role>, Result<bool>>
{
    public readonly IRepository<Role> _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<bool>> Handle(BaseDeleteCommand<Role> request, CancellationToken cancellationToken)
    {
        var role = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (role is null)
            return Result<bool>.Failure("Role not found.");
        if (role.IsDefault)
            return Result<bool>.Failure("Default roles cannot be deleted.");
        role.IsDefault = true;
        _repository.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Role Deleted successfully");
    }
}
