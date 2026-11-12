using MiniVentas.Models.Enums;

namespace MiniVentas.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string OrigenDatos { get; set; } = "MANUAL";
        public DateTime? ConsultadoEn { get; set; }

        public string TipoDocumentoNombre => TipoDocumentoSunat.Obtener(TipoDocumento).Abreviatura;
    }
}
