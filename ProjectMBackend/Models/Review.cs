using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectMBackend.Models
{
    public class Review
    {
        [BsonId] // Atributo para mapear o campo _id do MongoDB para a propriedade Id
        public ObjectId Id { get; set; }
        public int ReviewId { get; set; }
        [BsonElement("Rating")] // Mapeia explicitamente o campo "Rating" do MongoDB para a propriedade Rating
        public double Rating { get; set; }  
        public string MovieName { get; set; }
        public int UserId { get; set; }
    }
}
