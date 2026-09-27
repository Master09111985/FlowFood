using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/asistencia")]
  [ApiController]
  public class AsistenciaController : ControllerBase
  {
    private readonly IAsistenciaRepositorio _asistenciaRepo;

    public AsistenciaController(IAsistenciaRepositorio asistenciaRepo)
    {
      _asistenciaRepo = asistenciaRepo;
    }

    // ==========================================
    // POST: Registrar Checada de Empleado
    // ==========================================
    [HttpPost("registrar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarChecada([FromBody] RegistrarChecadaDto dto)
    {
      if (dto == null || string.IsNullOrWhiteSpace(dto.Codigo))
      {
        return BadRequest(new { Mensaje = "El código del empleado no puede estar vacío." });
      }

      // Llamamos al repositorio que hace toda la magia y la lógica de los 4 turnos
      var respuesta = await _asistenciaRepo.RegistrarChecadaAsync(dto.Codigo);

      // Si el repositorio detectó un código inválido o un empleado inactivo
      if (respuesta.Mensaje.StartsWith("Error"))
      {
        return BadRequest(respuesta); // Manda un HTTP 400 para que el ToastService en Angular lo pinte de Rojo
      }

      // Si el empleado ya checó sus 4 veces del día, lo bloqueamos
      if (respuesta.Mensaje.StartsWith("Atención"))
      {
        return BadRequest(respuesta); // Manda un HTTP 400 con la advertencia
      }

      // Si todo salió bien, regresa un HTTP 200 (Éxito) para el Toast verde
      return Ok(respuesta);
    }
  }
}
