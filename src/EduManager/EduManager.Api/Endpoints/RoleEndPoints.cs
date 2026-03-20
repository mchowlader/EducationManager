using EduManager.Application.DTOs.Feature.RoleFeature;
using EduManager.Domain.Entities;

namespace EduManager.Api.Endpoints;

public class RoleEndPoints
    : BaseEndPoints<Role, RoleCreateDto, RoleUpdateDto, RoleResponseDto>, IEndpoints
{
    public static void MapEndpoints(WebApplication app)
    {
        var instance = new RoleEndPoints();
        MapBaseEndPoints(app, instance.SupportedVersions, instance.DerecatedVersions);
    }
}
