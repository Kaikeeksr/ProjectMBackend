using ProjectMBackend.Endpoints.Review;
using ProjectMBackend.Endpoints.User;

namespace ProjectMBackend.Configurations
{
    public static class EndpointSetup
    {
        public static WebApplication ConfigureEndpoints(this WebApplication app)
        {
            // Mapeia todos os endpoints
            UserLogin.Map(app);
            InsertUser.Map(app);
            GetAllReviews.Map(app);
            InsertReview.Map(app);

            return app;
        }
    }
}