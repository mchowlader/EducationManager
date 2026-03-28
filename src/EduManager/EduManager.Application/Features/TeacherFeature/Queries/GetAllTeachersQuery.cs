using EduManager.Application.DTOs.Feature.TeacherFeature;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TeacherFeature.Queries;

public record GetAllTeachersQuery(int PageNumber, int PageSize)
    : IRequest<Result<IEnumerable<TeacherResponseDto>>>;