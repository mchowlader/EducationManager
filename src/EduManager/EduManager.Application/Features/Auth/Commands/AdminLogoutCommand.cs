using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.Auth.Commands;

public record AdminLogoutCommand(long AdminId) : IRequest<Result<bool>>;