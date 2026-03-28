using EduManager.Application.DTOs.Feature.TeacherFeature;
using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TeacherFeature.Command;

internal class UpdateeacherCommandHandler(ITeacherService teacherService)
    : IRequestHandler<UpdateTeacherCommand, Result<TeacherResponseDto>>
{
    public async Task<Result<TeacherResponseDto>> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
        => await teacherService.UpdateAsync(request.Id, request.Dto, cancellationToken);
}