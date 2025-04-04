using proeva.Dto;
using proeva.ViewModels;

namespace proeva.Services;

public interface IVentaService
{
    Task<VentaViewModel> PrepararVentaViewModel();
    Task<PedidoDto> ProcesarVenta(PedidoDto pedidoDto);
    Task<List<ProductoDto>> BuscarProductos(string term);
    Task<List<ClienteDto>> BuscarClientes(string term);
    Task<ClienteDto> ObtenerCliente(int id);
}