// Importa las bibliotecas necesarias para pruebas unitarias, acceso a MongoDB y configuración
using Xunit; // Framework de pruebas unitarias
using System.Threading.Tasks; // Soporte para métodos async
using MicroServicioInventario.Models; // Modelo Inventory
using MicroServicioInventario.Services; // Servicio InventoryService
using MicroServicioInventario.Settings; // Configuración personalizada de MongoDB
using MongoDB.Driver; // Cliente de MongoDB
using Microsoft.Extensions.Options; // Para pasar configuraciones
using Microsoft.Extensions.Configuration; // No usado directamente aquí pero útil en general
using MongoDB.Bson; // Para generar ObjectId
using System.Collections.Generic; // Para listas

// Clase de pruebas unitarias para InventoryService
public class TestsInventory
{
    private readonly InventoryService _service; // Servicio a probar
    private readonly IMongoCollection<Inventory> _inventoryCollection; // Referencia directa a la colección de inventarios

    // Constructor: configura la conexión y prepara el servicio
    public TestsInventory()
    {
        // Configuración de conexión a MongoDB local
        var mongoSettings = Options.Create(new MongoDBSettings
        {
            ConnectionString = "mongodb://localhost:27017", // Servidor local
            DatabaseName = "PruebaLinkticDB", // Base de datos usada para pruebas
            InventoryCollectionName = "Inventory" // Nombre de la colección
        });

        // Crear cliente y acceder a la colección
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _inventoryCollection = database.GetCollection<Inventory>(mongoSettings.Value.InventoryCollectionName);

        // Crear instancia del servicio con la configuración
        _service = new InventoryService(mongoSettings);
    }

    // Prueba para verificar que se pueda guardar un inventario en la base de datos
    [Fact]
    public async Task CreateAsync_SavesInventoryToDatabase()
    {
        var id = ObjectId.GenerateNewId().ToString(); // ID único del documento
        var productId = ObjectId.GenerateNewId().ToString(); // ID único del producto relacionado

        // Crear nuevo inventario
        var newInventory = new Inventory
        {
            Id = id,
            ProductId = productId,
            Stock = 100
        };

        // Ejecutar creación
        await _service.CreateAsync(newInventory);

        // Verificar que se guardó correctamente
        var insertedInventory = await _inventoryCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        Assert.NotNull(insertedInventory);
        Assert.Equal(productId, insertedInventory.ProductId);
        Assert.Equal(100, insertedInventory.Stock);

        // Limpieza
        await _inventoryCollection.DeleteOneAsync(Builders<Inventory>.Filter.Eq(x => x.Id, id));
    }

    // Prueba para obtener un inventario existente por ProductId
    [Fact]
    public async Task GetByProductIdAsync_ReturnsInventoryIfExists()
    {
        var id = ObjectId.GenerateNewId().ToString(); // ID del inventario
        var productId = ObjectId.GenerateNewId().ToString(); // ID del producto

        // Insertar un inventario de prueba
        var testInventory = new Inventory { Id = id, ProductId = productId, Stock = 50 };
        await _inventoryCollection.InsertOneAsync(testInventory);

        // Ejecutar búsqueda por productId
        var retrievedInventory = await _service.GetByProductIdAsync(productId);

        // Verificar resultados
        Assert.NotNull(retrievedInventory);
        Assert.Equal(productId, retrievedInventory.ProductId);
        Assert.Equal(50, retrievedInventory.Stock);

        // Limpieza
        await _inventoryCollection.DeleteOneAsync(p => p.Id == id);
    }

    // Prueba para actualizar el stock de un inventario
    [Fact]
    public async Task UpdateStockAsync_ModifiesStockSuccessfully()
    {
        var id = ObjectId.GenerateNewId().ToString(); // ID del documento
        var productId = ObjectId.GenerateNewId().ToString(); // ID del producto

        // Insertar inventario inicial
        var testInventory = new Inventory { Id = id, ProductId = productId, Stock = 30 };
        await _inventoryCollection.InsertOneAsync(testInventory);

        // Ejecutar actualización del stock
        var updateResult = await _service.UpdateStockAsync(productId, 75);

        // Verificar que el stock fue actualizado
        var modifiedInventory = await _inventoryCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        Assert.True(updateResult);
        Assert.NotNull(modifiedInventory);
        Assert.Equal(75, modifiedInventory.Stock);

        // Limpieza
        await _inventoryCollection.DeleteOneAsync(p => p.Id == id);
    }

    // Prueba para eliminar un inventario por su ProductId
    [Fact]
    public async Task DeleteByProductIdAsync_RemovesInventorySuccessfully()
    {
        var id = ObjectId.GenerateNewId().ToString(); // ID del inventario
        var productId = ObjectId.GenerateNewId().ToString(); // ID del producto

        // Insertar inventario de prueba
        var testInventory = new Inventory { Id = id, ProductId = productId, Stock = 20 };
        await _inventoryCollection.InsertOneAsync(testInventory);

        // Ejecutar eliminación por ProductId
        var deleteResult = await _service.DeleteByProductIdAsync(productId);

        // Verificar que el documento fue eliminado
        var deletedInventory = await _inventoryCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        Assert.True(deleteResult);
        Assert.Null(deletedInventory);
    }
}
