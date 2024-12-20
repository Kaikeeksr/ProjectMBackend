using MongoDB.Driver;
using ProjectMBackend.Models;

namespace ProjectMBackend.Endpoints.User
{
    public class InsertUser
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Insert", async (Models.User u, IMongoDatabase db) =>
            {
                try
                {
                    var userCollection = db.GetCollection<Models.User>("users");
                    await userCollection.InsertOneAsync(u);

                    return Results.Created($"/User/{u.Id}", u);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro ao cadastar o usuário.", error = ex.Message });
                }
            });
        }
    }
}
