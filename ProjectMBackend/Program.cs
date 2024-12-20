using MongoDB.Driver;
using ProjectMBackend.Endpoints.Review;
using ProjectMBackend.Endpoints.User;
using ProjectMBackend.Models;

var builder = WebApplication.CreateBuilder(args);

Setup.ConfigureAppSettings(builder);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inicializa a coleção na classe User
using (var scope = app.Services.CreateScope())
{
    var mongoCollection = scope.ServiceProvider.GetRequiredService<IMongoCollection<User>>();
    User.Initialize(mongoCollection);
}

GetAllReviews.Map(app);
InsertReview.Map(app);
InsertUser.Map(app);
UserLogin.Map(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
