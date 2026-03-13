namespace EduManager.Application.Interfaces;

public interface ITenantSeeder
{
    Task SeedAsync(string tenantDbconnectionString, string email = "", string password = "");
}
