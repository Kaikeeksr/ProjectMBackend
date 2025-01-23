using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ProjectMBackend.Endpoints.User
{
    public class InsertUser
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Insert", async (Models.User u, IMongoDatabase db) =>
            {
                return await Models.User.SignUp(u, db);
            })
            .Produces<Models.User.SignUpResponse>(StatusCodes.Status200OK)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
        }
    }
}