namespace MiniVentas.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public bool Activo { get; set; }
    }
}
