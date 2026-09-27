using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class TrasladoInventarioDto
  {
    [Required]
    public int ProductoId { get; set; }
    [Required]
    public int AlmacenOrigenId { get; set; }
    [Required]
    public int AlmacenDestinoId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    public decimal Cantidad { get; set; }
  }
}
