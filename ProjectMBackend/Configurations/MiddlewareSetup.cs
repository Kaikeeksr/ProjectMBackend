namespace ProjectMBackend.Configurations
{
    public static class MiddlewareSetup
    {
        public static WebApplication ConfigureMiddleware(this WebApplication app)
        {
            app.UseResponseCompression();

            app.UseResponseCompression();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowSpecificOrigin");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Middleware de tratamento global de erros
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    // Se o response já iniciou (?) -> não modificar headers
                    if (context.Response.HasStarted)
                    {
                        Console.WriteLine("Resposta já iniciada. Abortando tratamento de erro.");
                        throw;
                    }

                    // Log do erro
                    Console.WriteLine($"Erro: {ex.Message}");

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Erro interno do servidor",
                        message = ex.Message,
                        stackTrace = app.Environment.IsDevelopment() ? ex.StackTrace : null
                    });
                }
            });

            return app;
        }
    }
}