using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.Base.Queries;

public record BaseGetAllQuery<TEntity, TResponseDto>(int PageNumber = 1, int PageSize = 10) 
    : IRequest<Result<IEnumerable<TResponseDto>>>
      where TEntity : BaseEntity
      where TResponseDto : class;
