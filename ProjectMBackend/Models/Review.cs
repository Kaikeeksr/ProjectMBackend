﻿using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

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
