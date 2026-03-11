namespace EduManager.Application.Interfaces;

public interface ITenantContext
{
    long TenantId { get; }
    string? Slug { get; }
    string? ConnectionString { get; }
    bool HasTenant { get; }

}
