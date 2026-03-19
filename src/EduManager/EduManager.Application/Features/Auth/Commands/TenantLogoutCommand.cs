using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public record TenantLogoutCommand(long UserId)
    : IRequest<Result<object>>;