using AutoMapper;
using EduManager.Domain.Entities.Master;

namespace EduManager.Application.DTOs.Feature.TenantFeature;

public class TenantProfile : Profile
{
    public TenantProfile()
    {
        CreateMap<CreateTenantDto, Tenant>();
        CreateMap<Tenant, TenantResponseDto>();
    }
}
