namespace MicroServicioProducto.JsonApi
{
    // 📦 Clase genérica que representa la estructura del nodo "data" en una respuesta JSON:API
    public class JsonApiData<T>
    {
        // 🔖 "type" representa el tipo del recurso, por ejemplo: "products"
        public string Type { get; set; }

        // 🆔 "id" representa el identificador único del recurso
        public string Id { get; set; }

        // 📋 "attributes" contiene los datos reales del recurso, mapeados al tipo T (modelo)
        public T Attributes { get; set; }
    }
}