namespace FlowFood.Models.Dtos
{
  public class ProductoDto
  {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }

    public int unidadId { get; set; }
    public string unidadNombre { get; set; }

    public DateTime FechaRegistro { get; set; }
    public bool Estado { get; set; }
  }
}
