using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ProjectMBackend.Models
{
    public class User
    {
        private static IMongoCollection<User>? _collection;

        [BsonId]
        public ObjectId Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public static void Initialize(IMongoCollection<User> collection)
        {
            _collection = collection;
        }

        public bool Exists()
        {
            if (_collection == null)
                throw new InvalidOperationException("Collection not initialized");

            return _collection.Find(x => x.Username == Username).Any();
        }

    }
}
