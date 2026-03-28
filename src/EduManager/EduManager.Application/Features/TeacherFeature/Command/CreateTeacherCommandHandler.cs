using EduManager.Application.DTOs.Feature.TeacherFeature;
using EduManager.Application.Interfaces;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TeacherFeature.Command;

internal class CreateTeacherCommandHandler(ITeacherService teacherService)
    : IRequestHandler<CreateTeacherCommand, Result<TeacherResponseDto>>
{
    public async Task<Result<TeacherResponseDto>> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
        => await teacherService.CreateAsync(request.Dto, cancellationToken);
}
