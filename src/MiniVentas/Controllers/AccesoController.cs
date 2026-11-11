using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniVentas.Data;
using MiniVentas.Models;
using MiniVentas.ViewModels;
using System.Security.Claims;

namespace MiniVentas.Controllers
{
    public class AccesoController : Controller
    {
        private readonly UsuarioDao _usuarioDao;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        public AccesoController(UsuarioDao usuarioDao, IPasswordHasher<Usuario> passwordHasher)
        {
            _usuarioDao = usuarioDao;
            _passwordHasher = passwordHasher;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult IniciarSesion(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarSesion(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);
            var usuario = await _usuarioDao.ObtenerPorNombreUsuario(modelo.NombreUsuario.Trim());
            if (usuario == null || !usuario.Activo)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(modelo);
            }
            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, modelo.Password);
            if (resultado == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                return View(modelo);
            }
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
            var principal = new ClaimsPrincipal(identidad);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            if (!string.IsNullOrEmpty(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
                return Redirect(modelo.ReturnUrl);
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("IniciarSesion", "Acceso");
        }
    }
}