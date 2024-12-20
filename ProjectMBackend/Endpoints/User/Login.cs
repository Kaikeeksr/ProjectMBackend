namespace ProjectMBackend.Endpoints.User
{
    public class Login
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/User/Login", (Models))
        }
    }
}
