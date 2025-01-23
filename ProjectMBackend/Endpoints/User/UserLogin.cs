using MongoDB.Driver;
using ProjectMBackend.AuthModel;

namespace ProjectMBackend.Endpoints.User
{
    public class UserLogin
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Login", async (Login.LoginRequest req, IMongoDatabase db, Auth auth) =>
            {
                return await Login.SignIn(req, db, auth);
            })
            .Produces<Login.LoginResponse>(StatusCodes.Status200OK)
            .Produces<Login.LoginResponse>(StatusCodes.Status400BadRequest);
        }
    }
}