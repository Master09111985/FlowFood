using Microsoft.AspNetCore.Mvc;
using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class MotivoBajaController : ControllerBase
  {
    private readonly IMotivoBajaRepositorio _mbRepo;

    public MotivoBajaController(IMotivoBajaRepositorio mbRepo)
    {
      _mbRepo = mbRepo;
    }

    [HttpGet("Listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMotivos()
    {
      var lista = await _mbRepo.GetMotivosAsync();
      var listaDto = lista.Select(m => new MotivoBajaDto
      {
        Id = m.Id,
        Nombre = m.Nombre,
        Descripcion = m.Descripcion,
        Estado = m.Estado
      }).ToList();

      return Ok(listaDto);
    }

    [HttpGet("ListarActivos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMotivosActivos()
    {
      var lista = await _mbRepo.GetMotivosAsync();
      var listaDto = lista.Where(m => m.Estado == true).Select(m => new MotivoBajaDto
      {
        Id = m.Id,
        Nombre = m.Nombre,
        Descripcion = m.Descripcion,
        Estado = m.Estado
      }).ToList();

      return Ok(listaDto);
    }

    [HttpPost("Guardar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GuardarMotivo([FromBody] CrearMotivoBajaDto crearDto)
    {
      if (crearDto == null || !ModelState.IsValid)
        return BadRequest(ModelState);

      if (await _mbRepo.ExisteMotivoXNombreAsync(crearDto.Nombre))
      {
        ModelState.AddModelError("", "Ya existe un motivo de baja con este nombre.");
        return StatusCode(400, ModelState);
      }

      var nuevoMotivo = new MotivoBaja
      {
        Nombre = crearDto.Nombre,
        Descripcion = crearDto.Descripcion,
        Estado = crearDto.Estado
      };

      if (!await _mbRepo.CrearMotivoAsync(nuevoMotivo))
      {
        ModelState.AddModelError("", "Ocurrió un error al guardar el motivo de baja.");
        return StatusCode(500, ModelState);
      }

      return Ok(nuevoMotivo);
    }

    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarMotivo(int id, [FromBody] MotivoBajaDto motivoDto)
    {
      if (motivoDto == null || id != motivoDto.Id)
        return BadRequest(ModelState);

      if (!await _mbRepo.ExisteMotivoAsync(id))
        return NotFound();

      var motivoActualizar = new MotivoBaja
      {
        Id = motivoDto.Id,
        Nombre = motivoDto.Nombre,
        Descripcion = motivoDto.Descripcion,
        Estado = motivoDto.Estado
      };

      if (!await _mbRepo.ActualizarMotivoAsync(motivoActualizar))
      {
        ModelState.AddModelError("", "Ocurrió un error al actualizar el motivo de baja.");
        return StatusCode(500, ModelState);
      }

      return Ok(motivoActualizar);
    }

    [HttpDelete("Eliminar/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EliminarMotivo(int id)
    {
      if (!await _mbRepo.ExisteMotivoAsync(id))
        return NotFound();

      var motivoAEliminar = await _mbRepo.GetMotivoAsync(id);

      if (!await _mbRepo.BorrarMotivoAsync(motivoAEliminar))
      {
        ModelState.AddModelError("", "Ocurrió un error al borrar el motivo de baja.");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }
  }
}
