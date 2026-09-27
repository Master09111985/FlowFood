using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearComandaDetalleDto
  {
    [Required]
    public int PlatilloId { get; set; }
    public int NumeroPlato { get; set; }
    [Required]
    [Range(1, 100)]
    public int Cantidad { get; set; }
    [Required]
    public decimal PrecioUnitario { get; set; }
    public string? Notas { get; set; }
  }
}
