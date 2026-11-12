using Microsoft.Data.SqlClient;
using MiniVentas.Models;
using MiniVentas.Models.Enums;
using System.Data;

namespace MiniVentas.Data
{
    public class ClienteDao
    {
        private readonly ConexionBD _conexion;
        public ClienteDao(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        public async Task<List<Cliente>> Listar(string? busqueda = null, string? tipoDocumento = null)
        {
            var lista = new List<Cliente>();
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("ventas.sp_ListarClientes", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@busqueda", SqlDbType.NVarChar, 160).Value = string.IsNullOrWhiteSpace(busqueda) ? DBNull.Value : busqueda.Trim();
            command.Parameters.Add("@tipo_documento", SqlDbType.Char, 1).Value = string.IsNullOrWhiteSpace(tipoDocumento) ? DBNull.Value : tipoDocumento.Trim();

            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                lista.Add(new Cliente
                {
                    ClienteId = (int)dr["cliente_id"],
                    TipoDocumento = (string)dr["tipo_documento"],
                    NumeroDocumento = (string)dr["numero_documento"],
                    Nombre = (string)dr["nombre"],
                    OrigenDatos = (string)dr["origen_datos"],
                    ConsultadoEn = dr["consultado_en"] == DBNull.Value ? null : (DateTime)dr["consultado_en"]
                });
            }
            return lista;
        }

        public async Task<Cliente?> ObtenerPorId(int clienteId)
        {
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("ventas.sp_ObtenerClientePorId", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@cliente_id", SqlDbType.Int).Value = clienteId;

            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync();

            if (!await dr.ReadAsync())
                return null;
            return new Cliente
            {
                ClienteId = (int)dr["cliente_id"],
                TipoDocumento = (string)dr["tipo_documento"],
                NumeroDocumento = (string)dr["numero_documento"],
                Nombre = (string)dr["nombre"],
                OrigenDatos = (string)dr["origen_datos"],
                ConsultadoEn = dr["consultado_en"] == DBNull.Value ? null : (DateTime)dr["consultado_en"]
            };
        }

        public async Task<ResultadoOperacionCliente> Crear(Cliente cliente)
        {
            try
            {
                using var connection = _conexion.ObtenerConexion();
                using var command = new SqlCommand("ventas.sp_CrearCliente", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@tipo_documento", SqlDbType.Char, 1).Value = cliente.TipoDocumento;
                command.Parameters.Add("@numero_documento", SqlDbType.VarChar, 12).Value = cliente.NumeroDocumento;
                command.Parameters.Add("@nombre", SqlDbType.NVarChar, 160).Value = cliente.Nombre;
                command.Parameters.Add("@origen_datos", SqlDbType.VarChar, 12).Value = cliente.OrigenDatos;
                command.Parameters.Add("@consultado_en", SqlDbType.DateTime2).Value = cliente.ConsultadoEn.HasValue ? cliente.ConsultadoEn.Value : DBNull.Value;

                await connection.OpenAsync();
                await command.ExecuteScalarAsync();
                return ResultadoOperacionCliente.Exito;
            }
            catch (SqlException ex) when (ex.Number is CodigosErrorSql.RestriccionUnica or CodigosErrorSql.IndiceUnico)
            {
                return ResultadoOperacionCliente.DocumentoDuplicado;
            }
        }

        public async Task<ResultadoOperacionCliente> Actualizar(Cliente cliente)
        {
            try
            {
                using var connection = _conexion.ObtenerConexion();
                using var command = new SqlCommand("ventas.sp_ActualizarCliente", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@cliente_id", SqlDbType.Int).Value = cliente.ClienteId;
                command.Parameters.Add("@tipo_documento", SqlDbType.Char, 1).Value = cliente.TipoDocumento;
                command.Parameters.Add("@numero_documento", SqlDbType.VarChar, 12).Value = cliente.NumeroDocumento;
                command.Parameters.Add("@nombre", SqlDbType.NVarChar, 160).Value = cliente.Nombre;

                await connection.OpenAsync();
                bool actualizado = (bool)(await command.ExecuteScalarAsync())!;
                return actualizado ? ResultadoOperacionCliente.Exito : ResultadoOperacionCliente.NoEncontrado;
            }
            catch (SqlException ex) when (ex.Number is CodigosErrorSql.RestriccionUnica or CodigosErrorSql.IndiceUnico)
            {
                return ResultadoOperacionCliente.DocumentoDuplicado;
            }
        }
    }
}