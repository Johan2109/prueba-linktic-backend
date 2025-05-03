using MicroServicioInventario.Services;
using MicroServicioInventario.Settings;
using MicroServicioInventario.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------
// 🔧 Configuración de Servicios
// ---------------------------

// 🟦 Configuración de CORS para permitir solicitudes desde el frontend (Vue 3 por defecto en localhost:8080)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:8080")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 📦 Inyecta los valores del archivo appsettings.json en la clase MongoDBSettings
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));

// 📌 Inyección del servicio de Inventario como Singleton
builder.Services.AddSingleton<InventoryService>();

// 📚 Agrega soporte para controladores
builder.Services.AddControllers();

// 🧪 Habilita el sistema de documentación y exploración de endpoints
builder.Services.AddEndpointsApiExplorer();

// 📘 Configura Swagger (OpenAPI) para documentación interactiva
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MicroServicioInventario",
        Version = "v1"
    });

    // 🔐 Define esquema de autenticación por API Key para Swagger
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "Clave de API para autenticación. Ejemplo: mi-api-key-supersecreta"
    });

    // 🔒 Aplica el requisito global de API Key a todos los endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// 🌐 Configura el microservicio para escuchar por el puerto 80 dentro del contenedor
builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

// ---------------------------
// 🚀 Middleware y ejecución
// ---------------------------

// 🌐 Habilita CORS para el frontend configurado
app.UseCors("AllowFrontend");

// 📖 Habilita Swagger para pruebas e inspección de la API desde navegador
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MicroServicioInventario v1");
    c.RoutePrefix = string.Empty; // Swagger UI disponible en la raíz del sitio
});

// 🧱 Middleware personalizado para manejar errores no controlados
app.UseMiddleware<ErrorHandlingMiddleware>();

// 🔐 Middleware que valida la API Key en todas las peticiones entrantes
app.UseMiddleware<ApiKeyMiddleware>();

// 🔁 Habilita el sistema de ruteo de controladores
app.UseRouting();

// 🔐 Habilita autorización (aunque no haya políticas activas por defecto)
app.UseAuthorization();

// 📍 Mapea los endpoints de los controladores
app.MapControllers();

// ▶️ Ejecuta la aplicación
app.Run();
