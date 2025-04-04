using System.ComponentModel.DataAnnotations;

namespace proeva.DTOs
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Correo { get; set; } = null!;

        [Required, DataType(DataType.Password)]
        public string Contrasena { get; set; } = null!;

        public bool RememberMe { get; set; }
    }
}