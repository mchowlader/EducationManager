using EduManager.Domain.Common;

namespace EduManager.Domain.Entities;

public class Student : BaseEntity
{
    public string StudentCode { get; set; } = string.Empty;
    public int ClassRoll { get; set; }
    public bool IsActive { get; set; } = true;

    // FK
    public long SectionId { get; set; }
    public Section Section { get; set; } = null!;
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}
