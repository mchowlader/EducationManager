using EduManager.Application.DTOs.Feature.ClassesFeature;
using EduManager.Domain.Entities;

namespace EduManager.Api.Endpoints;

public class ClassesEndPoints : BaseEndPoints<Classes, ClassesCreateDto, ClassesUpdateDto, ClassesResponseDto>, IEndpoints
{
    public static void MapEndpoints(WebApplication app)
    {
        var group = MapBaseEndPoints(app);
    }
}
