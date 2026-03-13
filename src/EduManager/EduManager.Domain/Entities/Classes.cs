using EduManager.Domain.Common;

namespace EduManager.Domain.Entities;

public class Classes : BaseEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation
    public ICollection<Section> Sections { get; set; } = [];
    public ICollection<Teacher> Teachers { get; set; } = [];
}
