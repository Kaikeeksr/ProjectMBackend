using Microsoft.Extensions.Configuration.UserSecrets;
using MongoDB.Driver;
using Sprache;
using System.ComponentModel.DataAnnotations;

namespace ProjectMBackend.Endpoints.Review
{
    public class GetAllReviews
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/Reviews/FindAll/{userId?}", async (string? userId) =>
            {
                return await Models.Review.FindAll(userId);          
            })
            .RequireAuthorization();
        }
    }
}
