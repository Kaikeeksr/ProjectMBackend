using DotNetEnv;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace ProjectMBackend.Configurations
{
    public static class AppSetup
    {
        public static WebApplicationBuilder ConfigureAppSettings(this WebApplicationBuilder builder)
        {
            LoadEnvironmentVariables();
            ConfigureBasicServices(builder);
            return builder;
        }

        private static void LoadEnvironmentVariables()
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
                    throw new InvalidOperationException($"Variável de ambiente {varName} não encontrada");
                }
            }
        }

        public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
        {
            var localhost = Environment.GetEnvironmentVariable("LOCALHOST")!;
            var androidHost = Environment.GetEnvironmentVariable("ANDROID_HOST")!;

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins(localhost, androidHost)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return builder;
        }

        public static WebApplicationBuilder ConfigureCompression(this WebApplicationBuilder builder)
        {
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json", "text/json" });
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });

            return builder;
        }

        private static void ConfigureBasicServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}