using EduManager.Application.Common.Mappings;
using EduManager.Domain.Entities;

namespace EduManager.Application.DTOs.Feature.ClassesFeature;

public class ClassesProfile : BaseProfile<Classes, ClassesCreateDto, ClassesUpdateDto, ClassesResponseDto>
{
    public ClassesProfile() : base()
    {
    }
}
