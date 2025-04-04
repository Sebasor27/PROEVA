using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using proeva.Dto;
using proeva.Services;

public class ProductosController : Controller
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    public async Task<IActionResult> Index()
    {
        var productos = await _productoService.ObtenerTodos();
        return View(productos);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] ProductoDto productoDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
            );

            return Json(new
            {
                success = false,
                message = "Error de validación",
                errors
            });
        }

        try
        {
            var productoCreado = await _productoService.Crear(productoDto);
            return Json(new
            {
                success = true,
                message = "Producto creado exitosamente",
                producto = productoCreado
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = "Error al crear el producto",
                exception = ex.Message
            });
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var producto = await _productoService.ObtenerPorId(id);
        if (producto == null)
        {
            return NotFound();
        }
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromForm] ProductoDto productoDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
            );

            return Json(new
            {
                success = false,
                message = "Error de validación",
                errors
            });
        }

        try
        {
            await _productoService.Actualizar(productoDto.Id, productoDto);
            return Json(new
            {
                success = true,
                message = "Producto actualizado exitosamente"
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = "Error al actualizar el producto",
                exception = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var producto = await _productoService.ObtenerPorId(id);
        if (producto == null)
        {
            return NotFound();
        }
        return Json(producto);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var producto = await _productoService.ObtenerPorId(id);
        if (producto == null)
        {
            return NotFound();
        }
        return View(producto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productoService.Eliminar(id);
        return RedirectToAction(nameof(Index));
    }
}