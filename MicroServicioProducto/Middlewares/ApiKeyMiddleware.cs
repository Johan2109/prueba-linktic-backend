// Middleware para validar una API Key en cada solicitud entrante
public class ApiKeyMiddleware
{
    // Delegado que representa la siguiente etapa en el pipeline de middleware
    private readonly RequestDelegate _next;

    // Nombre del encabezado esperado para la API Key
    private const string APIKEY_HEADER_NAME = "X-API-KEY";

    // Valor de la API Key configurada en appsettings.json
    private readonly string _apiKey;

    // Constructor que inyecta la configuración y almacena la API Key
    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration.GetValue<string>("ApiKeySettings:Key")!; // Lee el valor de configuración
    }

    // Método principal que intercepta la solicitud HTTP
    public async Task InvokeAsync(HttpContext context)
    {
        // Verifica si el encabezado de la API Key está presente
        if (!context.Request.Headers.TryGetValue(APIKEY_HEADER_NAME, out var extractedApiKey))
        {
            // Si falta, devuelve 401 (No autorizado)
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key missing");
            return;
        }

        // Compara la clave proporcionada con la esperada
        if (!_apiKey.Equals(extractedApiKey))
        {
            // Si no coincide, devuelve 403 (Prohibido)
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Invalid API Key");
            return;
        }

        // Si es válida, continúa al siguiente middleware o controlador
        await _next(context);
    }
}
