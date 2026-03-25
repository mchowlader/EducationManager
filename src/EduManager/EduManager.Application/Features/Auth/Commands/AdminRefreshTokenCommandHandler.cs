using EduManager.Application.DTOs.Feature.Auth;
using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using EduManager.Domain.Interfaces.Repositories;
using EduManager.Infrastructure.Services;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public class AdminRefreshTokenCommandHandler(
    IAdminUserRepository repository,
    IMasterUnitOfWork unitOfWork,
    ITokenService tokenService,
    IEncryptionService encryption)
    : IRequestHandler<AdminRefreshTokenCommand, Result<TokenResponseDto>>
{
    private readonly IAdminUserRepository _repository = repository;
    private readonly IMasterUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IEncryptionService _encryption = encryption;
    public async Task<Result<TokenResponseDto>> Handle(AdminRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var admin = await _repository.GetByRefreshTokenAsync(
            _encryption.HashRefreshToken(request.dto.RefreshToken), cancellationToken);

        if (admin is null || !admin.IsActive)
            return Result<TokenResponseDto>.Failure("Invalid refresh token.");

        if(admin.RefreshTokenExpiry < DateTime.UtcNow)
            return Result<TokenResponseDto>.Failure("Refresh token expired. Please login again.");

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        admin.RefreshToken = _encryption.HashRefreshToken(newRefreshToken);
        admin.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _repository.Update(admin);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateSuperAdminToken(admin);

        return Result<TokenResponseDto>.Success(
            token with { RefreshToken = newRefreshToken },
            "Token refreshed.");
    }
}
