using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class Empleado
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nombre { get; set; }
    [Required]
    public string Direccion { get; set; }
    [Required]
    public string Telefono { get; set; }
    [Required]
    public string Edad { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SalarioSemanal { get; set; }
    [Required]
    public string Codigo { get; set; }
    [Required]
    public DateTime FechaContrato { get; set; }
    [Required]
    public DateTime FechaRegistro { get; set; }
    [Required]
    public string FotoUrl { get; set; }
    [Required]
    public bool Estado { get; set; }

    // Relacion con Puesto
    [Required]
    public int puestoId { get; set; }
    [ForeignKey("puestoId")]
    public Puesto Puesto { get; set; }
  }
}
