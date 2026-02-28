var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EduManager_Api>("edumanager-api");

builder.Build().Run();
