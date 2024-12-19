using MongoDB.Driver;
using ProjectMBackend.Models;

namespace ProjectMBackend.Endpoints
{
    public class InsertReview
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/Reviews/Insert", async (Review review, IMongoDatabase db ) =>
            {
                try
                {
                    var reviewsCollection = db.GetCollection<Review>("Reviews");

                    await reviewsCollection.InsertOneAsync(review);

                    return Results.Created($"/Reviews/{review.Id}", review);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro ao inserir o review.", error = ex.Message });
                }
            });
        }
    }
}
