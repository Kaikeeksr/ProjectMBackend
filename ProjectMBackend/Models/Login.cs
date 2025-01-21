using MongoDB.Driver;
using ProjectMBackend.AuthModel;
using ProjectMBackend.Models;

public class Login
{
    public required string Username { get; set; }
    public required string Password { get; set; }

    public class LoginResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public string? Token { get; set; }
    }

    public async Task<IResult> SignIn(IMongoDatabase db, Auth auth)
    {
        var userCollection = db.GetCollection<User>("users");
        var user = await userCollection.Find(x => x.Username == Username)
                                     .FirstOrDefaultAsync();

        if (user == null || !VerifyPassword(Password, user))
        {
            return Results.BadRequest(new LoginResponse
            {
                Message = "Username ou senha inválidos",
                Status = "NOT_OK",
                Token = null
            });
        }

        var token = auth.GenerateJwt(user);

        return Results.Ok(new LoginResponse
        {
            Message = "Login efetuado com sucesso",
            Status = "OK",
            Token = token
        });
    }

    private static bool VerifyPassword(string password, User user)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.Password);
    }
}