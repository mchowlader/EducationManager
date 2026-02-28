using AutoMapper;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using MediatR;

namespace EduManager.Application.Features.Base.Queries;

public class BaseGetAllQueryHandler<TEntity, TResponseDto>(
    IRepository<TEntity> repository,
    IMapper mapper)
    : IRequestHandler<BaseGetAllQuery<TEntity, TResponseDto>, Result<IEnumerable<TResponseDto>>>
    where TEntity : BaseEntity, new()
    where TResponseDto : class
{
    public async Task<Result<IEnumerable<TResponseDto>>> Handle(BaseGetAllQuery<TEntity, TResponseDto> request, CancellationToken cancellationToken)
    {
        var entities = repository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

        if(entities is null)
            return Result<IEnumerable<TResponseDto>>.Failure($"{typeof(TEntity).Name} not found ");

        var data = mapper.Map<IEnumerable<TResponseDto>>(entities);

        return Result<IEnumerable<TResponseDto>>.Success(data);
    }
}
