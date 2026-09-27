using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class PagoProveedorCajaDto
  {
    [Required]
    public int TurnoId { get; set; }

    [Required]
    public int ProveedorId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0.")]
    public decimal Monto { get; set; }

    // Credenciales del Supervisor
    [Required]
    public string SupervisorUsuario { get; set; }
    [Required]
    public string SupervisorPassword { get; set; }
  }
}
