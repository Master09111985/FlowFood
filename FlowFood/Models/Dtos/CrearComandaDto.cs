using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearComandaDto
  {
    [Required(ErrorMessage = "El tipo de pedido es obligatorio")]
    [Range(1, 5, ErrorMessage = "El tipo de pedido debe ser entre 1 y 5")]
    public int TipoPedido { get; set; }

    public string? NumeroMesa { get; set; }
    public string? NombreClienteLlevar { get; set; }
    public int? ClienteId { get; set; }
    public DateTime? FechaHoraAgendada { get; set; }
    public int? PlataformaId { get; set; }


    [Required(ErrorMessage = "La comanda debe contener al menos un detalle")]
    public List<CrearComandaDetalleDto> Detalles { get; set; }
  }
}
