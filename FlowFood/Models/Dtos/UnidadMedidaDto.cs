using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class UnidadMedidaDto
  {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public bool Estado { get; set; }
  }
}
