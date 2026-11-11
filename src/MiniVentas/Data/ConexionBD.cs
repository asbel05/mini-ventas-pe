using Microsoft.Data.SqlClient;

namespace MiniVentas.Data
{
    public class ConexionBD
    {
        private readonly string _cadenaSql;

        public ConexionBD(IConfiguration configuration)
        {
            _cadenaSql = configuration.GetConnectionString("CadenaSql");
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(_cadenaSql);
        }
    }
}
