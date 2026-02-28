namespace EduManager.Domain.Common;

public class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public long CreateBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long UpdateBy { get; set; }
    public bool IsDelete { get; set; }
}
