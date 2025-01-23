var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

Setup.ConfigureServices(builder);

var app = builder.Build();

app.MapDefaultEndpoints();

Setup.InitializeCollections(app);
Setup.ConfigureEndpoints(app);
Setup.ConfigureMiddleware(app);

app.Run();
