using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TenantFeature.Queries;

public record GetAllTenantQuery(int pageNumber, int pageSize)
    : IRequest<Result<IEnumerable<TenantResponseDto>>>;
