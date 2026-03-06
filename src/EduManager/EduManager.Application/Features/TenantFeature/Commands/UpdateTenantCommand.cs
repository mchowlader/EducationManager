using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TenantFeature.Commands;

public record UpdateTenantCommand(UpdateTenantDto Dto) : IRequest<Result<TenantResponseDto>>;