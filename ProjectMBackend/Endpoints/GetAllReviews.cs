using MongoDB.Driver;
using ProjectMBackend.Models;

namespace ProjectMBackend.Endpoints
{
    public class GetAllReviews
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/Reviews/FindAll/{id}", async (int id, IMongoDatabase db) =>
            {
                var reviewsCollection = db.GetCollection<Review>("reviews");

                var reviews = await reviewsCollection
                    .Find(review => review.UserId == id)
                    .ToListAsync();

                if (reviews.Count == 0)
                {
                    return Results.NotFound(new { msg = $"Não foi encontrado nenhum review para o id: {id}" });
                }

                return Results.Ok(reviews);
               
            });
        }
    }
}
