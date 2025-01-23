using MongoDB.Driver;
using ProjectMBackend.AuthModel;
using ProjectMBackend.Models;
using Microsoft.AspNetCore.Http.HttpResults;

public static class Login
{
    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string Message, string Status, string? Token);

    public static async Task<IResult> SignIn(LoginRequest req, IMongoDatabase db, Auth auth)
    {
        var userCollection = db.GetCollection<User>("users");
        var user = await userCollection.Find(x => x.Username == req.Username)
                                     .FirstOrDefaultAsync();

        if (user == null || !VerifyPassword(req.Password, user))
        {
            return TypedResults.BadRequest(
                new LoginResponse("Usuário ou senha incorretos", "NOT_OK", null)
            );
        }

        var token = auth.GenerateJwt(user);
        return TypedResults.Ok(
            new LoginResponse("Login efetuado com sucesso", "OK", token)
        );
    }

    private static bool VerifyPassword(string password, User user)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.Password);
    }
}