using AutoMapper;
using EduManager.Domain.Common;
using EduManager.Domain.Interfaces;
using MediatR;

namespace EduManager.Application.Features.Base.Queries;

public class BaseGetByIdQueryHandler<TEntity, TResponseDto>(
    IRepository<TEntity> repository,
    IMapper mapper)
    : IRequestHandler<BaseGetByIdQuery<TEntity, TResponseDto>, Result<TResponseDto>>
    where TEntity : BaseEntity, new()
    where TResponseDto : class
{
    public async Task<Result<TResponseDto>> Handle(BaseGetByIdQuery<TEntity, TResponseDto> request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.id, cancellationToken);

        if (entity is null)
            return Result<TResponseDto>.Failure($"{typeof(TEntity).Name} not found");

        var data = mapper.Map<TResponseDto>(entity);

        return Result<TResponseDto>.Success(data);
    }
}
