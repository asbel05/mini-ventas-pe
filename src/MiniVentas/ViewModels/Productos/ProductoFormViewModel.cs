using System.ComponentModel.DataAnnotations;

namespace MiniVentas.ViewModels.Productos
{
    public class ProductoFormViewModel
    {
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El código SKU es obligatorio.")]
        [StringLength(32, ErrorMessage = "El SKU no puede superar los 32 caracteres.")]
        [Display(Name = "Código SKU")]
        public string Sku { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(120, ErrorMessage = "El nombre no puede superar los 120 caracteres.")]
        [Display(Name = "Nombre del Producto")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(0.00, 99999999.99, ErrorMessage = "El precio debe ser mayor o igual a 0.00.")]
        [Display(Name = "Precio de Venta (S/)")]
        public decimal PrecioVenta { get; set; }

        [Display(Name = "Producto activo")]
        public bool Activo { get; set; } = true;

        public bool EsEdicion => ProductoId > 0;
    }
}

