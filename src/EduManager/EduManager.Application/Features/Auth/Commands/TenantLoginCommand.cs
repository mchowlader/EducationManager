using EduManager.Application.DTOs.Feature.Auth;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public record TenantLoginCommand(TenantLoginDto dto) : IRequest<Result<TokenResponseDto>>;