using MiniVentas.Models;

namespace MiniVentas.ViewModels.Usuarios
{
    public class UsuarioIndexViewModel
    {
        public string? Busqueda { get; set; }
        public byte? RolId { get; set; }
        public bool? SoloActivos { get; set; }
        public List<Usuario> Usuarios { get; set; } = [];
    }
}
