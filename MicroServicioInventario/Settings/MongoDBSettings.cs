namespace MicroServicioInventario.Settings
{
    // Esta clase representa la configuración necesaria para conectarse a MongoDB.
    // Se usa junto con IOptions<MongoDBSettings> para inyectar la configuración desde appsettings.json u otro proveedor de configuración.
    public class MongoDBSettings
    {
        // Cadena de conexión de MongoDB (por ejemplo: mongodb://localhost:27017)
        public string ConnectionString { get; set; } = null!;

        // Nombre de la base de datos que se utilizará (por ejemplo: InventarioDB)
        public string DatabaseName { get; set; } = null!;

        // Nombre de la colección donde se almacenarán los documentos de inventario
        public string InventoryCollectionName { get; set; } = null!;
    }
}