using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class CajaTurno
  {
    [Key]
    public int Id { get; set; }

    public int UsuarioCajeroId { get; set; }
    [ForeignKey("UsuarioCajeroId")]
    public Usuario Cajero { get; set; }

    public decimal FondoInicial { get; set; }

    public DateTime FechaApertura { get; set; }

    public bool EstaAbierta { get; set; } = true;

    // Datos del Cierre
    public DateTime? FechaCierre { get; set; }

    public int? UsuarioSupervisorId { get; set; }
    [ForeignKey("UsuarioSupervisorId")]
    public Usuario Supervisor { get; set; }

    public decimal EfectivoReportado { get; set; } // Lo que el supervisor contó físicamente
    public decimal EfectivoCalculado { get; set; } // Lo que el sistema dice que debería haber
    public decimal Diferencia { get; set; } // Faltante o Sobrante
  }
}
