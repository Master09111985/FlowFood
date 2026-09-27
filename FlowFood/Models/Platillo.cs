using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models
{
    public class Platillo
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        [Required]
        public decimal Precio { get; set; }
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string FotoUrl { get; set; }
        [Required]
        public DateTime FechaRegistro { get; set; }
        [Required]
        public bool Estado { get; set; }
    }
}
