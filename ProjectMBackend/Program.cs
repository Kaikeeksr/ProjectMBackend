var builder = WebApplication.CreateBuilder(args);

Setup.ConfigureServices(builder);

var app = builder.Build();

Setup.InitializeCollections(app);
Setup.ConfigureEndpoints(app);
Setup.ConfigureMiddleware(app);

app.Run();
