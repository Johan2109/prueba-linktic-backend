namespace MicroServicioInventario.JsonApi
{
    // Representa una respuesta JSON:API para múltiples recursos (colección)
    public class JsonApiResponse<T>
    {
        // Lista de datos siguiendo el estándar JSON:API
        public IEnumerable<JsonApiData<T>> Data { get; set; } = default!;

        // Información opcional de metadatos como paginación, conteo, etc.
        public JsonApiMeta? Meta { get; set; }
    }

    // Representa una respuesta JSON:API para un solo recurso
    public class JsonApiResponseSingle<T>
    {
        // Único recurso devuelto
        public JsonApiData<T> Data { get; set; } = default!;
    }
}