using MongoDB.Driver;
using ProjectMBackend.AuthModel;
using Login = ProjectMBackend.Models.Login;

namespace ProjectMBackend.Endpoints.User
{
    public class UserLogin
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Login", async (Login l, IMongoDatabase db, Auth auth) =>
            {
                return await Login.SignIn(l, db, auth);
            })
            .Produces<Login.LoginResponse>(StatusCodes.Status200OK)
            .Produces<Login.LoginResponse>(StatusCodes.Status400BadRequest);
        }
    }
}