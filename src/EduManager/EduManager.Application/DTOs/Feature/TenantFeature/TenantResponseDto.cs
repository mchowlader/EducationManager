using EduManager.Domain.Enums;

namespace EduManager.Application.DTOs.Feature.TenantFeature;

public record TenantResponseDto
(
    long Id,
    string Name,
    string Slug,
    TenantStatus Status
);