using Microsoft.AspNetCore.Mvc;
using proeva.Dto;
using proeva.Services;

public class ClientesController : Controller
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    public async Task<IActionResult> Index()
    {
        var clientes = await _clienteService.ObtenerTodos();
        return View(clientes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromForm] ClienteDto clienteDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
            );
            
            return Json(new { 
                success = false, 
                message = "Error de validación",
                errors 
            });
        }

        try
        {
            var clienteCreado = await _clienteService.Crear(clienteDto);
            return Json(new { 
                success = true,
                message = "Cliente creado exitosamente",
                cliente = clienteCreado
            });
        }
        catch (Exception ex)
        {
            return Json(new { 
                success = false, 
                message = ex.Message,
                exception = ex.GetType().Name
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromForm] ClienteDto clienteDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault()
            );
            
            return Json(new { 
                success = false, 
                message = "Error de validación",
                errors 
            });
        }

        try
        {
            await _clienteService.Actualizar(clienteDto.Id, clienteDto);
            return Json(new { 
                success = true,
                message = "Cliente actualizado exitosamente"
            });
        }
        catch (Exception ex)
        {
            return Json(new { 
                success = false, 
                message = ex.Message,
                exception = ex.GetType().Name
            });
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _clienteService.Eliminar(id);
            return Json(new { 
                success = true,
                message = "Cliente eliminado exitosamente"
            });
        }
        catch (Exception ex)
        {
            return Json(new { 
                success = false, 
                message = ex.Message,
                exception = ex.GetType().Name
            });
        }
    }
}