using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class Producto
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    [Required]
    public DateTime FechaRegistro { get; set; }
    [Required]
    public bool Estado { get; set; }

    // Relacion con unidades
    [Required]
    public int unidadId { get; set; }
    [ForeignKey("unidadId")]
    public UnidadMedida UnidadMedida { get; set; }
  }
}
