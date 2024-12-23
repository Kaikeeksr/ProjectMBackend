using System.ComponentModel.DataAnnotations;
using FluentValidation;
using MongoDB.Driver;
using ProjectMBackend.Models;

namespace ProjectMBackend.Endpoints.User
{
    public class InsertUser
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Insert", async (Models.User u, IMongoDatabase db, IValidator<Models.User> validator) =>
            {
                var validationResult = await validator.ValidateAsync(u);
                if (!validationResult.IsValid)
                    return Results.BadRequest(validationResult.Errors);

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
                    u.IsActive = true;

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