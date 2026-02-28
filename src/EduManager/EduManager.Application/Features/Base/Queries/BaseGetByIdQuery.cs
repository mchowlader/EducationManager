using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.Base.Queries;

public record BaseGetByIdQuery<TEntity, TResponseDto>(long id)
    : IRequest<Result<TResponseDto>>
    where TEntity : BaseEntity
    where TResponseDto : class;
