using MiniVentas.Models;

namespace MiniVentas.ViewModels
{
    public class ProductoIndexViewModel
    {
        public string? Busqueda { get; set; }
        public bool? SoloActivos { get; set; }
        public List<Producto> Producto { get; set; } = [];
    }
}
