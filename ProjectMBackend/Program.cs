using ProjectMBackend.Endpoints;

var builder = WebApplication.CreateBuilder(args);

Setup.ConfigureAppSettings(builder);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

GetAllReviews.Map(app);
InsertReview.Map(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
