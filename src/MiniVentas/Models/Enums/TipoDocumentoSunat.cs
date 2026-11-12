namespace MiniVentas.Models.Enums
{
    public class TipoDocumentoSunat
    {
        public string Codigo { get; }
        public string Abreviatura { get; }
        public int LongitudMinima { get; }
        public int LongitudMaxima { get; }
        public bool SoloNumeros { get; }

        private TipoDocumentoSunat(string codigo, string abreviatura, int min, int max, bool soloNumeros)
        {
            Codigo = codigo;
            Abreviatura = abreviatura;
            LongitudMinima = min;
            LongitudMaxima = max;
            SoloNumeros = soloNumeros;
        }

        public static readonly TipoDocumentoSunat SinDocumento = new("0", "VARIOS", 1, 12, false);
        public static readonly TipoDocumentoSunat Dni = new("1", "DNI", 8, 8, true);
        public static readonly TipoDocumentoSunat CarnetExtranjeria = new("4", "CE", 8, 12, false);
        public static readonly TipoDocumentoSunat Ruc = new("6", "RUC", 11, 11, true);
        public static readonly TipoDocumentoSunat Pasaporte = new("7", "PASAPORTE", 8, 12, false);

        public static readonly IReadOnlyList<TipoDocumentoSunat> DisponiblesParaClientes =
        [
            Dni,
            Ruc,
            CarnetExtranjeria,
            Pasaporte
        ];

        public static TipoDocumentoSunat Obtener(string? codigo) => codigo switch
        {
            "0" => SinDocumento,
            "1" => Dni,
            "4" => CarnetExtranjeria,
            "6" => Ruc,
            "7" => Pasaporte,
            _ => new TipoDocumentoSunat(codigo ?? string.Empty, "OTRO", 1, 12, false)
        };
    }
}

