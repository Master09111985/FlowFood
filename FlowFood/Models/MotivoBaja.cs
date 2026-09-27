using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models
{
  public class MotivoBaja
  {
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; }
    [MaxLength(200)]
    public string Descripcion { get; set; }
    [Required]
    public bool Estado { get; set; }
  }
}
