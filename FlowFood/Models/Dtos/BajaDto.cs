namespace FlowFood.Models.Dtos
{
  public class BajaDto
  {
    public int Id { get; set; }
    public decimal Cantidad { get; set; }
    public DateTime FechaBaja { get; set; }
    public string Comentarios { get; set; }

    public int InventarioId { get; set; }
    public string ProductoNombre { get; set; }
    public string AlmacenNombre { get; set; }
    public string UnidadMedidaNombre { get; set; }

    public int MotivoBajaId { get; set; }
    public string MotivoBajaNombre { get; set; }
  }
}
