using EduManager.Application.DTOs.Feature.TeacherFeature;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TeacherFeature.Command;

public record CreateTeacherCommand(CreateTeacherDto Dto)
    : IRequest<Result<TeacherResponseDto>>;
