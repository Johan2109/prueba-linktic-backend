namespace MicroServicioInventario.JsonApi
{
    // Esta clase representa una estructura de datos genérica siguiendo el estándar JSON:API
    public class JsonApiData<T>
    {
        // Tipo del recurso (por ejemplo: "inventory", "product")
        public string Type { get; set; } = default!;

        // Identificador único del recurso (por ejemplo: el ID del inventario)
        public string Id { get; set; } = default!;

        // Contiene los atributos del recurso (se mapea a una entidad, como Inventory o Product)
        public T Attributes { get; set; } = default!;

        // Información de relaciones con otros recursos (opcional)
        public object? Relationships { get; set; }
    }
}