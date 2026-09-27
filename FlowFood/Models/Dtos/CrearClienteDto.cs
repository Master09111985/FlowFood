using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearClienteDto
  {
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "El telefono es obligatorio.")]
    public string Telefono { get; set; }
    public string Direccion { get; set; }
    public string Referencias { get; set; }
  }
}
