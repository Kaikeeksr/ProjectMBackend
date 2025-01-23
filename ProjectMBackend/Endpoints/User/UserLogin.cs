using MongoDB.Driver;
using ProjectMBackend.AuthModel;

namespace ProjectMBackend.Endpoints.User
{
    public class UserLogin
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Login", async (Models.Login l, IMongoDatabase db, Auth auth) =>
            {
                return await Models.Login.SignIn(l, db, auth);
            })
            .Produces<Models.Login.LoginResponse>(StatusCodes.Status200OK)
            .Produces<Models.Login.LoginResponse>(StatusCodes.Status400BadRequest);
        }
    }
}