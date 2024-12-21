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
            app.MapGet("/Reviews/FindAll", async (HttpRequest req, IMongoDatabase db) =>
            {
                var body = await req.ReadFromJsonAsync<RequestBody>();
                var userId = body?.UserId;

                if (String.IsNullOrEmpty(userId))
                    return Results.BadRequest(new { message = "id não informado" });

                var reviewsCollection = db.GetCollection<Models.Review>("reviews");

                var reviews = await reviewsCollection
                    .Find(review => review.UserId == userId)
                    .ToListAsync();

                if (reviews.Count == 0)
                {
                    return Results.NotFound(new { msg = $"Não foi encontrado nenhum review para o id: {userId}" });
                }

                return Results.Ok(reviews);
            })
            .RequireAuthorization();
        }
    }

    // Definindo a classe RequestBody
    public class RequestBody
    {
        [Required]
        public string UserId { get; set; }
    }
}
