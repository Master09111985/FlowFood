using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
    public class CrearPlatilloDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres")]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La foto es obligatoria")]
        public IFormFile Foto { get; set; }
    }
}
