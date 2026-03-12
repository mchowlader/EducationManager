using EduManager.Domain.Common;

namespace EduManager.Domain.Entities.Master;

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public Classes Classes { get; set; } = null!;
    public int SectionId { get; set; }
    public Section Section { get; set; } = null!;
}
