using EduManager.Application.DTOs.Feature.RoleFeature;
using EduManager.Domain.Entities;

namespace EduManager.Application.Common.Mappings;

public class RoleProfile
    : BaseProfile<Role, RoleCreateDto, RoleUpdateDto, RoleResponseDto>;