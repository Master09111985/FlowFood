using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CerrarTurnoDto
  {
    [Required]
    public int TurnoId { get; set; }

    [Required]
    public decimal EfectivoReportado { get; set; } // Lo que el cajero/supervisor contó físicamente

    // Credenciales del Supervisor
    [Required]
    public string SupervisorUsuario { get; set; }
    [Required]
    public string SupervisorPassword { get; set; }
  }
}
