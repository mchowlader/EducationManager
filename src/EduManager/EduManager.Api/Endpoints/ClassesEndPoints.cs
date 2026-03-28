using EduManager.Application.DTOs.Feature.ClassesFeature;
using EduManager.Domain.Entities;

namespace EduManager.Api.Endpoints;

public class ClassesEndPoints : BaseEndPoints<Classes, CreateClassesDTO, UpdateClassesDto, ClassesResponseDto>, IEndpoints
{
    public static void MapEndpoints(WebApplication app)
    {
        var instance = new ClassesEndPoints();
        MapBaseEndPoints(app, instance.SupportedVersions, instance.DerecatedVersions);
    }
}
