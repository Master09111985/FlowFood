namespace FlowFood.Models.Dtos
{
  public class MapaPermisosDto
  {
    public int RolId { get; set; }
    public string RolNombre { get; set; }
    public Dictionary<string, bool> Permisos { get; set; }
  }
}
