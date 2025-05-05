// Importa los paquetes necesarios para pruebas unitarias, MongoDB y configuración
using Xunit; // Framework de pruebas unitarias
using System.Threading.Tasks; // Soporte para async/await
using MicroServicioProducto.Models; // Modelo Product
using MicroServicioProducto.Services; // Servicio ProductService
using MongoDB.Driver; // Cliente y operaciones MongoDB
using Microsoft.Extensions.Options; // Para manejar configuración inyectada
using Microsoft.Extensions.Configuration; // Para configurar settings desde appsettings.json si fuera necesario
using MongoDB.Bson; // Para trabajar con ObjectId de MongoDB
using System.Collections.Generic; // Soporte para List<>

// Clase de pruebas unitarias para el servicio ProductService
public class UnitTests1
{
    private readonly ProductService _service; // Instancia del servicio a probar
    private readonly IMongoCollection<Product> _productsCollection; // Referencia directa a la colección de productos

    // Constructor que configura la conexión a MongoDB de forma local
    public UnitTests1()
    {
        // Configuración manual de los parámetros de conexión a MongoDB
        var mongoSettings = Options.Create(new MongoDBSettings
        {
            ConnectionString = "mongodb://localhost:27017", // Dirección de Mongo local
            DatabaseName = "PruebaLinkticDB", // Nombre de la base de datos
            ProductsCollectionName = "Products" // Nombre de la colección
        });

        // Crear cliente Mongo y obtener la base de datos y colección
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        _productsCollection = database.GetCollection<Product>(mongoSettings.Value.ProductsCollectionName);

        // Crear instancia del servicio a probar
        _service = new ProductService(mongoSettings);
    }

    // Prueba: Crear producto y verificar que se haya guardado
    [Fact]
    public async Task CreateAsync_SavesProductToDatabase()
    {
        // Crear un ID válido para MongoDB
        var id = ObjectId.GenerateNewId().ToString();

        // Crear un nuevo producto de prueba
        var newProduct = new Product
        {
            Id = id,
            Name = "Producto Test",
            Description = "Prueba unitaria",
            Price = 999
        };

        // Llamar al método que guarda el producto en MongoDB
        await _service.CreateAsync(newProduct);

        // Verificar que el producto se haya guardado correctamente
        var insertedProduct = await _productsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        Assert.NotNull(insertedProduct); // El producto no debe ser null
        Assert.Equal("Producto Test", insertedProduct.Name);
        Assert.Equal(999, insertedProduct.Price);

        // Eliminar el producto después de la prueba para limpiar la BD
        await _productsCollection.DeleteOneAsync(Builders<Product>.Filter.Eq(x => x.Id, id));
    }

    // Prueba: Actualizar producto existente
    [Fact]
    public async Task UpdateAsync_ModifiesExistingProduct()
    {
        // Crear un producto inicial
        var id = ObjectId.GenerateNewId().ToString();
        var originalProduct = new Product { Id = id, Name = "Producto Antiguo", Price = 300 };
        await _productsCollection.InsertOneAsync(originalProduct);

        // Crear un producto con los datos actualizados
        var updatedProduct = new Product { Id = id, Name = "Producto Actualizado", Price = 600 };

        // Ejecutar la actualización usando el servicio
        await _service.UpdateAsync(id, updatedProduct);

        // Recuperar el producto modificado para validar los cambios
        var modifiedProduct = await _productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        Assert.NotNull(modifiedProduct);
        Assert.Equal("Producto Actualizado", modifiedProduct.Name);
        Assert.Equal(600, modifiedProduct.Price);

        // Limpieza: eliminar el producto de prueba
        await _productsCollection.DeleteOneAsync(p => p.Id == id);
    }

    // Prueba: Eliminar un producto
    [Fact]
    public async Task DeleteAsync_RemovesProductFromDatabase()
    {
        // Crear un producto de prueba
        var id = ObjectId.GenerateNewId().ToString();
        var testProduct = new Product { Id = id, Name = "Producto a Eliminar", Price = 400 };
        await _productsCollection.InsertOneAsync(testProduct);

        // Eliminar el producto usando el servicio
        await _service.DeleteAsync(id);

        // Verificar que el producto ya no exista en la base de datos
        var deletedProduct = await _productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        Assert.Null(deletedProduct); // Debe ser null si se eliminó correctamente
    }

    // Prueba: Buscar un producto por ID
    [Fact]
    public async Task GetByIdAsync_ReturnsProductIfExists()
    {
        // Crear e insertar un producto
        var id = ObjectId.GenerateNewId().ToString();
        var testProduct = new Product { Id = id, Name = "Producto Test", Price = 500 };
        await _productsCollection.InsertOneAsync(testProduct);

        // Obtener el producto por ID usando el servicio
        var retrievedProduct = await _service.GetByIdAsync(id);

        // Validar que el producto recuperado es el esperado
        Assert.NotNull(retrievedProduct);
        Assert.Equal("Producto Test", retrievedProduct.Name);
        Assert.Equal(500, retrievedProduct.Price);

        // Limpieza: eliminar el producto después de la prueba
        await _productsCollection.DeleteOneAsync(p => p.Id == id);
    }
}
