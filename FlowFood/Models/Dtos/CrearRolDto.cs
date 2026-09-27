using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearRolDto
  {
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El campo Categoria es obligatorio")]
    public string Categoria { get; set; }

    [Required(ErrorMessage = "El campo Funcion es obligatorio")]
    public string Funcion { get; set; }
  }
}
