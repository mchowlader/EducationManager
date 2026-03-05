using AutoMapper;
using EduManager.Domain.Entities;

namespace EduManager.Application.DTOs.Feature.TenantFeature;

public class TenantProfile : Profile
{
    protected TenantProfile()
    {
        CreateMap<CreateTenantDto, Tenant>();
        CreateMap<Tenant, TenantResponseDto>();
    }
}
