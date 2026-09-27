using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models
{
  public class Permiso
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Descripcion { get; set; }
  }
}
