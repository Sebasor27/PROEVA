using System.ComponentModel.DataAnnotations;

namespace proeva.Dto;
public class PedidoDto
{
    public int Id { get; set; }
    [Required]
    public int ClienteId { get; set; }
    [Required]
    public int MetodoPagoId { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public List<DetallePedidoDto> Detalles { get; set; } = new List<DetallePedidoDto>();
}