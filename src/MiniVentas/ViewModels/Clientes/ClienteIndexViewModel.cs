using MiniVentas.Models;

namespace MiniVentas.ViewModels.Clientes
{
    public class ClienteIndexViewModel
    {
        public string? Busqueda { get; set; }
        public string? TipoDocumento { get; set; }
        public List<Cliente> Clientes { get; set; } = [];
    }
}

