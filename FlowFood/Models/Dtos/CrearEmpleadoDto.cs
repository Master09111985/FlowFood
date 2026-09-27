using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class CrearEmpleadoDto
  {
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150, MinimumLength = 3)]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria")]
    public string Direccion { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [Phone(ErrorMessage = "El formato de teléfono no es válido")]
    public string Telefono { get; set; }

    [Required(ErrorMessage = "La edad es obligatoria")]
    public string Edad { get; set; }

    [Required(ErrorMessage = "El salario semanal es obligatorio")]
    [Range(0, double.MaxValue, ErrorMessage = "El salario no puede ser negativo")]
    public decimal SalarioSemanal { get; set; }

    [Required(ErrorMessage = "La fecha de contrato es obligatoria")]
    public DateTime FechaContrato { get; set; }

    [Required(ErrorMessage = "El puesto es obligatorio")]
    public int PuestoId { get; set; }

    [Required(ErrorMessage = "La foto es obligatoria")]
    public IFormFile Foto { get; set; }

    // Codigo, FechaRegistro, Estado y FotoUrl NO se piden al cliente:
    // se generan/asignan en el backend
  }
}
