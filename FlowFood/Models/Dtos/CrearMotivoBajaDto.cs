using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearMotivoBajaDto
  {
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 3)]
    public string Nombre { get; set; }
    [MaxLength(200)]
    public string? Descripcion { get; set; }
    [Required]
    public bool Estado { get; set; }
  }
}
