using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.Base.Commands;

public record BaseUpdateCommand<TEntity, TUpdateDto, TResponseDto>(long Id, TUpdateDto Dto)
    : IRequest<Result<TResponseDto>>
    where TEntity : BaseEntity, new()
    where TUpdateDto : class
    where TResponseDto : class;