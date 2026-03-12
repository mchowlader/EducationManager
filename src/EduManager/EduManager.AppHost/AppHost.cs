var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EduManager_Api>("edumanager-api")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development"); 

builder.Build().Run();
