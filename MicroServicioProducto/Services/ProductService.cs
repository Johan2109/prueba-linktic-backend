// Importaciones necesarias
using MicroServicioProducto.Models; // Modelo de datos Product y configuración de MongoDB
using Microsoft.Extensions.Options; // Para acceder a la configuración inyectada desde appsettings
using MongoDB.Driver; // Driver oficial de MongoDB para .NET

namespace MicroServicioProducto.Services
{
    // Servicio encargado de manejar la lógica de acceso a datos para productos en MongoDB
    public class ProductService
    {
        // Representa la colección de productos en MongoDB
        private readonly IMongoCollection<Product> _productsCollection;

        // Constructor: inicializa la conexión a la base de datos usando la configuración inyectada
        public ProductService(IOptions<MongoDBSettings> mongoSettings)
        {
            // Crear cliente MongoDB con la cadena de conexión del archivo de configuración
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            // Obtener la base de datos por nombre
            var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            // Obtener la colección de productos
            _productsCollection = database.GetCollection<Product>(mongoSettings.Value.ProductsCollectionName);
        }

        // Obtener todos los productos sin paginación
        public async Task<List<Product>> GetAllAsync() =>
            await _productsCollection.Find(_ => true).ToListAsync();

        // Obtener productos con paginación y el total de registros
        public async Task<(List<Product> products, long totalCount)> GetPagedAsync(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;

            var products = await _productsCollection.Find(_ => true)
                                          .Skip(skip) // Saltar los elementos de páginas anteriores
                                          .Limit(pageSize) // Limitar al tamaño de página
                                          .ToListAsync();

            var totalCount = await _productsCollection.CountDocumentsAsync(_ => true); // Contar todos los productos

            return (products, totalCount);
        }

        // Buscar un producto por ID
        public async Task<Product?> GetByIdAsync(string id) =>
            await _productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();

        // Crear un nuevo producto
        public async Task CreateAsync(Product product) =>
            await _productsCollection.InsertOneAsync(product);

        // Actualizar un producto existente por ID
        public async Task UpdateAsync(string id, Product updated) =>
            await _productsCollection.ReplaceOneAsync(p => p.Id == id, updated);

        // Eliminar un producto por ID
        public async Task DeleteAsync(string id) =>
            await _productsCollection.DeleteOneAsync(p => p.Id == id);
    }
}
