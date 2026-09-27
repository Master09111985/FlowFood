using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class Baja
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public decimal Cantidad { get; set; }

    [Required]
    public DateTime FechaBaja { get; set; }
    public string? Comentarios { get; set; }


    [Required]
    public int InventarioId { get; set; }
    [ForeignKey("InventarioId")]
    public Inventario Inventario { get; set; }
    [Required]
    public int MotivoBajaId { get; set; }
    [ForeignKey("MotivoBajaId")]
    public MotivoBaja MotivoBaja { get; set; }
  }
}
