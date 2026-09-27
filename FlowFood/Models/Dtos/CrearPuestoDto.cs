using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearPuestoDto
  {
    [Required(ErrorMessage = "El nombre del puesto es obligatorio.")]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "El estado del puesto es obligatorio.")]
    public bool Estado { get; set; }
  }
}
