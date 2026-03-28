using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TeacherFeature.Command;

public class DeleteTeacherCommandHandler(ITeacherService teacherService)
    : IRequestHandler<DeleteTeacherCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteTeacherCommand request, CancellationToken cancellationToken)
        => await teacherService.DeleteAsync(request.Id, cancellationToken);
}
