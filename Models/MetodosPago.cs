using System;
using System.Collections.Generic;

namespace proeva.Models;

public partial class MetodosPago
{
    public int Id { get; set; }

    public string Metodo { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
