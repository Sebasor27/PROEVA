using Microsoft.AspNetCore.Mvc;
using proeva.Dto;
using proeva.Services;
using proeva.ViewModels;

[Route("Ventas")]
public class VentasController : Controller
{
    private readonly IVentaService _ventaService;

    public VentasController(IVentaService ventaService)
    {
        _ventaService = ventaService;
    }

    [HttpGet("")]  
    [HttpGet("Index")]  
    public async Task<IActionResult> Index()
    {
        var viewModel = await _ventaService.PrepararVentaViewModel();
        return View(viewModel);
    }

    [HttpPost("ProcesarVenta")]  
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcesarVenta([FromBody] PedidoDto pedidoDto)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await _ventaService.PrepararVentaViewModel();
            viewModel.Pedido = pedidoDto;
            return View("Index", viewModel);
        }

        try
        {
            var resultado = await _ventaService.ProcesarVenta(pedidoDto);
            return Json(new { success = true, pedidoId = resultado.Id });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("BuscarProductos")]  
    public async Task<IActionResult> BuscarProductos([FromQuery] string term)
    {
        var productos = await _ventaService.BuscarProductos(term);
        return Json(productos);
    }

    [HttpGet("BuscarClientes")]  
    public async Task<IActionResult> BuscarClientes([FromQuery] string term)
    {
        var clientes = await _ventaService.BuscarClientes(term);
        return Json(clientes);
    }

    [HttpGet("ObtenerCliente/{id:int}")]  
    public async Task<IActionResult> ObtenerCliente(int id)
    {
        var cliente = await _ventaService.ObtenerCliente(id);
        return Json(cliente);
    }
}