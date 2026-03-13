using EduManager.Domain.Common;

namespace EduManager.Domain.Entities;

public class Section : BaseEntity, ITenantEntity
{
    public string Name { get; set; } = string.Empty;
    public long ClassId { get; set; }
    public Classes Class { get; set; } = null!;
    public int Capacity { get; set; }
    public ICollection<Student> Students { get; set; } = [];
}
