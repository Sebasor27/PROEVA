using proeva.Dto;
using proeva.Models;

namespace proeva.ViewModels;
public class VentaViewModel
{
    public List<ProductoDto> ProductosDisponibles { get; set; }
    public List<ClienteDto> Clientes { get; set; }
    public List<MetodosPago> MetodosPago { get; set; }
    public PedidoDto Pedido { get; set; } = new PedidoDto();
}