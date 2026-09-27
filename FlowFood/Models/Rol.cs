using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models
{
  public class Rol
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Categoria { get; set; }
    [Required]
    public string Funcion { get; set; }
  }
}
