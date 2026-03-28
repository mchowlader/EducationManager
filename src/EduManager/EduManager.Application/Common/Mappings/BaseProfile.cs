using AutoMapper;
using EduManager.Domain.Common;

namespace EduManager.Application.Common.Mappings;

public class BaseProfile<TEntity, TCreateDto, TUpdateDto, TResponseDto> : Profile
    where TEntity : BaseEntity, new()
    where TCreateDto : class
    where TUpdateDto : class
    where TResponseDto : class
{
    public BaseProfile()
    {
        CreateMap<TCreateDto, TEntity>();

        CreateMap<TUpdateDto, TEntity>()
            .ForAllMembers(opt => opt
            .Condition((src, dst, srcMember) => srcMember is not null));

        CreateMap<TEntity, TResponseDto>();
    }
}
