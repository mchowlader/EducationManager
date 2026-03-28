using EduManager.Application.DTOs.Feature.Auth;
using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Services;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public class AdminLoginCommandHandler(
    IAdminUserRepository repository,
    IMasterUnitOfWork unitOfWork,
    IEncryptionService encryption,
    ITokenService tokenService)
    : IRequestHandler<AdminLoginCommand, Result<TokenResponseDto>>
{
    private readonly IAdminUserRepository _repository = repository;
    private readonly IMasterUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEncryptionService _encryption = encryption;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<Result<TokenResponseDto>> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
    {
        var admin = await _repository.GetByEmailAsync(request.Dto.Email, cancellationToken);

        var passwordValid = admin is not null &&
            _encryption.VerifyPassword(request.Dto.Password, admin.PasswordHash);

        if (admin is null || !passwordValid || !admin.IsActive)
            return Result<TokenResponseDto>.Failure("Invalid user credential.");

        var refreshToken = _tokenService.GenerateRefreshToken();
        admin.RefreshToken = _encryption.HashRefreshToken(refreshToken);
        admin.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _repository.Update(admin);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateSuperAdminToken(admin);

        return Result<TokenResponseDto>.Success(token with {RefreshToken = refreshToken }, "Login successful.");
    }
}
