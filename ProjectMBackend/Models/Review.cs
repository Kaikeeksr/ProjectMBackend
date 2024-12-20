using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectMBackend.Models
{
    public class Review
    {
        [BsonId] // Atributo para mapear o campo _id do MongoDB para a propriedade Id
        public required ObjectId Id { get; set; }
        public required int ReviewId { get; set; }
        [BsonElement("Rating")] // Mapeia explicitamente o campo "Rating" do MongoDB para a propriedade Rating
        public required double Rating { get; set; }
        public required string MovieName { get; set; }
        public required int UserId { get; set; }
    }
}
