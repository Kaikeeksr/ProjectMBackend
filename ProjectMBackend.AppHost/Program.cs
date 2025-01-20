var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ProjectMBackend>("projectmbackend");

builder.Build().Run();
