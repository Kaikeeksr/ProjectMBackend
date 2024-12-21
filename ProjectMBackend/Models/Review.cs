using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectMBackend.Models
{
    public class Review
    {
        [BsonId] // Atributo para mapear o campo _id do MongoDB para a propriedade Id
        public ObjectId Id { get; set; }
        public required string UserId { get; set; }
        public required string MovieApiId { get; set; }
        [BsonElement("Rating")] // Mapeia explicitamente o campo "Rating" do MongoDB para a propriedade Rating
        public required double Rating { get; set; }
        public required string MovieName { get; set; }
        public string? MovieCoverUrl { get; set; }
        public required string MovieDirector { get; set; }
        public required string MovieReleaseYear { get; set; }
    }
}
