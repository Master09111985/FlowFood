using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models
{
  public class Proveedor
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    public string? Comentario { get; set; }
    [Required]
    public bool Estado { get; set; }
  }
}
