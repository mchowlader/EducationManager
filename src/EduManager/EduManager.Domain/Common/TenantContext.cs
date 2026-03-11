namespace EduManager.Domain.Common;

public class TenantContext
{
    public long TenantId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
}
