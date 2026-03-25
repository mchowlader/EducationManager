using EduManager.Application.DTOs.Feature.Auth;
using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Services;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public class TenantLoginCommandHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    IEncryptionService encryption,
    ITokenService tokenService,
    ITenantContext tenantContext)
    : IRequestHandler<TenantLoginCommand, Result<TokenResponseDto>>
{
    private readonly IUserRepository _repository = repository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _encryption = encryption;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ITenantContext _tenantContext = tenantContext;
    public async Task<Result<TokenResponseDto>> Handle(
    TenantLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByEmailOrCodeAsync(
            request.dto.EmailOrCode, cancellationToken);

        var passwordValid = user is not null &&
            _encryption.VerifyPassword(request.dto.Password, user.PasswordHash);

        if (user is null || !passwordValid || user.IsDelete || !user.IsActive)
            return Result<TokenResponseDto>.Failure("Invalid email or password.");

        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission)
            .Distinct()
            .ToList();

        if (!permissions.Any())
            return Result<TokenResponseDto>.Failure("User has no permissions assigned.");

        var refreshToken = _tokenService.GenerateRefreshToken();
        user.RefreshToken = _encryption.HashRefreshToken(refreshToken);
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _repository.UpdateUser(user);

        try
        {
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
        }
        catch
        {
            return Result<TokenResponseDto>.Failure("Login failed. Please try again.");
        }

        var token = _tokenService.GenerateTenantUserToken(
            user, permissions, _tenantContext.TenantId);

        return Result<TokenResponseDto>.Success(
            token with { RefreshToken = refreshToken }, "Login successful.");
    }
}
