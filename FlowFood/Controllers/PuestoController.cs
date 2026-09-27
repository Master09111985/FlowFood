using Microsoft.AspNetCore.Mvc;
using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;

namespace FlowFood.Controllers
{
  [Route("flowfood/puesto")]
  [ApiController]
  public class PuestoController : ControllerBase
  {
    private readonly IPuestoRepositorio _pRepo;

    public PuestoController(IPuestoRepositorio pRepo)
    {
      _pRepo = pRepo;
    }

    [HttpGet("listarpuestos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPuestos()
    {
      var listaPuestos = await _pRepo.GetPuestosAsync();
      var listaPuestosDto = listaPuestos.Select(p => new PuestoDto
      {
        Id = p.Id,
        Nombre = p.Nombre,
        Estado = p.Estado
      }).ToList();

      return Ok(listaPuestosDto);
    }

    [HttpGet("listarpuestosactivos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPuestosActivosAsync()
    {
      var listaPuestos = await _pRepo.GetPuestosAsync();
      var listaPuestosDto = listaPuestos.Where(p => p.Estado)
        .Select(p => new PuestoDto
        {
          Id = p.Id,
          Nombre = p.Nombre,
          Estado = p.Estado
        }).ToList();

      return Ok(listaPuestosDto);
    }

    [HttpPost("crearpuesto")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CrearPuesto([FromBody] CrearPuestoDto crearPuestoDto)
    {
      if (!ModelState.IsValid || crearPuestoDto == null)
        return BadRequest(ModelState);

      if (await _pRepo.ExistePuestoXNombreAsync(crearPuestoDto.Nombre))
      {
        ModelState.AddModelError("", "El puesto ya existe.");
        return StatusCode(400, ModelState);
      }

      var puesto = new Puesto
      {
        Nombre = crearPuestoDto.Nombre,
        Estado = crearPuestoDto.Estado
      };

      if (!await _pRepo.CrearPuestoAsync(puesto))
      {
        ModelState.AddModelError("", $"Algo salió mal guardando el registro: {puesto.Nombre}");
        return StatusCode(500, ModelState);
      }
      return Ok(puesto);
    }

    // MÉTODO CORREGIDO: Ahora sí actualiza un Puesto
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarPuesto(int id, [FromBody] PuestoDto puestoDto)
    {
      if (puestoDto == null || id != puestoDto.Id)
        return BadRequest(ModelState);

      if (!await _pRepo.ExistePuestoAsync(id))
        return NotFound();

      // Usamos los datos completos que vienen de Angular
      var puestoActualizar = new Puesto
      {
        Id = puestoDto.Id,
        Nombre = puestoDto.Nombre,
        Estado = puestoDto.Estado
      };

      if (!await _pRepo.ActualizarPuestoAsync(puestoActualizar))
      {
        ModelState.AddModelError("", $"Algo salió mal actualizando el registro de {puestoActualizar.Nombre}");
        return StatusCode(500, ModelState);
      }

      return Ok(puestoActualizar);
    }

    [HttpDelete("{puestoId:int}", Name = "BorrarPuesto")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BorrarPuesto(int puestoId)
    {
      if (!await _pRepo.ExistePuestoAsync(puestoId))
        return NotFound();

      var puesto = await _pRepo.GetPuestoAsync(puestoId);

      if (!await _pRepo.BorrarPuestoAsync(puesto))
      {
        ModelState.AddModelError("", $"Algo salió mal borrando el registro {puesto.Nombre}");
        return StatusCode(500, ModelState);
      }
      return NoContent();
    }
  }
}
