using FluentValidation;
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

    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(u => u.FirstName)
               .NotEmpty()
               .Length(2, 50)
               .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Nome deve conter apenas letras e acentos");

            RuleFor(u => u.LastName)
               .NotEmpty()
               .Length(2, 50)
               .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("Sobrenome deve conter apenas letras e acentos");

            RuleFor(u => u.Username)
                .NotEmpty()
                .Length(3, 20)
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Nome de usuário deve contar apenas letras, números e underscore");

            RuleFor(u => u.Password)
                .NotEmpty()
                .MinimumLength(5)
                .Matches("[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
                .Matches("[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula")
                .Matches("[0-9]").WithMessage("Senha deve conter pelo menos um número")
                .Matches("[^a-zA-Z0-9]").WithMessage("Senha deve conter pelo menos um caracter esepcial");

            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);
        }
    }
}
