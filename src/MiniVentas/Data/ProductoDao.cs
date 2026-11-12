using Microsoft.Data.SqlClient;
using MiniVentas.Models;
using System.Data;

namespace MiniVentas.Data
{
    public enum ResultadoOperacionProducto
    {
        Exito,
        SkuDuplicado,
        NoEncontrado
    }
    public class ProductoDao
    {
        private readonly ConexionBD _conexion;
        public ProductoDao(ConexionBD conexion)
        {
            _conexion = conexion;
        }
        public async Task<List<Producto>> Listar(string? busqueda = null, bool? soloActivos = null)
        {
            var lista = new List<Producto>();
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("ventas.sp_ListarProductos", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@busqueda", SqlDbType.NVarChar, 120).Value = string.IsNullOrWhiteSpace(busqueda) ? DBNull.Value : busqueda.Trim();
            command.Parameters.Add("@solo_activos", SqlDbType.Bit).Value = soloActivos.HasValue ? soloActivos.Value : DBNull.Value;

            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                lista.Add(new Producto
                {
                    ProductoId = (int)dr["producto_id"],
                    Sku = (string)dr["sku"],
                    Nombre = (string)dr["nombre"],
                    PrecioVenta = (decimal)dr["precio_venta"],
                    Activo = (bool)dr["activo"]
                });
            }
            return lista;
        }

        public async Task<Producto?> ObtenerPorId(int productoId)
        {
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("ventas.sp_ObtenerProductoPorId", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@producto_id", SqlDbType.Int).Value = productoId;

            await connection.OpenAsync();
            using var dr = await command.ExecuteReaderAsync();

            if (!await dr.ReadAsync())
                return null;
            return new Producto
            {
                ProductoId = (int)dr["producto_id"],
                Sku = (string)dr["sku"],
                Nombre = (string)dr["nombre"],
                PrecioVenta = (decimal)dr["precio_venta"],
                Activo = (bool)dr["activo"]
            };
        }
        public async Task<ResultadoOperacionProducto> Crear(Producto producto)
        {
            try
            {
                using var connection = _conexion.ObtenerConexion();
                using var command = new SqlCommand("ventas.sp_CrearProducto", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@sku", SqlDbType.VarChar, 32).Value = producto.Sku;
                command.Parameters.Add("@nombre", SqlDbType.NVarChar, 120).Value = producto.Nombre;
                command.Parameters.Add("@precio_venta", SqlDbType.Decimal).Value = producto.PrecioVenta;
                command.Parameters.Add("@activo", SqlDbType.Bit).Value = producto.Activo;

                await connection.OpenAsync();
                await command.ExecuteScalarAsync();
                return ResultadoOperacionProducto.Exito;
            }
            catch (SqlException ex) when (ex.Number is CodigosErrorSql.RestriccionUnica or CodigosErrorSql.IndiceUnico)
            {
                return ResultadoOperacionProducto.SkuDuplicado;
            }
        }

        public async Task<ResultadoOperacionProducto> Actualizar(Producto producto)
        {
            try
            {
                using var connection = _conexion.ObtenerConexion();
                using var command = new SqlCommand("ventas.sp_ActualizarProducto", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@producto_id", SqlDbType.Int).Value = producto.ProductoId;
                command.Parameters.Add("@sku", SqlDbType.VarChar, 32).Value = producto.Sku;
                command.Parameters.Add("@nombre", SqlDbType.NVarChar, 120).Value = producto.Nombre;
                command.Parameters.Add("@precio_venta", SqlDbType.Decimal).Value = producto.PrecioVenta;
                command.Parameters.Add("@activo", SqlDbType.Bit).Value = producto.Activo;

                await connection.OpenAsync();
                bool actualizado = (bool)(await command.ExecuteScalarAsync())!;
                return actualizado ? ResultadoOperacionProducto.Exito : ResultadoOperacionProducto.NoEncontrado;
            }
            catch (SqlException ex) when (ex.Number is CodigosErrorSql.RestriccionUnica or CodigosErrorSql.IndiceUnico)
            {
                return ResultadoOperacionProducto.SkuDuplicado;
            }
        }

        public async Task<bool> CambiarEstado(int productoId, bool activo)
        {
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("ventas.sp_CambiarEstadoProducto", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@producto_id", SqlDbType.Int).Value = productoId;
            command.Parameters.Add("@activo", SqlDbType.Bit).Value = activo;

            await connection.OpenAsync();
            return (bool)(await command.ExecuteScalarAsync())!;
        }
    }
}