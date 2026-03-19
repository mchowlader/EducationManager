using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public class TenantLogoutCommandHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<TenantLogoutCommand, Result<object>>
{
    private readonly IUserRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<object>> Handle(TenantLogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId);

        if (user is null)
            return Result<object>.Failure("User not found.");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;

        _repository.UpdateUser(user);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Result<object>.Failure("Request was cancelled.");
        }
        catch
        {
            return Result<object>.Failure("Logout failed. Please try again.");
        }

        return Result<object>.Success(true, "Logged out successfully.");
    }
}