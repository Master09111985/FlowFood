using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class MetodoPagoDto
  {
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;
    public bool Estado { get; set; }
  }
}
