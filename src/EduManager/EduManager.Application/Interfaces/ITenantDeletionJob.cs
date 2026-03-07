namespace EduManager.Application.Interfaces;

public interface ITenantDeletionJob
{
    Task ExecuteAsync(long tenantId);
}