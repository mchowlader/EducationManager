using EduManager.Application.DTOs.Feature.Shared;

namespace EduManager.Application.DTOs.Feature.TeacherFeature;

public record CreateTeacherDto(
    string Email,
    string Password,
    string FullName,
    string Mobile,
    DateTime DateOfBirth,
    string Designation,
    AddressDto Address
);
