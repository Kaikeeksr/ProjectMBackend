using MongoDB.Driver;
using ProjectMBackend.Models;

namespace ProjectMBackend.Configurations
{
    public static class DatabaseSetup
    {
        // Classe estática para acesso global -> inicialização do db nas classes
        public static class dbContext 
        {
            public static IMongoDatabase Database { get; internal set; } = null!;
            public static IMongoCollection<User> Users { get; internal set; } = null!;
            public static IMongoCollection<Review> Reviews { get; internal set; } = null!;
        }

        private static string GetMongoConnectionString()
        {
            var dbUser = Environment.GetEnvironmentVariable("DB_USER");
            var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
            return $"mongodb+srv://{dbUser}:{dbPass}@db-projectm.dqdjc.mongodb.net/?retryWrites=true&w=majority&appName=db-projectM&ssl=true&tls=true";
        }

        public static WebApplicationBuilder ConfigureMongoDb(this WebApplicationBuilder builder)
        {
            var connectionString = GetMongoConnectionString();

            builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
            builder.Services.AddSingleton<IMongoDatabase>(sp =>
                sp.GetRequiredService<IMongoClient>().GetDatabase("projectM"));

            RegisterCollections(builder);
            return builder;
        }

        private static void RegisterCollections(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IMongoCollection<User>>(sp =>
                sp.GetRequiredService<IMongoDatabase>().GetCollection<User>("users"));

            builder.Services.AddSingleton<IMongoCollection<Review>>(sp =>
                sp.GetRequiredService<IMongoDatabase>().GetCollection<Review>("reviews"));
        }

        public static WebApplication InitializeDatabase(this WebApplication app)
        {

            using var scope = app.Services.CreateScope();

            // Inicializa o contexto estático
            dbContext.Database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
            dbContext.Users = scope.ServiceProvider.GetRequiredService<IMongoCollection<User>>();
            dbContext.Reviews = scope.ServiceProvider.GetRequiredService<IMongoCollection<Review>>();

            CreateUserIndexes(dbContext.Database);
            CreateReviewIndexes(dbContext.Database);

            return app;
        }

        private static void CreateUserIndexes(IMongoDatabase database)
        {
            var usersCollection = database.GetCollection<User>("users");
            var usernameIndex = new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Ascending(u => u.Username),
                new CreateIndexOptions { Unique = true }
            );
            usersCollection.Indexes.CreateOne(usernameIndex);
        }

        private static void CreateReviewIndexes(IMongoDatabase database)
        {
            var reviewsCollection = database.GetCollection<Review>("reviews");

            var reviewIndex = new CreateIndexModel<Review>(
                Builders<Review>.IndexKeys
                    .Ascending(r => r.MovieApiId)
                    .Descending(r => r.CreatedAt),
                new CreateIndexOptions { Name = "ProductReviews" }
            );

            reviewsCollection.Indexes.CreateOne(reviewIndex);
        }
    }
}