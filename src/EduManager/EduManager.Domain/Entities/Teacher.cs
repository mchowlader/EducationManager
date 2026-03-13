using EduManager.Domain.Common;

namespace EduManager.Domain.Entities;

public class Teacher : BaseEntity, ITenantEntity
{
    public string TeacherCode { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // FK
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Classes> Classes { get; set; } = [];
}