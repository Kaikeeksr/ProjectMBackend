using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using ProjectMBackend.AuthModel;
using System.Text;
using System.Text.Json;

namespace ProjectMBackend.Configurations
{
    public static class AuthSetup
    {
        public static WebApplicationBuilder ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")!;

            builder.Services.AddScoped<Auth>();
            builder.Services.AddSingleton(jwtKey);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => ConfigureJwtBearer(options, jwtKey));

            return builder;
        }

        private static void ConfigureJwtBearer(JwtBearerOptions options, string jwtKey)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "https://localhost:3000",
                ValidAudience = "client-test",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context => HandleMissingToken(context),
                OnAuthenticationFailed = context => HandleInvalidToken(context),
                OnForbidden = context => HandleForbidden(context)
            };
        }

        private static Task HandleMissingToken(MessageReceivedContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            var isAuthRequired = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() != null;

            if (isAuthRequired && string.IsNullOrEmpty(context.Token))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var errorResponse = JsonSerializer.Serialize(new
                {
                    status = 401,
                    message = "Token de autenticação não fornecido",
                    details = "Bearer token é obrigatório no header Authorization"
                });

                return context.Response.WriteAsync(errorResponse);
            }

            return Task.CompletedTask;
        }

        private static Task HandleInvalidToken(AuthenticationFailedContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            string errorMessage = context.Exception switch
            {
                SecurityTokenExpiredException => "Token expirado",
                SecurityTokenInvalidSignatureException => "Assinatura inválida",
                SecurityTokenValidationException => "Token malformado",
                _ => "Erro de autenticação"
            };

            var errorResponse = JsonSerializer.Serialize(new
            {
                status = 401,
                message = "Falha na autenticação",
                details = errorMessage,
                stackTrace = context.HttpContext.RequestServices.GetService<IWebHostEnvironment>()!.IsDevelopment()
                    ? context.Exception?.ToString()
                    : null
            });

            return context.Response.WriteAsync(errorResponse);
        }

        private static Task HandleForbidden(ForbiddenContext context)
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";

            var errorResponse = JsonSerializer.Serialize(new
            {
                status = 403,
                message = "Acesso negado",
                details = "Você não tem permissão para acessar este recurso"
            });

            return context.Response.WriteAsync(errorResponse);
        }
    }
}