using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class ActualizarPermisoDto
  {
    [Required(ErrorMessage = "El permiso es obligatorio")]
    public int PermisoId { get; set; }

    [Required(ErrorMessage = "El estado habilitado/deshabilitado es obligatorio")]
    public bool Habilitado { get; set; }
  }
}
