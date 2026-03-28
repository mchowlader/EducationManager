using EduManager.Application.DTOs.Feature.TeacherFeature;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TeacherFeature.Queries;

public record GetTeacherByIdQuery(long Id)
    : IRequest<Result<TeacherResponseDto>>;