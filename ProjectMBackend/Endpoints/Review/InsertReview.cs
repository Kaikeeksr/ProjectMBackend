using Microsoft.AspNetCore.Mvc;

namespace ProjectMBackend.Endpoints.Review
{
    public class InsertReview
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/Reviews/Insert", async (Models.Review r) =>
            {
                return await Models.Review.InsertReview(r);
            })
            .RequireAuthorization()
            .Produces<Models.Review.InsertReviewResponse>(StatusCodes.Status200OK)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
        }
    }
}
