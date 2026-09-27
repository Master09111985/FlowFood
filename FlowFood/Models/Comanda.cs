using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class Comanda
  {
    [Key]
    public int Id { get; set; }

    // ---Campo para la prioridad de la comanda
    public int Prioridad { get; set; } = 0;

    // 1: Local, 2: Llevar, 3: Domicilio, 4: Agendado, 5: Plataforma
    [Required]
    public int TipoPedido { get; set; }

    // --- Campos condicionales según el tipo de pedido ---
    [MaxLength(20)]
    public string? NumeroMesa { get; set; } // Para tipo 1

    [MaxLength(100)]
    public string? NombreClienteLlevar { get; set; } // Para tipo 2

    public DateTime? FechaHoraAgendada { get; set; } // Para tipo 4

    // 0: Agendado, 1: Cocinando, 2: Entregado, 3: Pagado
    [Required]
    public int Estatus { get; set; }

    [Required]
    public decimal Subtotal { get; set; }

    [Required]
    public decimal Total { get; set; }

    [Required]
    public DateTime FechaRegistro { get; set; }

    // --- Relaciones ---

    public int? ClienteId { get; set; } // Para tipos 3 y 4 (Catálogo de clientes)
    [ForeignKey("ClienteId")]
    public Cliente Cliente { get; set; }

    public int? PlataformaId { get; set; } // Para tipo 5
    [ForeignKey("PlataformaId")]
    public Plataforma Plataforma { get; set; }

    public int? MetodoPagoId { get; set; }
    [ForeignKey("MetodoPagoId")]
    public MetodoPago MetodoPago { get; set; }

    // Relación uno a muchos con los Detalles de la comanda (Los platillos)
    public ICollection<ComandaDetalle> Detalles { get; set; }
  }
}
