using EduManager.Application.DTOs.Feature.Shared;

namespace EduManager.Application.DTOs.Feature.TeacherFeature;

public record UpdateTeacherDto(
    string? FullName,
    string? Mobile,
    DateTime? DateOfBirth,
    string? Designation,
    AddressDto? Address
);
