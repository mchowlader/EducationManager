namespace EduManager.Application.DTOs.Feature.TenantFeature;

public record UpdateTenantDto
(
    string Name,
    string Slug,
    string Email,
    string Mobile
);