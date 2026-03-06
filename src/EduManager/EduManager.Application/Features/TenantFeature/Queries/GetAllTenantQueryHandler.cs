using AutoMapper;
using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EduManager.Application.Features.TenantFeature.Queries;

public class GetAllTenantQueryHandler(ITenantRepository repository, IMapper mapper, ILogger<GetAllTenantQueryHandler> logger)
    : IRequestHandler<GetAllTenantQuery, Result<IEnumerable<TenantResponseDto>>>
{
    private readonly IMapper _mapper = mapper;
    private readonly ITenantRepository _repository = repository;
    private readonly ILogger<GetAllTenantQueryHandler> _logger = logger;

    public async Task<Result<IEnumerable<TenantResponseDto>>> Handle(GetAllTenantQuery request, CancellationToken cancellationToken)
    {
        var tenants = await _repository.GetAllAsync(request.pageNumber, request.pageSize, cancellationToken);

        if (tenants is null || !tenants.Any())
            return Result<IEnumerable<TenantResponseDto>>.Success(Enumerable.Empty<TenantResponseDto>(), "No data found");

        var mapped = _mapper.Map<IEnumerable<TenantResponseDto>>(tenants);

        return Result<IEnumerable<TenantResponseDto>>.Success(mapped);
    }
}
