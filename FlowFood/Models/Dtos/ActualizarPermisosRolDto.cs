using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class ActualizarPermisosRolDto
  {
    [Required(ErrorMessage = "El rol es obligatorio")]
    public int RolId { get; set; }

    [Required(ErrorMessage = "Debe enviar al menos un permiso")]
    public List<ActualizarPermisoDto> Permisos { get; set; }
  }
}
