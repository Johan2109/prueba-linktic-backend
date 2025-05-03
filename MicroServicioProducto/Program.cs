using MicroServicioProducto.Models;
using MicroServicioProducto.Services;
using MicroServicioProducto.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ──────────────── CONFIGURACIÓN DE SERVICIOS ────────────────

// Cargar la configuración de MongoDB desde appsettings.json
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));

// Inyectar el servicio de productos como singleton
builder.Services.AddSingleton<ProductService>();

// Agregar soporte para controladores
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Requerido para Swagger

// Configuración de CORS para permitir acceso desde el frontend (por ejemplo Vue)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:8080") // Dirección del frontend permitido
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ──────────────── CONFIGURACIÓN DE SWAGGER ────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MicroServicioProducto",
        Version = "v1"
    });

    // Definición de la API Key para documentación Swagger
    c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "X-API-KEY",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Description = "Clave de API para autenticación. Ejemplo: mi-api-key-supersecreta"
    });

    // Aplicación de la API Key a todas las operaciones
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Autorización (aunque en este caso aún no hay autenticación real configurada)
builder.Services.AddAuthorization();

// Forzar que la app escuche en el puerto 80 dentro del contenedor
builder.WebHost.UseUrls("http://0.0.0.0:80");

// ──────────────── CONSTRUCCIÓN DE LA APP ────────────────
var app = builder.Build();

// Aplicar la política de CORS
app.UseCors("AllowFrontend");

// Activar Swagger sólo en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroServicioProducto v1");
        c.RoutePrefix = string.Empty; // Mostrar Swagger en la raíz
    });
}

// Middleware para manejo de errores global
app.UseMiddleware<ErrorHandlingMiddleware>();

// Middleware para validar la API Key en cada solicitud
app.UseMiddleware<ApiKeyMiddleware>();

// Configurar middleware de autorización (aunque aún no se usan roles ni claims)
app.UseAuthorization();

// Mapear endpoints de controladores
app.MapControllers();

// Iniciar la aplicación
app.Run();
