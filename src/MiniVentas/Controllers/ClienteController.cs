using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniVentas.Data;
using MiniVentas.Models;
using MiniVentas.Models.Enums;
using MiniVentas.ViewModels.Clientes;

namespace MiniVentas.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly ClienteDao _clienteDao;
        public ClienteController(ClienteDao clienteDao)
        {
            _clienteDao = clienteDao;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? busqueda, string? tipoDocumento)
        {
            var clientes = await _clienteDao.Listar(busqueda, tipoDocumento);
            var vm = new ClienteIndexViewModel
            {
                Busqueda = busqueda,
                TipoDocumento = tipoDocumento,
                Clientes = clientes
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View("Formulario", new ClienteFormViewModel { TipoDocumento = "1" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ClienteFormViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View("Formulario", modelo);

            var cliente = new Cliente
            {
                TipoDocumento = modelo.TipoDocumento,
                NumeroDocumento = modelo.NumeroDocumento,
                Nombre = modelo.Nombre,
                OrigenDatos = "MANUAL"
            };

            var resultado = await _clienteDao.Crear(cliente);
            if (resultado == ResultadoOperacionCliente.DocumentoDuplicado)
            {
                ModelState.AddModelError(nameof(modelo.NumeroDocumento), "Este tipo y número de documento ya se encuentra registrado.");
                return View("Formulario", modelo);
            }

            TempData["Exito"] = $"Cliente \"{cliente.Nombre.Trim().ToUpperInvariant()}\" registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var cliente = await _clienteDao.ObtenerPorId(id);
            if (cliente == null)
                return NotFound();

            var modelo = new ClienteFormViewModel
            {
                ClienteId = cliente.ClienteId,
                TipoDocumento = cliente.TipoDocumento,
                NumeroDocumento = cliente.NumeroDocumento,
                Nombre = cliente.Nombre
            };
            return View("Formulario", modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, ClienteFormViewModel modelo)
        {
            if (id != modelo.ClienteId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("Formulario", modelo);

            var cliente = new Cliente
            {
                ClienteId = modelo.ClienteId,
                TipoDocumento = modelo.TipoDocumento,
                NumeroDocumento = modelo.NumeroDocumento,
                Nombre = modelo.Nombre
            };

            var resultado = await _clienteDao.Actualizar(cliente);
            if (resultado == ResultadoOperacionCliente.NoEncontrado)
                return NotFound();

            if (resultado == ResultadoOperacionCliente.DocumentoDuplicado)
            {
                ModelState.AddModelError(nameof(modelo.NumeroDocumento), "Este tipo y número de documento ya pertenece a otro cliente.");
                return View("Formulario", modelo);
            }

            TempData["Exito"] = $"Cliente \"{cliente.Nombre.Trim().ToUpperInvariant()}\" actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
