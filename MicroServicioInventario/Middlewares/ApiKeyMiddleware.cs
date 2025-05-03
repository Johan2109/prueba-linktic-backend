// Middleware personalizado para validar una API Key en las solicitudes HTTP
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next; // Delegado para la siguiente operación en el pipeline
    private const string APIKEY_HEADER_NAME = "X-API-KEY"; // Nombre del encabezado esperado
    private readonly string _apiKey; // Valor de la API Key configurada en appsettings

    // Constructor que recibe el siguiente middleware y la configuración del sistema
    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        // Obtiene la API Key desde la configuración (appsettings.json o variables de entorno)
        _apiKey = configuration.GetValue<string>("ApiKeySettings:Key")!;
    }

    // Método que intercepta cada solicitud HTTP entrante
    public async Task InvokeAsync(HttpContext context)
    {
        // Verifica si el encabezado X-API-KEY está presente en la solicitud
        if (!context.Request.Headers.TryGetValue(APIKEY_HEADER_NAME, out var extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("API Key missing");
            return;
        }

        // Compara la API Key proporcionada con la esperada
        if (!_apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 403; // Forbidden
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }

        // Si la clave es válida, continúa al siguiente middleware
        await _next(context);
    }
}
