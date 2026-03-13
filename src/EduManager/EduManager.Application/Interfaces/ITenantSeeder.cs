namespace EduManager.Application.Interfaces;

public interface ITenantSeeder
{
    Task SeedAsync(string connectionString);
}
