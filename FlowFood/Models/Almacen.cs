using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models
{
  public class Almacen
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    public string? Descripcion { get; set; }
    [Required]
    public bool Estado { get; set; }
  }
}
