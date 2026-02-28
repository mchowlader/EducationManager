using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.Base.Commands;

public record BaseDeleteCommand<TEntity>(long Id)
    : IRequest<Result<bool>>
    where TEntity : BaseEntity, new();
