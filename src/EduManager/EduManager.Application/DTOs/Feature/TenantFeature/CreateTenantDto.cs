namespace EduManager.Application.DTOs.Feature.TenantFeature;

public record CreateTenantDto
(
    string Name,
    string Slug,
    string Email,
    string Mobile,
    string Password
);