using System.ComponentModel.DataAnnotations;

namespace proeva.Dto
{
    public class ProductoDto
    {
        public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Precio { get; set; }
    }
}