using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

public static class Setup
{
    public static void ConfigureAppSettings(WebApplicationBuilder builder)
    {
        Env.Load();

        string dbPass = Environment.GetEnvironmentVariable("DB_PASS");
        string connectionString = $"mongodb+srv://Kaike:{dbPass}@db-projectm.dqdjc.mongodb.net/?retryWrites=true&w=majority&appName=db-projectM";

        builder.Configuration["ConnectionStrings:Mongo"] = connectionString;

        // Register MongoDB Client and Database
        builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            return new MongoClient(connectionString);
        });

        builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
        {
            var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
            return mongoClient.GetDatabase("projectM"); 
        });
    }
}
