using System;
using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class RespuestaChecadaDto
  {
    public string NombreEmpleado { get; set; }
    public string NombreChecada { get; set; } // Ej. "Entrada", "Salida"
    public DateTime FechaHora { get; set; }
    public string Mensaje { get; set; }
  }
}
