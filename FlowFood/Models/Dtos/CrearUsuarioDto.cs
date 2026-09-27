using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearUsuarioDto
  {
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio")]
    public int RolId { get; set; }

    [Required(ErrorMessage = "El empleado es obligatorio")]
    public int EmpleadoId { get; set; }
  }
}
