using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class ComandaDetalle
  {
    [Key]
    public int Id { get; set; }

    public int NumeroPlato { get; set; } = 1;

    [Required]
    public int ComandaId { get; set; }
    [ForeignKey("ComandaId")]
    public Comanda Comanda { get; set; }

    [Required]
    public int PlatilloId { get; set; }
    [ForeignKey("PlatilloId")]
    public Platillo Platillo { get; set; }

    [Required]
    public int Cantidad { get; set; }

    [Required]
    public decimal PrecioUnitario { get; set; }

    [Required]
    public decimal Subtotal { get; set; }

    [MaxLength(200)]
    public string? Notas { get; set; } // Ej: "Sin cebolla", "Aderezo extra"
  }
}
