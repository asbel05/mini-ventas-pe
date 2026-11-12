using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniVentas.Data;
using MiniVentas.Models;
using MiniVentas.Models.Enums;
using MiniVentas.ViewModels.Productos;

namespace MiniVentas.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly ProductoDao _productoDao;
        public ProductoController(ProductoDao productoDao)
        {
            _productoDao = productoDao;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? busqueda, bool? soloActivos)
        {
            var productos = await _productoDao.Listar(busqueda, soloActivos);
            var vm = new ProductoIndexViewModel
            {
                Busqueda = busqueda,
                SoloActivos = soloActivos,
                Producto = productos
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View("Formulario", new ProductoFormViewModel { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProductoFormViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View("Formulario", modelo);

            var producto = new Producto
            {
                Sku = modelo.Sku,
                Nombre = modelo.Nombre,
                PrecioVenta = modelo.PrecioVenta!.Value,
                Activo = modelo.Activo
            };

            var resultado = await _productoDao.Crear(producto);
            if (resultado == ResultadoOperacionProducto.SkuDuplicado)
            {
                ModelState.AddModelError(nameof(modelo.Sku), "El código SKU ya se encuentra registrado en otro producto.");
                return View("Formulario", modelo);
            }

            TempData["Exito"] = $"Producto \"{producto.Nombre.Trim()}\" creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _productoDao.ObtenerPorId(id);
            if (producto == null)
                return NotFound();

            var modelo = new ProductoFormViewModel
            {
                ProductoId = producto.ProductoId,
                Sku = producto.Sku,
                Nombre = producto.Nombre,
                PrecioVenta = producto.PrecioVenta,
                Activo = producto.Activo
            };
            return View("Formulario", modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, ProductoFormViewModel modelo)
        {
            if (id != modelo.ProductoId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("Formulario", modelo);

            var producto = new Producto
            {
                ProductoId = modelo.ProductoId,
                Sku = modelo.Sku,
                Nombre = modelo.Nombre,
                PrecioVenta = modelo.PrecioVenta!.Value,
                Activo = modelo.Activo
            };

            var resultado = await _productoDao.Actualizar(producto);
            if (resultado == ResultadoOperacionProducto.NoEncontrado)
                return NotFound();

            if (resultado == ResultadoOperacionProducto.SkuDuplicado)
            {
                ModelState.AddModelError(nameof(modelo.Sku), "El código SKU ya pertenece a otro producto.");
                return View("Formulario", modelo);
            }

            TempData["Exito"] = $"Producto \"{producto.Nombre.Trim()}\" actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, bool? activo, string? busqueda, bool? soloActivos)
        {
            if (id <= 0 || activo is null || !ModelState.IsValid)
                return BadRequest();

            bool actualizado = await _productoDao.CambiarEstado(id, activo.Value);
            if (!actualizado)
                return NotFound();

            if (activo.Value)
            {
                TempData["Exito"] = "Producto activado correctamente.";
            }
            else
            {
                TempData["Aviso"] = "Producto desactivado.";
            }
            return RedirectToAction(nameof(Index), new { busqueda, soloActivos });
        }
    }
}
