using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class MovimientoCaja
  {
    [Key]
    public int Id { get; set; }

    public int CajaTurnoId { get; set; }
    [ForeignKey("CajaTurnoId")]
    public CajaTurno Turno { get; set; }

    // 1 = Venta (Entrada), 2 = Pago Proveedor (Salida)
    public int TipoMovimiento { get; set; }

    public decimal Monto { get; set; }
    public DateTime Fecha { get; set; }

    public int MetodoPagoId { get; set; }

    // Referencias Opcionales
    public int? ComandaId { get; set; }
    public int? ProveedorId { get; set; }

    public int? UsuarioAutorizaId { get; set; } // ID del supervisor si fue un pago a proveedor
  }
}
