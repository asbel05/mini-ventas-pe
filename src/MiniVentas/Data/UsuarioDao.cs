using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using MiniVentas.Models;
using MiniVentas.Models.Enums;
using System.Data;

namespace MiniVentas.Data
{
    public class UsuarioDao
    {
        private readonly ConexionBD _conexion;

        public UsuarioDao(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        public async Task<Usuario?> ObtenerPorNombreUsuario(string nombreUsuario)
        {
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("seguridad.sp_ObtenerUsuarioLogin", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@nombre_usuario", SqlDbType.NVarChar, 100).Value = nombreUsuario;

            await connection.OpenAsync();

            using var dr = await command.ExecuteReaderAsync();
            if (!await dr.ReadAsync())
                return null;

            return new Usuario
            {
                UsuarioId = (int)dr["usuario_id"],
                NombreUsuario = (string)dr["nombre_usuario"],
                PasswordHash = (string)dr["password_hash"],
                Nombre = (string)dr["nombre"],
                Activo = (bool)dr["activo"],
                RolId = (byte)dr["rol_id"],
                RolCodigo = (string)dr["rol_codigo"],
                RolNombre = (string)dr["rol_nombre"],
                VendedorId = dr["vendedor_id"] != DBNull.Value ? (int)dr["vendedor_id"] : null
            };
        }

        public async Task<List<Usuario>> Listar(string? busqueda = null, byte? rolId = null, bool? soloActivos = null)
        {
            var lista = new List<Usuario>();
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("seguridad.sp_ListarUsuarios", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@busqueda", SqlDbType.NVarChar, 120).Value =string.IsNullOrWhiteSpace(busqueda) ? DBNull.Value : busqueda.Trim();
            command.Parameters.Add("@rol_id", SqlDbType.TinyInt).Value =rolId.HasValue ? rolId.Value : DBNull.Value;
            command.Parameters.Add("@solo_activos", SqlDbType.Bit).Value =soloActivos.HasValue ? soloActivos.Value : DBNull.Value;

            await connection.OpenAsync();

            using var dr = await command.ExecuteReaderAsync();

            while (await dr.ReadAsync())
            {
                lista.Add(new Usuario
                {
                    UsuarioId = (int)dr["usuario_id"],
                    NombreUsuario = (string)dr["nombre_usuario"],
                    Nombre = (string)dr["nombre"],
                    Activo = (bool)dr["activo"],
                    RolId = (byte)dr["rol_id"],
                    RolCodigo = (string)dr["rol_codigo"],
                    RolNombre = (string)dr["rol_nombre"],
                    VendedorId = dr["vendedor_id"] != DBNull.Value ? (int)dr["vendedor_id"] : null,
                    CodigoVendedor = dr["codigo_vendedor"] != DBNull.Value ? (string)dr["codigo_vendedor"] : null,
                    ZonaVentas = dr["zona_ventas"] != DBNull.Value ? (string)dr["zona_ventas"] : null
                });
            }
            return lista;
        }

        public async Task<Usuario?> ObtenerPorId(int id)
        {
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("seguridad.sp_ObtenerUsuarioPorId", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            using var dr = await command.ExecuteReaderAsync();
            if (!await dr.ReadAsync())
                return null;

            return new Usuario
            {
                UsuarioId = (int)dr["usuario_id"],
                NombreUsuario = (string)dr["nombre_usuario"],
                Nombre = (string)dr["nombre"],
                Activo = (bool)dr["activo"],
                RolId = (byte)dr["rol_id"],
                RolCodigo = (string)dr["rol_codigo"],
                RolNombre = (string)dr["rol_nombre"],
                VendedorId = dr["vendedor_id"] != DBNull.Value ? (int)dr["vendedor_id"] : null,
                CodigoVendedor = dr["codigo_vendedor"] != DBNull.Value ? (string)dr["codigo_vendedor"] : null,
                ZonaVentas = dr["zona_ventas"] != DBNull.Value ? (string)dr["zona_ventas"] : null
            };
        }

        public async Task<ResultadoOperacionUsuario> Crear(Usuario usuario)
        {
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("seguridad.sp_CrearUsuario", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@nombre_usuario", SqlDbType.NVarChar, 100).Value = usuario.NombreUsuario.Trim();
            command.Parameters.Add("@password_hash", SqlDbType.VarChar, 255).Value = usuario.PasswordHash;
            command.Parameters.Add("@rol_id", SqlDbType.TinyInt).Value = usuario.RolId;
            command.Parameters.Add("@nombre", SqlDbType.NVarChar, 120).Value = usuario.Nombre.Trim();
            command.Parameters.Add("@activo", SqlDbType.Bit).Value = usuario.Activo;
            command.Parameters.Add("@codigo_vendedor", SqlDbType.VarChar, 24).Value = 
                string.IsNullOrWhiteSpace(usuario.CodigoVendedor) ? DBNull.Value : usuario.CodigoVendedor.Trim();
            command.Parameters.Add("@zona_ventas", SqlDbType.NVarChar, 80).Value = 
                string.IsNullOrWhiteSpace(usuario.ZonaVentas) ? DBNull.Value : usuario.ZonaVentas.Trim();

            await connection.OpenAsync();

            try
            {
                await command.ExecuteScalarAsync();
                return ResultadoOperacionUsuario.Exito;
            }
            catch (SqlException ex) when (ex.Number is CodigosErrorSql.RestriccionUnica or CodigosErrorSql.IndiceUnico)
            {
                if (ex.Message.Contains("UQ_VENDEDOR_codigo", StringComparison.OrdinalIgnoreCase))
                    return ResultadoOperacionUsuario.CodigoVendedorDuplicado;
                return ResultadoOperacionUsuario.UsuarioDuplicado;
            }
        }

        public async Task<ResultadoOperacionUsuario> Actualizar(Usuario usuario, string? nuevoPasswordHash)
        {
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("seguridad.sp_ActualizarUsuario", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = usuario.UsuarioId;
            command.Parameters.Add("@nombre_usuario", SqlDbType.NVarChar, 100).Value = usuario.NombreUsuario.Trim();
            command.Parameters.Add("@password_hash", SqlDbType.VarChar, 255).Value =
                string.IsNullOrWhiteSpace(nuevoPasswordHash) ? DBNull.Value : nuevoPasswordHash;
            command.Parameters.Add("@rol_id", SqlDbType.TinyInt).Value = usuario.RolId;
            command.Parameters.Add("@nombre", SqlDbType.NVarChar, 120).Value = usuario.Nombre.Trim();
            command.Parameters.Add("@activo", SqlDbType.Bit).Value = usuario.Activo;
            command.Parameters.Add("@codigo_vendedor", SqlDbType.VarChar, 24).Value =
                string.IsNullOrWhiteSpace(usuario.CodigoVendedor) ? DBNull.Value : usuario.CodigoVendedor.Trim();
            command.Parameters.Add("@zona_ventas", SqlDbType.NVarChar, 80).Value =
                string.IsNullOrWhiteSpace(usuario.ZonaVentas) ? DBNull.Value : usuario.ZonaVentas.Trim();

            await connection.OpenAsync();

            try
            {
                var resultado = await command.ExecuteScalarAsync();
                bool actualizado = resultado != null && (bool)resultado;
                return actualizado ? ResultadoOperacionUsuario.Exito : ResultadoOperacionUsuario.NoEncontrado;
            }
            catch (SqlException ex) when (ex.Number is CodigosErrorSql.RestriccionUnica or CodigosErrorSql.IndiceUnico)
            {
                if (ex.Message.Contains("UQ_VENDEDOR_codigo", StringComparison.OrdinalIgnoreCase))
                    return ResultadoOperacionUsuario.CodigoVendedorDuplicado;
                return ResultadoOperacionUsuario.UsuarioDuplicado;
            }
        }

        public async Task<bool> CambiarEstado(int id, bool activo)
        {
            using var connection = _conexion.ObtenerConexion();
            using var command = new SqlCommand("seguridad.sp_CambiarEstadoUsuario", connection);

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@usuario_id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@activo", SqlDbType.Bit).Value = activo;

            await connection.OpenAsync();
            var resultado = await command.ExecuteScalarAsync();
            return resultado != null && (bool)resultado;
        }

        public async Task AsegurarUsuarioAdmin(IPasswordHasher<Usuario> hasher)
        {
            using var connection = _conexion.ObtenerConexion();
            await connection.OpenAsync();

            using var cmdCheck = new SqlCommand("SELECT COUNT(1) FROM seguridad.USUARIO;", connection);
            int totalUsuarios = (int)await cmdCheck.ExecuteScalarAsync();

            if (totalUsuarios == 0)
            {
                var nuevoUsuario = new Usuario();
                string hash = hasher.HashPassword(nuevoUsuario, "Admin123!");

                using var command = new SqlCommand(
                    "INSERT INTO seguridad.USUARIO (nombre_usuario, password_hash, rol_id, nombre, activo) " +
                    "VALUES (@nombre_usuario, @password_hash, 1, @nombre, 1);", connection);
                command.Parameters.Add("@nombre_usuario", SqlDbType.NVarChar, 100).Value = "admin";
                command.Parameters.Add("@password_hash", SqlDbType.VarChar, 255).Value = hash;
                command.Parameters.Add("@nombre", SqlDbType.NVarChar, 120).Value = "Administrador";

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
