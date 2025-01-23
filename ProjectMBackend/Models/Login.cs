using MongoDB.Driver;
using ProjectMBackend.AuthModel;
using ProjectMBackend.Models;

namespace ProjectMBackend.Models
{
    public class Login
    {
        public required string Username { get; set; }
        public required string Password { get; set; }

        public record LoginResponse(string Message, string Status, string? Token);

        public static async Task<IResult> SignIn(Login login, IMongoDatabase db, Auth auth)
        {
            var userCollection = db.GetCollection<User>("users");
            var user = await userCollection.Find(x => x.Username == login.Username)
                                         .FirstOrDefaultAsync();

            if (user == null || !VerifyPassword(login.Password, user.Password))
            {
                return TypedResults.BadRequest(
                    new LoginResponse("Credenciais inválidas", "NOT_OK", null)
                );
            }

            var token = auth.GenerateJwt(user);
            return TypedResults.Ok(
                new LoginResponse("Login efetuado com sucesso", "OK", token)
            );
        }

        private static bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
        }
    }
}