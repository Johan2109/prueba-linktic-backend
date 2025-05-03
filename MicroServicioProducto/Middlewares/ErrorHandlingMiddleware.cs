// Espacios de nombres requeridos
using System.Net;
using System.Text.Json;

namespace MicroServicioProducto.Middlewares
{
    // Middleware para manejo global de errores no controlados
    public class ErrorHandlingMiddleware
    {
        // Delegado al siguiente middleware en el pipeline
        private readonly RequestDelegate _next;

        // Logger para registrar excepciones
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        // Constructor que recibe el siguiente middleware y el logger
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Método principal que se ejecuta por cada solicitud entrante
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continúa con el siguiente middleware en la cadena
                await _next(context);
            }
            catch (Exception ex)
            {
                // Si ocurre una excepción, se registra en el log
                _logger.LogError(ex, "Unhandled exception occurred.");

                // Se maneja la excepción devolviendo una respuesta formateada
                await HandleExceptionAsync(context, ex);
            }
        }

        // Método auxiliar para construir y retornar una respuesta de error
        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // Establece tipo de contenido según especificación JSON:API
            context.Response.ContentType = "application/vnd.api+json";

            // Código HTTP 500 para error interno
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Estructura del mensaje de error según JSON:API
            var errorResponse = new
            {
                errors = new[] {
                    new {
                        status = "500",
                        title = "Internal Server Error",
                        detail = ex.Message // Mensaje técnico del error
                    }
                }
            };

            // Configura el serializador para usar nombres camelCase
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            // Serializa la respuesta y la escribe en el cuerpo de la respuesta HTTP
            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
        }
    }
}
