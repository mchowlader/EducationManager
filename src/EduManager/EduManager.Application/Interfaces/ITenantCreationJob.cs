namespace EduManager.Application.Interfaces;

public interface ITenantCreationJob
{
    Task ExecutionAsync(long tenantId, string password);
}
