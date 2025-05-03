namespace MicroServicioProducto.JsonApi
{
    // ❗ Representa un error estructurado según el formato JSON:API
    public class JsonApiError
    {
        // 🔢 Código de estado HTTP del error, por ejemplo: 404, 500
        public int Status { get; set; }

        // 📝 Título corto del error, útil para mostrar al usuario o registrar
        public string Title { get; set; }

        // 📄 Descripción más detallada del error, ideal para debugging o soporte
        public string Detail { get; set; }
    }
}