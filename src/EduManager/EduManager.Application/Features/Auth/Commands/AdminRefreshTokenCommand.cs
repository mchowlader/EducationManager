using EduManager.Application.DTOs.Feature.Auth;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public record AdminRefreshTokenCommand(RefreshTokenDto dto) 
    : IRequest<Result<TokenResponseDto>>;