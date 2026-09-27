namespace FlowFood.Models.Dtos
{
  public class UsuarioDto
  {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Estado { get; set; }

    public int RolId { get; set; }
    public string RolNombre { get; set; }

    public int EmpleadoId { get; set; }
    public string EmpleadoNombre { get; set; }
  }
}
