using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class Asistencia
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public int EmpleadoId { get; set; }

    [ForeignKey("EmpleadoId")]
    public Empleado Empleado { get; set; }

    [Required]
    public DateTime FechaHora { get; set; }

    [Required]
    // 1 = Entrada, 2 = Salida a Comida, 3 = Regreso de Comida, 4 = Salida Final
    public int TipoChecada { get; set; }
  }
}
