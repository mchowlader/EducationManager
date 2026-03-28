using EduManager.Application.DTOs.Feature.Shared;

namespace EduManager.Application.DTOs.Feature.TeacherFeature;

public record TeacherResponseDto(
    long Id,
    string TeacherCode,
    string Designation,
    bool IsActive,
    string UserCode,
    string Email,
    string FullName,
    string Mobile,
    DateTime? DateOfBirth,
    AddressDto Address
);
