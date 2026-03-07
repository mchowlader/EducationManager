using EduManager.Domain.Enums;

namespace EduManager.Application.DTOs.Feature.TenantFeature;

public record TenantResponseDto
(
    long Id,
    string Name,
    string Email,
    string Slug,
    string Mobile,
    long CreateBy,
    DateTime CreatedAt,
    long? UpdateBy,
    DateTime? UpdatedAt,
    TenantStatus Status
);