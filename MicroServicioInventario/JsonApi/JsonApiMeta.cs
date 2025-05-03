namespace MicroServicioInventario.JsonApi
{
    // Esta clase representa la sección "meta" en una respuesta JSON:API,
    // utilizada comúnmente para incluir información adicional sobre paginación.
    public class JsonApiMeta
    {
        // Cantidad total de elementos disponibles en la colección
        public long TotalItems { get; set; }

        // Número de página actual
        public int Page { get; set; }

        // Cantidad de elementos por página
        public int PageSize { get; set; }

        // Número total de páginas disponibles basado en los filtros y el tamaño de página
        public int TotalPages { get; set; }
    }
}
