using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearMetodoPagoDto
  {
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "El estado del metodo de pago es obligatorio.")]
    public bool Estado { get; set; }
  }
}
