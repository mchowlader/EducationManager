namespace EduManager.Application.DTOs.Feature.RoleFeature;

public record UserRoleDto(
    long UserId,
    long RoleId,
    string RoleName,
    DateTime AssignedAt
);