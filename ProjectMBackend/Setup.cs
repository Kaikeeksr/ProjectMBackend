using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using ProjectMBackend.AuthModel;
using ProjectMBackend.Endpoints.Review;
using ProjectMBackend.Endpoints.User;
using ProjectMBackend.Models;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

public static class Setup
{
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        ConfigureEnvironment();
        ConfigureMongoDb(builder);
        ConfigureAuthentication(builder);
        ConfigureCompression(builder);
        ConfigureCors(builder);
        ConfigureBasicServices(builder);
    }
    private static void ConfigureEnvironment()
    {
        Env.Load();
        ValidateEnvironmentVariables();
    }

    private static void ValidateEnvironmentVariables()
    {
        var requiredVars = new[] { "DB_USER", "DB_PASS", "JWT_KEY", "LOCALHOST" };
        foreach (var varName in requiredVars)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(varName)))
            {
                throw new InvalidOperationException($"A variável {varName} não foi encontrado no arquivo de ambiente.");
            }
        }
    }

    private static void ConfigureCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            var localhost = Environment.GetEnvironmentVariable("LOCALHOST");

            if (string.IsNullOrEmpty(localhost))
            {
                throw new ArgumentNullException(nameof(localhost), "Environment variable 'LOCAHOST' is not set.");
            }

            options.AddPolicy("AllowSpecificOrigin", builder =>
            {
                builder.WithOrigins(localhost)
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });
    }

    private static void ConfigureCompression(WebApplicationBuilder builder)
    {
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true; // Habilita compressão mesmo para HTTPS
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] {
                    "application/json",
                    "application/xml",
                    "text/plain",
                    "text/json"
                });
        });

        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest; // Balanceamento entre CPU e compressão
        });
    }

    private static void ConfigureMongoDb(WebApplicationBuilder builder)
    {
        string connectionString = GetMongoConnectionString();

        builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

        builder.Services.AddScoped<IMongoDatabase>(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase("projectM"));

        builder.Services.AddScoped<IMongoCollection<User>>(sp =>
            sp.GetRequiredService<IMongoDatabase>().GetCollection<User>("users"));
    }

    private static string GetMongoConnectionString()
    {
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
        return $"mongodb+srv://{dbUser}:{dbPass}@db-projectm.dqdjc.mongodb.net/?retryWrites=true&w=majority&appName=db-projectM&ssl=true&tls=true";
    }

    public static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        
        if (string.IsNullOrEmpty(jwtKey))
            throw new InvalidOperationException("A variável JWT_KEY não foi encontrada no arquivo de ambiente.");
        
        builder.Services.AddSingleton(jwtKey);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "https://localhost:3000",
                    ValidAudience = "client-test",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
                };

                options.Events = new JwtBearerEvents
                {
                    // Se não foi informado um token em uma rota que necessita do token
                    OnMessageReceived = context =>
                    {
                        var endpoint = context.HttpContext.GetEndpoint();

                        if (endpoint == null) return Task.CompletedTask;

                        var isTokenRequired = endpoint.Metadata
                            .GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null;

                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                        if (isTokenRequired && String.IsNullOrEmpty(token))
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            var result = JsonSerializer.Serialize(new
                            {
                                status = 401,
                                message = "Token de autenticação não fornecido",
                                details = "É necessário enviar um Bearer token no header Authorization para acessar essa rota"
                            });
                            context.Response.WriteAsync(result);
                        }

                        return Task.CompletedTask;
                    },

                    // Token inválido (expirado, assinatura incorreta, etc)
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        var result = JsonSerializer.Serialize(new
                        {
                            status = 401,
                            message = "Token de autenticação inválido",
                            details = context.Exception switch
                            {
                                SecurityTokenExpiredException _ => "Token expirado",
                                SecurityTokenSignatureKeyNotFoundException _ => "Token com assinatura inválida",
                                _ => "Token inválido ou mal formatado"
                            }
                        });

                        return context.Response.WriteAsync(result);
                    },

                    // Token válido mas sem permissões necessárias
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new
                        {
                            status = 403,
                            message = "Acesso negado",
                            details = "Você não tem permissão para acessar este recurso"
                        });
                        return context.Response.WriteAsync(result);
                    }
                };
            });
    }

    private static void ConfigureBasicServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<Auth>();
    }

    public static void InitializeCollections(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var mongoCollection = scope.ServiceProvider.GetRequiredService<IMongoCollection<User>>();
        User.Initialize(mongoCollection);
    }

    public static void ConfigureEndpoints(WebApplication app)
    {
        GetAllReviews.Map(app);
        InsertReview.Map(app);
        InsertUser.Map(app);
        UserLogin.Map(app);
    }

    public static void ConfigureMiddleware(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                // Opcionalmente, você pode retornar uma resposta mais amigável
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Erro interno do servidor",
                    message = ex.Message,
                    // Em produção, você pode querer omitir o stackTrace por segurança
                    stackTrace = app.Environment.IsDevelopment() ? ex.StackTrace : null
                });
            }

        });

        // Adiciona UseResponseCompression antes de qualquer middleware que gera resposta
        app.UseResponseCompression();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Aplica a política de CORS
        app.UseCors("AllowSpecificOrigin");

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}