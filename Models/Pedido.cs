using System;
using System.Collections.Generic;

namespace proeva.Models;

public partial class Pedido
{
    public int Id { get; set; }

    public DateTime? Fecha { get; set; }

    public int ClienteId { get; set; }

    public int MetodoPagoId { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Cliente Cliente { get; set; } = null!;

    public virtual ICollection<DetallesPedido> DetallesPedidos { get; set; } = new List<DetallesPedido>();

    public virtual MetodosPago MetodoPago { get; set; } = null!;
}
