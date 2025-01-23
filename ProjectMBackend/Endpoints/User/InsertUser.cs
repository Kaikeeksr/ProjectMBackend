using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using UserModel = ProjectMBackend.Models.User; // Alias para evitar conflito

namespace ProjectMBackend.Endpoints.User
{
    public class InsertUser
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Insert", async (UserModel u, IMongoDatabase db) =>
            {
                return await UserModel.SignUp(u, db);
            })
            .Produces<UserModel.SignUpResponse>(StatusCodes.Status200OK)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
        }
    }
}