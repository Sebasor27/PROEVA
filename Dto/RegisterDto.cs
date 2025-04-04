using System.ComponentModel.DataAnnotations;

namespace proeva.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Nombre { get; set; } = null!;

        [Required, EmailAddress]
        public string Correo { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        public string Contrasena { get; set; } = null!;

        [DataType(DataType.Password), Compare(nameof(Contrasena))]
        public string ConfirmarContrasena { get; set; } = null!;
    }
}