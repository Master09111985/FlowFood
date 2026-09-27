using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models
{
  public class Cliente
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Telefono { get; set; }
    [Required]
    public string Direccion { get; set; }
    [Required]
    public string Referencias { get; set; }
  }
}
