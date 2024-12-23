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
}
