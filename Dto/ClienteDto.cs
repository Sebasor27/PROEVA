using System.ComponentModel.DataAnnotations;

namespace proeva.Dto
{
    public class ClienteDto
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Direccion { get; set; }
        [Required]
        [EmailAddress]
        public string Correo { get; set; }
    }
}