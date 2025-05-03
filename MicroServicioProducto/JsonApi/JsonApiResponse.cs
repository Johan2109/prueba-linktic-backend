namespace MicroServicioProducto.JsonApi
{
    // 📦 Representa una respuesta estándar JSON:API con datos y metadatos
    public class JsonApiResponse<T>
    {
        // 🔸 Propiedad principal que contiene los datos devueltos (puede ser una entidad o lista de entidades)
        public T Data { get; set; }

        // ℹ️ Metadatos asociados a la respuesta, como información de paginación
        public JsonApiMeta Meta { get; set; }
    }
}
