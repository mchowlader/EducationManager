namespace EduManager.Application.DTOs.Feature.ClassesFeature;

public record ClassesResponseDto(
    long Id,
    string Name,
    string Description,
    DateTime CreatedAt
);
