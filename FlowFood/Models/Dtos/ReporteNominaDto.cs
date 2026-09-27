namespace FlowFood.Models.Dtos
{
  public class ReporteNominaDto
  {
    public int EmpleadoId { get; set; }
    public string NombreEmpleado { get; set; }
    public decimal SalarioSemanal { get; set; }
    public decimal PagoPorMinuto { get; set; }
    public int TotalMinutosTrabajados { get; set; }
    public decimal TotalAPagar { get; set; }
    public int TotalAsistencias { get; set; }
  }
}
