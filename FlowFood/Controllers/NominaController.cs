using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class NominaController : ControllerBase
  {
    private readonly INominaRepositorio _nominaRepo;

    public NominaController(INominaRepositorio nominaRepo)
    {
      _nominaRepo = nominaRepo;
    }

    [HttpPost("Generar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerarNomina([FromBody] RangoFechaDto fechas)
    {
      if (fechas == null || fechas.FechaInicio > fechas.FechaFin)
      {
        return BadRequest(new { Mensaje = "El rango de fechas seleccionado no es válido." });
      }

      var resultado = await _nominaRepo.GenerarReporteNominaAsync(fechas.FechaInicio, fechas.FechaFin);

      return Ok(resultado);
    }
  }
}
