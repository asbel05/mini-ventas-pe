using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using MiniVentas.Models;
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
