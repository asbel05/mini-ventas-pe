namespace MiniVentas.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public byte RolId { get; set; }
        public string RolCodigo { get; set; } = string.Empty;
        public string RolNombre { get; set; } = string.Empty;
        public int? VendedorId { get; set; }
        public string? CodigoVendedor { get; set; }
        public string? ZonaVentas { get; set; }
    }
}
