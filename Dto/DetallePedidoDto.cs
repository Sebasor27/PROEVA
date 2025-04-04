using System.ComponentModel.DataAnnotations;
namespace proeva.Dto;
public class DetallePedidoDto
{
    public int ProductoId { get; set; }
    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}