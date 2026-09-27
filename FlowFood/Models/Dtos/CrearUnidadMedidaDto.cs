using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearUnidadMedidaDto
  {
    [Required(ErrorMessage = "El nombre de la unidad de medida es obligatorio.")]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "El estado de la unidad de medida es obligatorio.")]
    public bool Estado { get; set; }
  }
}
