using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.Base.Commands;

public record BaseCreateCommand<TEntity, TCreateDto, TResponseDto>(TCreateDto Dto)
    : IRequest<Result<TResponseDto>>
    where TEntity : BaseEntity, new()
     where TCreateDto : class
     where TResponseDto : class;
