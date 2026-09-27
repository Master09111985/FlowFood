using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearAlmacenDto
  {
    [Required(ErrorMessage = "El nombre del almacen es obligatorio.")]
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    [Required(ErrorMessage = "El estado del almacen es obligatorio.")]
    public bool Estado { get; set; }
  }
}
