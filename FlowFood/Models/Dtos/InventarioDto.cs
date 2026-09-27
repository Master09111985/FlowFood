namespace FlowFood.Models.Dtos
{
  public class InventarioDto
  {
    public int Id { get; set; }
    public decimal Cantidad { get; set; }

    public int AlmacenId { get; set; }
    public string AlmacenNombre { get; set; }

    public int ProductoId { get; set; }
    public string ProductoNombre { get; set; }

    public int UnidadMedidaId { get; set; }
    public string UnidadMedidaNombre { get; set; }
  }
}
