using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearProveedorDto
  {
    [Required(ErrorMessage = "El nombre del proveedor es obligatorio")]
    public string Nombre { get; set; }
    public string Comentario { get; set; }
    [Required(ErrorMessage = "El estado del proveedor es obligatorio")]
    public bool Estado { get; set; }
  }
}
