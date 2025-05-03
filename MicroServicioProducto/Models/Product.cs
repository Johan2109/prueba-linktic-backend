// Importa los espacios de nombres necesarios para trabajar con BSON (formato usado por MongoDB)
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroServicioProducto.Models
{
    // Esta clase representa un documento en la colección de productos en MongoDB
    public class Product
    {
        // Identificador único del producto (MongoDB lo maneja como ObjectId)
        [BsonId] // Indica que esta propiedad es la clave primaria del documento
        [BsonRepresentation(BsonType.ObjectId)] // Se almacenará como ObjectId, pero se manipula como string en C#
        public string? Id { get; set; }

        // Nombre del producto
        [BsonElement("name")] // Mapea esta propiedad con el campo "name" en el documento BSON
        public string Name { get; set; } = null!;

        // Descripción del producto
        [BsonElement("description")]
        public string Description { get; set; } = null!;

        // Precio del producto
        [BsonElement("price")]
        public decimal Price { get; set; }
    }
}
