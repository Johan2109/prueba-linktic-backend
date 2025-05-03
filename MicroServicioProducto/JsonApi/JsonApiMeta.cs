namespace MicroServicioProducto.JsonApi
{
    // ℹ️ Representa información adicional sobre una respuesta paginada según el estándar JSON:API
    public class JsonApiMeta
    {
        // 🔢 Número total de elementos disponibles en la colección
        public long TotalItems { get; set; }

        // 📄 Número de la página actual
        public int Page { get; set; }

        // 📦 Cantidad de elementos por página
        public int PageSize { get; set; }

        // 📊 Cantidad total de páginas calculadas
        public int TotalPages { get; set; }
    }
}
