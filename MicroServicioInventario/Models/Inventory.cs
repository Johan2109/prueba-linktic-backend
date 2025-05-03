using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroServicioInventario.Models
{
    // Representa un documento de inventario en la colección de MongoDB
    public class Inventory
    {
        // Identificador único del documento en MongoDB
        // Se marca como BsonId y se convierte automáticamente de string a ObjectId
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = string.Empty;

        // ID del producto asociado a este inventario
        // Se almacena en MongoDB como "productId"
        [BsonElement("productId")]
        public string ProductId { get; set; } = null!;

        // Cantidad de unidades disponibles del producto
        // Se almacena en MongoDB como "stock"
        [BsonElement("stock")]
        public int Stock { get; set; }
    }
}
