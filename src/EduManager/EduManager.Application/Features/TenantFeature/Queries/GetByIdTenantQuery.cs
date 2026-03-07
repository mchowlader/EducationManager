using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TenantFeature.Queries;

public record GetByIdTenantQuery(long id)
    : IRequest<Result<TenantResponseDto>>;
