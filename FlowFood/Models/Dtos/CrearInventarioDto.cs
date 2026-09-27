using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearInventarioDto
  {
    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
    public decimal Cantidad { get; set; }

    [Required(ErrorMessage = "El producto es obligatorio")]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "El almacén es obligatorio")]
    public int AlmacenId { get; set; }

    [Required(ErrorMessage = "La unidad de medida es obligatoria")]
    public int UnidadMedidaId { get; set; }
  }
}
