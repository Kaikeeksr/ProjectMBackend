using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ProjectMBackend.Configurations;

namespace ProjectMBackend.Models
{
    public class Review
    {
        [BsonId]
        public ObjectId ReviewId { get; set; }
        public required string UserId { get; set; }
        [BsonElement("Rating")]
        public required double Rating { get; set; }
        public string? Coment { get; set; }
        public required string MovieApiId { get; set; }
        public required string MovieName { get; set; }
        public string? MovieCoverUrl { get; set; }
        public string? MovieDirector { get; set; }
        public required int MovieReleaseYear { get; set; }
        public string? Genre { get; set; }
        public DateTime? CreatedAt { get; set; }

        public record InsertReviewResponse(string Status, string? Message, Review? review);
        public record FindAllReviewResponse(string Status, string Message, List<Review>? reviews);

        public static async Task<IResult> InsertReview(Review r)
        {
            var v = new ReviewValidator();
            var validationResult = await v.ValidateAsync(r);

            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.Errors);

            r.CreatedAt = DateTime.Now;

            try
            {
                await DatabaseSetup.dbContext.Reviews.InsertOneAsync(r);

                return Results.Created(
                    $"/Reviews/{r.ReviewId}",
                    new InsertReviewResponse("OK", "Review cadastrado com sucesso", r)
                );
            }
            catch (Exception ex)
            {
                return Results.BadRequest(
                    new InsertReviewResponse("NOT_OK", $"Erro ao cadastrar o review. {ex.Message}", null)    
                );
            }
        }

        public static async Task<IResult> FindAll(string? userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                return Results.BadRequest(
                    new FindAllReviewResponse("NOT_OK", "Id não informado", null)
                );
            }

            try
            {
                var reviewsCollection = DatabaseSetup.dbContext.Reviews;

                var reviews = await reviewsCollection
                    .Find(review => review.UserId == userId)
                    .ToListAsync();

                if (reviews.Count == 0)
                {
                    return Results.NotFound(
                        new FindAllReviewResponse("NOT_OK", $"Não foi encontrado nenhum review para o id: {userId}", null)    
                    );
                }

                return Results.Ok(
                    new FindAllReviewResponse("OK", "Reviews encontrados com sucesso", reviews)
                );
            }
            catch (Exception ex)
            {
                return Results.BadRequest(
                   new FindAllReviewResponse("NOT_OK", $"Erro ao buscar reviews. Erro: {ex.Message}", null)    
                );
            }
        }
    }

    public class ReviewValidator : AbstractValidator<Review> 
    { 
        public ReviewValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId é obrigatório.");

            RuleFor(x => x.MovieApiId)
                .NotEmpty().WithMessage("MovieApiId é obrigatório.");

            RuleFor(x => x.MovieName)
                .NotEmpty().WithMessage("MovieName é obrigatório.");

            RuleFor(x => x.MovieReleaseYear)
                .GreaterThan(1900).WithMessage("O ano de lançamento do filme deve ser maior que 1900.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(0, 10).WithMessage("A avaliação deve ser um número entre 0 e 10.");

            RuleFor(x => x.Coment)
                .MaximumLength(150).WithMessage("O comentário não pode ter mais de 150 caracteres.");
        }
    }
}
