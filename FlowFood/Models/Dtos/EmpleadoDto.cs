namespace FlowFood.Models.Dtos
{
  public class EmpleadoDto
  {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string Edad { get; set; }
    public decimal SalarioSemanal { get; set; }
    public string Codigo { get; set; }
    public DateTime FechaContrato { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string FotoUrl { get; set; }
    public bool Estado { get; set; }

    public int PuestoId { get; set; }
    public string PuestoNombre { get; set; }
  }
}
