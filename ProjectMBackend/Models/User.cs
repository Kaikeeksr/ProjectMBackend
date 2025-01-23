using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using ProjectMBackend.Configurations;

namespace ProjectMBackend.Models
{
    public class User
    {
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

        public record SignUpResponse(string Message, string Status, User? User);

        public static async Task<IResult> SignUp(User u, IMongoDatabase db)
        {
            var v = new UserValidator();
            var validationResult = await v.ValidateAsync(u);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult.Errors);

            if (await u.Exists())
            {
                return TypedResults.BadRequest(
                    new SignUpResponse("Esse usuário já existe", "NOT_OK", null)
                );
            }

            try
            {
                u.Password = BCrypt.Net.BCrypt.HashPassword(u.Password);
                u.CreatedAt = DateTime.Now;
                u.IsActive = true;

                var userCollection = db.GetCollection<Models.User>("users");
                await userCollection.InsertOneAsync(u);

                return TypedResults.Ok(
                    new SignUpResponse("Usuário cadastrado com sucesso", "OK", u)
                );
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { message = "Erro ao cadastar o usuário.", error = ex.Message });
            }
        }

        public async Task<bool> Exists()
        {
            return await DatabaseSetup.dbContext.Users
                .Find(u => u.Username == Username)
                .AnyAsync();
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
