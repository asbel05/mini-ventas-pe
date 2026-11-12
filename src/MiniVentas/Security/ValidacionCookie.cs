using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MiniVentas.Data;
using MiniVentas.Models;
using System.Security.Claims;

namespace MiniVentas.Security
{
    public class ValidacionCookie : CookieAuthenticationEvents
    {
        private readonly UsuarioDao _usuarioDao;

        public ValidacionCookie(UsuarioDao usuarioDao)
        {
            _usuarioDao = usuarioDao;
        }

        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var principal = context.Principal;
            Usuario? usuario = null;

            if (int.TryParse(principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
            {
                usuario = await _usuarioDao.ObtenerPorId(id);
            }

            if (usuario is not { Activo: true })
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(context.Scheme.Name);
                return;
            }

            if (principal?.FindFirstValue(ClaimTypes.Name) == usuario.NombreUsuario &&
                principal?.FindFirstValue("NombreCompleto") == usuario.Nombre &&
                principal?.FindFirstValue(ClaimTypes.Role) == usuario.RolCodigo &&
                principal?.FindFirstValue("VendedorId") == usuario.VendedorId?.ToString())
            {
                return;
            }

            context.ReplacePrincipal(SesionUsuario.CrearPrincipal(usuario));
            context.ShouldRenew = true;
        }
    }
}

