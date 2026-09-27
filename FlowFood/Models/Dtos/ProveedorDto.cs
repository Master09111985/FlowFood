using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class ProveedorDto
  {
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }
    public string? Comentario { get; set; }
    public bool Estado { get; set; }
  }
}
