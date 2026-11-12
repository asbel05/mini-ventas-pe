using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniVentas.Data;
using MiniVentas.Models;
using MiniVentas.Models.Enums;
using MiniVentas.ViewModels.Usuarios;
using System.Security.Claims;

namespace MiniVentas.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class UsuarioController : Controller
    {
        private readonly UsuarioDao _usuarioDao;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        public UsuarioController(UsuarioDao usuarioDao, IPasswordHasher<Usuario> passwordHasher)
        {
            _usuarioDao = usuarioDao;
            _passwordHasher = passwordHasher;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? busqueda, byte? rolId, bool? soloActivos)
        {
            var usuarios = await _usuarioDao.Listar(busqueda, rolId, soloActivos);
            var vm = new UsuarioIndexViewModel
            {
                Busqueda = busqueda,
                RolId = rolId,
                SoloActivos = soloActivos,
                Usuarios = usuarios
            };
            return View(vm);
        }
        [HttpGet]
        public IActionResult Crear()
        {
            return View("Formulario", new UsuarioFormViewModel
            {
                RolId = 2,
                Activo = true,
                ZonaVentas = "SEDE PRINCIPAL"
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(UsuarioFormViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View("Formulario", modelo);
            var usuario = new Usuario
            {
                NombreUsuario = modelo.NombreUsuario.Trim().ToLowerInvariant(),
                Nombre = modelo.Nombre.Trim(),
                RolId = modelo.RolId!.Value,
                Activo = modelo.Activo,
                CodigoVendedor = modelo.CodigoVendedor,
                ZonaVentas = modelo.ZonaVentas
            };
            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, modelo.Password!);
            var resultado = await _usuarioDao.Crear(usuario);
            if (resultado == ResultadoOperacionUsuario.UsuarioDuplicado)
            {
                ModelState.AddModelError(nameof(modelo.NombreUsuario), "El nombre de usuario ya está en uso.");
                return View("Formulario", modelo);
            }
            if (resultado == ResultadoOperacionUsuario.CodigoVendedorDuplicado)
            {
                ModelState.AddModelError(nameof(modelo.CodigoVendedor), "El código de vendedor ya pertenece a otro usuario.");
                return View("Formulario", modelo);
            }
            TempData["Exito"] = $"Usuario \"{usuario.NombreUsuario}\" creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _usuarioDao.ObtenerPorId(id);
            if (usuario == null)
                return NotFound();
            var modelo = new UsuarioFormViewModel
            {
                UsuarioId = usuario.UsuarioId,
                NombreUsuario = usuario.NombreUsuario,
                Nombre = usuario.Nombre,
                RolId = usuario.RolId,
                Activo = usuario.Activo,
                CodigoVendedor = usuario.CodigoVendedor,
                ZonaVentas = usuario.ZonaVentas
            };
            return View("Formulario", modelo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, UsuarioFormViewModel modelo)
        {
            if (id != modelo.UsuarioId)
                return BadRequest();
            if (!modelo.Activo && EsUsuarioActual(id))
            {
                ModelState.AddModelError(string.Empty, "No puedes desactivar tu propia cuenta mientras tienes sesión iniciada.");
            }
            if (!ModelState.IsValid)
                return View("Formulario", modelo);
            var usuario = new Usuario
            {
                UsuarioId = modelo.UsuarioId,
                NombreUsuario = modelo.NombreUsuario.Trim().ToLowerInvariant(),
                Nombre = modelo.Nombre.Trim(),
                RolId = modelo.RolId!.Value,
                Activo = modelo.Activo,
                CodigoVendedor = modelo.CodigoVendedor,
                ZonaVentas = modelo.ZonaVentas
            };
            string? nuevoHash = null;
            if (!string.IsNullOrEmpty(modelo.Password))
            {
                nuevoHash = _passwordHasher.HashPassword(usuario, modelo.Password);
            }
            var resultado = await _usuarioDao.Actualizar(usuario, nuevoHash);
            if (resultado == ResultadoOperacionUsuario.NoEncontrado)
                return NotFound();
            if (resultado == ResultadoOperacionUsuario.UsuarioDuplicado)
            {
                ModelState.AddModelError(nameof(modelo.NombreUsuario), "El nombre de usuario ya pertenece a otra cuenta.");
                return View("Formulario", modelo);
            }
            if (resultado == ResultadoOperacionUsuario.CodigoVendedorDuplicado)
            {
                ModelState.AddModelError(nameof(modelo.CodigoVendedor), "El código de vendedor ya pertenece a otro usuario.");
                return View("Formulario", modelo);
            }
            TempData["Exito"] = $"Usuario \"{usuario.NombreUsuario}\" actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, bool? activo, string? busqueda, byte? rolId, bool? soloActivos)
        {
            if (id <= 0 || activo is null || !ModelState.IsValid)
                return BadRequest();
            if (!activo.Value && EsUsuarioActual(id))
            {
                TempData["Aviso"] = "No puedes desactivar tu propia cuenta en sesión.";
                return RedirectToAction(nameof(Index), new { busqueda, rolId, soloActivos });
            }
            bool actualizado = await _usuarioDao.CambiarEstado(id, activo.Value);
            if (!actualizado)
                return NotFound();
            if (activo.Value)
                TempData["Exito"] = "Usuario activado correctamente.";
            else
                TempData["Aviso"] = "Usuario desactivado.";
            return RedirectToAction(nameof(Index), new { busqueda, rolId, soloActivos });
        }
        private bool EsUsuarioActual(int usuarioId)
        {
            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimId, out int idActual) && idActual == usuarioId;
        }
    }
}