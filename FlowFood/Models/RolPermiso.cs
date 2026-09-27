using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlowFood.Models
{
  public class RolPermiso
  {
    [Key]
    public int Id { get; set; }

    // Relacion con Rol
    [Required]
    public int rolId { get; set; }
    [ForeignKey("rolId")]
    public Rol Rol { get; set; }

    // Relacion con Permiso
    [Required]
    public int permisoId { get; set; }
    [ForeignKey("permisoId")]
    public Permiso Permiso { get; set; }

    [Required]
    public bool Habilitado { get; set; }
  }
}
