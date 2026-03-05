using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Domain.Entities;

namespace EduManager.Api.Endpoints;

public class TenantEndpoints : BaseEndPoints<Tenant, CreateTenantDto, UpdateTenantDto, TenantResponseDto>, IEndpoints
{
    public static void MapEndpoints(WebApplication app)
    {
        var group = MapBaseEndPoints(app);
    }
}
