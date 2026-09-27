using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearProductoDto
  {
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 150 caracteres")]
    public string Nombre { get; set; }

    public string Descripcion { get; set; }

    [Required]
    public int unidadId { get; set; } 
  }
}
