using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models
{
  public class Plataforma
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public bool Estado { get; set; }
  }
}
