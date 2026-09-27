using System;
using System.ComponentModel.DataAnnotations;

namespace FlowFood.Models.Dtos
{
  public class RegistrarChecadaDto
  {
    [Required(ErrorMessage = "El código del empleado es obligatorio.")]
    public string Codigo { get; set; }
  }
}
