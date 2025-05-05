Pruebas Unitarias - Proyecto Microservicios Linktic

Este proyecto contiene pruebas unitarias desarrolladas para validar la lógica de negocio de dos microservicios:

MicroServicioProducto

MicroServicioInventario

Ambos microservicios están construidos con .NET 8 y MongoDB como base de datos NoSQL.

Estructura de Carpetas

prueba-linktic-backend/
├── MicroServicioProducto/
├── MicroServicioInventario/
├── MicroServicioProducto.Tests/         ← Pruebas del servicio de productos
└── MicroServicioInventario.Tests/       ← Pruebas del servicio de inventario

Tecnologías utilizadas

.NET 8 SDK

xUnit

MongoDB.Driver

Microsoft.NET.Test.Sdk

MongoDB (corriendo en localhost:27017)

Swashbuckle y OpenAPI (opcional)

Docker y Docker Compose

Requisitos Previos

Tener Docker y Docker Compose instalados.

Tener MongoDB local o levantarlo mediante Docker.

(Opcional) Instalar .NET 8 SDK si se desea correr sin contenedor.

Cómo ejecutar las pruebas

1. Clonar el proyecto

git clone https://github.com/Johan2109/prueba-linktic-backend.git
cd prueba-linktic-backend

2. Restaurar paquetes

dotnet restore

3. Ejecutar pruebas de productos

cd MicroServicioProducto.Tests
dotnet test

4. Ejecutar pruebas de inventario

cd MicroServicioInventario.Tests
dotnet test

🐳 Cómo ejecutar el proyecto local con Docker

1. Asegúrate de tener Docker Desktop corriendo

2. En la raíz del proyecto, ejecuta:

docker-compose up --build

Esto levantará:

MicroServicioProducto

MicroServicioInventario

MongoDB

3. Acceder a Swagger (si está habilitado):

Producto: http://localhost:5154/ <- estos puertos son los que se me generaron localmente

Inventario: http://localhost:5208/ <- estos puertos son los que se me generaron localmente

4. Detener los servicios:

docker-compose down

Contenido de las pruebas

MicroServicioProducto.Tests

Ubicación: MicroServicioProducto.Tests/UnitTests1.cs

Se validan:

Crear un producto

Obtener un producto por ID

Actualizar un producto existente

Eliminar un producto

Obtener productos con paginación

MicroServicioInventario.Tests

Ubicación: MicroServicioInventario.Tests/TestsInventory.cs

Se validan:

Crear un inventario

Obtener inventario por ID de producto

Actualizar stock de un inventario

Eliminar inventario por ID de producto

Configuración usada (appsettings.json)

{
  "MongoDBSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "PruebaLinkticDB",
    "ProductsCollectionName": "Products",
    "InventoryCollectionName": "Inventory"
  }
}

Este archivo se copia automáticamente al directorio de salida gracias a la configuración del archivo .csproj.

Notas adicionales

Todas las pruebas limpian sus datos después de ejecutarse.

Las pruebas utilizan ObjectId generado al vuelo para evitar colisiones.

Se usa Options.Create(...) para simular la inyección de configuración real.
