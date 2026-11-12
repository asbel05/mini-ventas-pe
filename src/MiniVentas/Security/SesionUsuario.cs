using Microsoft.AspNetCore.Authentication.Cookies;
using MiniVentas.Models;
using System.Security.Claims;

namespace MiniVentas.Security
{
    public static class SesionUsuario
    {
        public static ClaimsPrincipal CrearPrincipal(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim("NombreCompleto", usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.RolCodigo)
            };

            if (usuario.VendedorId.HasValue)
            {
                claims.Add(new Claim("VendedorId", usuario.VendedorId.Value.ToString()));
            }

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identidad);
        }
    }
}

