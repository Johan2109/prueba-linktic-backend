using System.Net;
using System.Text.Json;

namespace MicroServicioInventario.Middlewares
{
    // Middleware para el manejo global de errores no controlados en la aplicación
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next; // Representa el siguiente middleware en el pipeline
        private readonly ILogger<ErrorHandlingMiddleware> _logger; // Logger para registrar errores

        // Constructor que inyecta el middleware siguiente y el logger
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Método principal que intercepta cada solicitud HTTP
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Intenta ejecutar el siguiente middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                // Si ocurre una excepción, se registra y se maneja con una respuesta estándar
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        // Método que genera una respuesta con formato JSON:API ante un error inesperado
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/vnd.api+json"; // Cabecera estándar JSON:API
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // Código 500

            // Estructura de error compatible con el estándar JSON:API
            var errorResponse = new
            {
                errors = new[]
                {
                    new
                    {
                        status = "500",
                        title = "Internal Server Error",
                        detail = ex.Message // Mensaje de la excepción
                    }
                }
            };

            // Serializa la respuesta usando estilo camelCase
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
        }
    }
}
