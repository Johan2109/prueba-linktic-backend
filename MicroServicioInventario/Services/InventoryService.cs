using MicroServicioInventario.Models;
using MicroServicioInventario.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MicroServicioInventario.Services
{
    // Servicio que encapsula la lógica de acceso a datos para la colección Inventory en MongoDB
    public class InventoryService
    {
        // Referencia a la colección Inventory en la base de datos de MongoDB
        private readonly IMongoCollection<Inventory> _inventoryCollection;

        // Constructor que recibe la configuración de MongoDB y establece la conexión con la colección correspondiente
        public InventoryService(IOptions<MongoDBSettings> mongoSettings)
        {
            // Se crea un cliente de MongoDB con la cadena de conexión especificada
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            // Se obtiene la base de datos usando el nombre configurado
            var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            // Se accede a la colección de inventario definida en la configuración
            _inventoryCollection = database.GetCollection<Inventory>(mongoSettings.Value.InventoryCollectionName);
        }

        // Obtiene todos los documentos de inventario de la colección
        public async Task<List<Inventory>> GetAllAsync() =>
            await _inventoryCollection.Find(_ => true).ToListAsync();

        // Busca un documento de inventario por el ID del producto asociado
        public async Task<Inventory?> GetByProductIdAsync(string productId) =>
            await _inventoryCollection.Find(i => i.ProductId == productId).FirstOrDefaultAsync();

        // Crea un nuevo documento de inventario en la colección
        public async Task CreateAsync(Inventory inventory) =>
            await _inventoryCollection.InsertOneAsync(inventory);

        // Actualiza el stock de un producto específico según su ID
        public async Task<bool> UpdateStockAsync(string productId, int newStock)
        {
            var result = await _inventoryCollection.UpdateOneAsync(
                i => i.ProductId == productId, // filtro: coincidencia por ID del producto
                Builders<Inventory>.Update.Set(i => i.Stock, newStock) // operación: actualizar el campo stock
            );

            // Devuelve true si al menos un documento fue modificado
            return result.ModifiedCount > 0;
        }

        // Elimina el documento de inventario asociado a un producto por su ID
        public async Task<bool> DeleteByProductIdAsync(string productId)
        {
            var result = await _inventoryCollection.DeleteOneAsync(i => i.ProductId == productId);
            // Devuelve true si al menos un documento fue eliminado
            return result.DeletedCount > 0;
        }
    }
}
