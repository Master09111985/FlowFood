using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class Usuario
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public DateTime FechaRegistro { get; set; }
    [Required]
    public bool Estado { get; set; }

    // Relacion con roles
    [Required]
    public int rolId { get; set; }
    [ForeignKey("rolId")]
    public Rol Rol { get; set; }

    // Relacion con empleado
    [Required]
    public int empleadoId { get; set; }
    [ForeignKey("empleadoId")]
    public Empleado Empleado { get; set; }
  }
}
