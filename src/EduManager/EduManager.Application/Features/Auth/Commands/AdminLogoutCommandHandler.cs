using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public class AdminLogoutCommandHandler(
    IAdminUserRepository repository,
    IMasterUnitOfWork unitOfWork)
    : IRequestHandler<AdminLogoutCommand, Result<bool>>
{
    private readonly IAdminUserRepository _repository = repository;
    private readonly IMasterUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<bool>> Handle(AdminLogoutCommand request, CancellationToken cancellationToken)
    {
        var admin = await _repository.GetByIdAsync(request.AdminId, cancellationToken);

        if (admin is null)
            return Result<bool>.Failure("Admin not found.");

        admin.RefreshToken = null;
        admin.RefreshTokenExpiry = null;

        _repository.Update(admin);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, "Logged ou successfully.");
    }
}
