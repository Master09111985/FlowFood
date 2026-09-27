using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearCategoriaDto
  {
    [Required(ErrorMessage = "El nombre de la categoria es obligatorio.")]
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    [Required(ErrorMessage = "El estado de la categoria es obligatorio.")]
    public bool Estado { get; set; }
  }
}
