// Define el namespace donde se agrupa la clase
namespace MicroServicioProducto.Models
{
    // Esta clase se utiliza para mapear la configuración de MongoDB definida en appsettings.json
    public class MongoDBSettings
    {
        // Cadena de conexión a MongoDB (por ejemplo: "mongodb://localhost:27017")
        public string ConnectionString { get; set; } = string.Empty;

        // Nombre de la base de datos dentro de MongoDB que se va a utilizar
        public string DatabaseName { get; set; } = string.Empty;

        // Nombre de la colección de productos que se usará dentro de esa base de datos
        public string ProductsCollectionName { get; set; } = string.Empty;
    }
}
