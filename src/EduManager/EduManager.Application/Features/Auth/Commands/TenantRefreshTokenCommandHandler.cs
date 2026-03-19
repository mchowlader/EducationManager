using EduManager.Application.DTOs.Feature.Auth;
using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Services;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public class TenantRefreshTokenCommandHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    ITokenService tokenService,
    ITenantContext tenantContext)
    : IRequestHandler<TenantRefreshTokenCommand, Result<TokenResponseDto>>
{
    private readonly IUserRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ITenantContext _tenantContext = tenantContext;
    public async Task<Result<TokenResponseDto>> Handle(TenantRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByRefreshTokenAsync(request.Dto.RefreshToken, cancellationToken);

        if (user is null || !user.IsActive)
            return Result<TokenResponseDto>.Failure("Invalid request token.");

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<TokenResponseDto>.Failure("Refresh token expired.");

        var permission = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Distinct()
            .ToList();

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _repository.UpdateUser(user);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Result<TokenResponseDto>.Failure("Request was cancelled.");
        }
        catch (Exception ex)
        {
            return Result<TokenResponseDto>.Failure("Token refresh failed. Please try again.");
        }

        var token = _tokenService.GenerateTenantUserToken(user, permission, _tenantContext.TenantId);

        return Result<TokenResponseDto>.Success(
            token with { RefreshToken = newRefreshToken },
            "Token refreshed successfully.");
    }
}
