using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniVentas.Data;
using MiniVentas.Models;
using MiniVentas.Security;
using MiniVentas.ViewModels.Acceso;

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
            var principal = SesionUsuario.CrearPrincipal(usuario);
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

        [HttpGet]
        [Authorize]
        public IActionResult Denegado()
        {
            return View();
        }
    }
}