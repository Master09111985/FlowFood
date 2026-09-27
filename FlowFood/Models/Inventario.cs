using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class Inventario
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public decimal Cantidad { get; set; }

    // Relacion con Almacen
    [Required]
    public int almacenId { get; set; }
    [ForeignKey("almacenId")]
    public Almacen Almacen { get; set; }

    // Relacion con Producto
    [Required]
    public int productoId { get; set; }
    [ForeignKey("productoId")]
    public Producto Producto { get; set; }
  }
}
