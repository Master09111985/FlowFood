namespace FlowFood.Models.Dtos
{
  public class RolPermisoDto
  {
    public int Id { get; set; }
    public int RolId { get; set; }
    public int PermisoId { get; set; }
    public string PermisoNombre { get; set; }
    public string PermisoDescripcion { get; set; }
    public bool Habilitado { get; set; }
  }
}
