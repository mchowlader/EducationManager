using AutoMapper;
using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces.Repositories;
using MediatR;

namespace EduManager.Application.Features.TenantFeature.Queries;

public class GetByIdTenantQueryHandler(ITenantRepository repository, IMapper mapper)
    : IRequestHandler<GetByIdTenantQuery, Result<TenantResponseDto>>
{
    private readonly IMapper _mapper = mapper;
    private readonly ITenantRepository _repository = repository;

    public async Task<Result<TenantResponseDto>> Handle(GetByIdTenantQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _repository.GetByIdAsync(request.id, cancellationToken);

        if (tenant is null)
            return Result<TenantResponseDto>.Success(null, "No data found");

        var mapped = _mapper.Map<TenantResponseDto>(tenant);

        return Result<TenantResponseDto>.Success(mapped);
    }
}
