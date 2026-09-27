using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class AbrirTurnoDto
  {
    [Required]
    public int UsuarioCajeroId { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "El fondo inicial no puede ser negativo.")]
    public decimal FondoInicial { get; set; }
  }
}
