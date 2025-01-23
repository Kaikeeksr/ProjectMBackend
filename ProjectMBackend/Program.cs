using ProjectMBackend.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()
       .ConfigureAppSettings()    
       .ConfigureCors()           
       .ConfigureCompression()    
       .ConfigureAuthentication() 
       .ConfigureMongoDb();       

var app = builder.Build();

app.MapDefaultEndpoints()
   .InitializeDatabase()          
   .ConfigureMiddleware()         
   .ConfigureEndpoints();         

app.Run();