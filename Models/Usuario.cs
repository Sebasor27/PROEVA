using System;
using System.Collections.Generic;

namespace proeva.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public int RolId { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual Role Rol { get; set; } = null!;
}
