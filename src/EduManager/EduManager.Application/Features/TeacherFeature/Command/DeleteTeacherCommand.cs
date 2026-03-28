using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TeacherFeature.Command;

public record DeleteTeacherCommand(long Id) : IRequest<Result<bool>>;
