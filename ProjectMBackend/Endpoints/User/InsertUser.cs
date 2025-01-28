using Microsoft.AspNetCore.Mvc;
using UserModel = ProjectMBackend.Models.User; // Alias para evitar conflito

namespace ProjectMBackend.Endpoints.User
{
    public class InsertUser
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Insert", async (UserModel u) =>
            {
                return await UserModel.SignUp(u);
            })
            .Produces<UserModel.SignUpResponse>(StatusCodes.Status200OK)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
        }
    }
}