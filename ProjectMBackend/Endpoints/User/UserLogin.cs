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
                var userCollection = db.GetCollection<Models.User>("users");
                var user = await userCollection.Find(x => x.Username == l.Username).FirstOrDefaultAsync();

                if (user == null || !VerifyPass(l.Password, user))
                {
                    return Results.BadRequest(
                    new
                    {
                        msg = $"Username ou senha inválidos",
                        status = "NOT_OK"
                    });
                }

                var token = auth.GenerateJwt(user);

                return Results.Ok(new
                {
                    message = "Login efetuado com sucesso",
                    token = token
                });

            });
        }

        private static bool VerifyPass(string pass, Models.User user)
        {
            return BCrypt.Net.BCrypt.Verify(pass, user.Password);
        }
    }
}
