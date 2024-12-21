using MongoDB.Driver;

namespace ProjectMBackend.Endpoints.User
{
    public class InsertUser
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Insert", static async (Models.User u, IMongoDatabase db) =>
            {
                if (u.Exists())
                {
                    return Results.BadRequest(
                    new
                    {
                        message = $"O usuário {u.Username} já existe. Tente outro username",
                        status = "NOT_OK"
                    });
                }

                try
                {
                    u.Password = BCrypt.Net.BCrypt.HashPassword(u.Password);
                    u.CreatedAt = DateTime.Now;

                    var userCollection = db.GetCollection<Models.User>("users");
                    await userCollection.InsertOneAsync(u);

                    return Results.Created(
                        $"/User/{u.Id}",
                        new
                        {
                            message = "Usuário criado com sucesso",
                            userId = u.Id.ToString(),
                            user = u
                        }
                    );
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro ao cadastar o usuário.", error = ex.Message });
                }
            });
        }
    }
}