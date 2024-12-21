using MongoDB.Driver;

namespace ProjectMBackend.Endpoints.Review
{
    public class InsertReview
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/Reviews/Insert", async (Models.Review r, IMongoDatabase db) =>
            {
                try
                {
                    var reviewsCollection = db.GetCollection<Models.Review>("reviews");
                    await reviewsCollection.InsertOneAsync(r);

                    return Results.Created($"/Reviews/{r.Id}", r);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { message = "Erro ao inserir o review.", error = ex.Message });
                }
            })
            .RequireAuthorization();
        }
    }
}
