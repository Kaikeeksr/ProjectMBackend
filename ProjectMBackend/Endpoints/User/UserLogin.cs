using MongoDB.Driver;

namespace ProjectMBackend.Endpoints.User
{
    public class UserLogin
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Login", async (Models.Login l, IMongoDatabase db) =>
            {

                var userCollection = db.GetCollection<Models.User>("users");
                var user = userCollection.Find(x => x.Username == l.Username).FirstOrDefault();

                if (user == null || !VerifyPass(l.Password, user))
                {
                    return Results.BadRequest(
                    new
                    {
                        msg = $"Username ou senha inválidos",
                        status = "NOT_OK"
                    });
                }

                return Results.Ok($"Usuário {user.Username} foi encontrado");
                //gerar token JWT
            });
        }

        private static bool VerifyPass(string pass, Models.User user)
        {
            return BCrypt.Net.BCrypt.Verify(pass, user.Password);
        }
    }
}
