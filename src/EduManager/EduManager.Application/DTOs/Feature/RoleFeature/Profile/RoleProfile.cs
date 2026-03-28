using EduManager.Application.Common.Mappings;
using EduManager.Application.DTOs.Feature.RoleFeature;
using EduManager.Domain.Entities;

namespace EduManager.Application.DTOs.Feature.RoleFeature.Profile;

public class RoleProfile
    : BaseProfile<Role, RoleCreateDto, RoleUpdateDto, RoleResponseDto>;