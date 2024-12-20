using DotNetEnv;
using MongoDB.Driver;
using ProjectMBackend.Models;

public static class Setup
{
    public static void ConfigureAppSettings(WebApplicationBuilder builder)
    {
        Env.Load();

        // Verificação das variáveis de ambiente
        string dbUser = Environment.GetEnvironmentVariable("DB_USER") 
            ?? throw new InvalidOperationException("DB_USER not found in environment variables");
        string dbPass = Environment.GetEnvironmentVariable("DB_PASS") 
            ?? throw new InvalidOperationException("DB_PASS not found in environment variables");

        // Construção da string de conexão
        string connectionString = $"mongodb+srv://{dbUser}:{dbPass}@db-projectm.dqdjc.mongodb.net/?retryWrites=true&w=majority&appName=db-projectM";

        // Registro dos serviços do MongoDB
        builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
        {
            return new MongoClient(connectionString);
        });

        builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
        {
            var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
            return mongoClient.GetDatabase("projectM");
        });

        // Registro da coleção de usuários (?) --> pesquisar prpósito/possível utilidade
        builder.Services.AddScoped<IMongoCollection<User>>(serviceProvider =>
        {
            var database = serviceProvider.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<User>("users");
        });
    }
}