using EduManager.Application.DTOs.Feature.TeacherFeature;
using EduManager.Domain.Common;

namespace EduManager.Application.Interfaces;

public interface ITeacherService
{
    Task<Result<TeacherResponseDto>> CreateAsync(CreateTeacherDto dto, CancellationToken ct = default);
    Task<Result<TeacherResponseDto>> UpdateAsync(long id, UpdateTeacherDto dto, CancellationToken ct = default);
    Task<Result<bool>> DeleteAsync(long id, CancellationToken ct = default);
}
