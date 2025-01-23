namespace ProjectMBackend.Configurations
{
    public static class MiddlewareSetup
    {
        public static WebApplication ConfigureMiddleware(this WebApplication app)
        {
            // Middleware de tratamento global de erros
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
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

            // Middlewares padrão
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

            return app;
        }
    }
}