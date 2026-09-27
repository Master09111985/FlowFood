using Microsoft.AspNetCore.Mvc;

using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;

namespace FlowFood.Controllers
{
  [Route("flowfood/unidad")]
  [ApiController]
  public class UnidadMedidaController : ControllerBase
  {
    private readonly IUnidadMedidaRepositorio _uniRepo;

    public UnidadMedidaController(IUnidadMedidaRepositorio uniRepo)
    {
      _uniRepo = uniRepo;
    }

    [HttpGet("listarunidades")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnidades()
    {
      var listaUnidades = await _uniRepo.GetUnidadMedidasAsync();
      var listaUnidadesDto = new List<UnidadMedidaDto>();

      foreach (var unidad in listaUnidades)
      {
        var unidadDto = new UnidadMedidaDto
        {
          Id = unidad.Id,
          Nombre = unidad.Nombre,
          Estado = unidad.Estado
        };
        listaUnidadesDto.Add(unidadDto);
      }
      return Ok(listaUnidadesDto);
    }

    [HttpGet("listarunidadesactivas")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetUnidadesActivasAsync()
    {
      // Obtenemos todas las unidades
      var listaUnidades = await _uniRepo.GetUnidadMedidasAsync();
      var listaUnidadesDto = new List<UnidadMedidaDto>();

      foreach (var unidad in listaUnidades.Where(u => u.Estado == true))
      {
        var unidadDto = new UnidadMedidaDto
        {
          Id = unidad.Id,
          Nombre = unidad.Nombre,
          Estado = unidad.Estado
        };
        listaUnidadesDto.Add(unidadDto);
      }
      return Ok(listaUnidadesDto);
    }

    [HttpGet("{unidadId:int}", Name = "GetUnidad")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUnidad(int unidadId)
    {
      var unidad = await _uniRepo.GetUnidadMedidaAsync(unidadId);

      if (unidad == null)
        return NotFound();

      var unidadDto = new UnidadMedidaDto
      {
        Id = unidad.Id,
        Nombre = unidad.Nombre,
        Estado = unidad.Estado
      };

      return Ok(unidadDto);
    }

    [HttpPost("crearunidad")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CrearUnidad([FromBody] CrearUnidadMedidaDto crearUnidadDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (crearUnidadDto == null)
        return BadRequest(ModelState);

      if (await _uniRepo.ExisteUnidadMedidaXNombreAsync(crearUnidadDto.Nombre))
      {
        ModelState.AddModelError("", "La unidad de Medida ya existe.");
        return StatusCode(404, ModelState);
      }

      var unidad = new UnidadMedida
      {
        Nombre = crearUnidadDto.Nombre,
        Estado = crearUnidadDto.Estado
      };

      if (!await _uniRepo.CrearUnidadMedidaAsync(unidad))
      {
        ModelState.AddModelError("", $"Algo salio mal guardando el registro: {unidad.Nombre}");
        return StatusCode(500, ModelState);
      }
      return Ok(unidad);
    }

    [HttpPatch("{unidadMedidaId:int}", Name = "ActualizarPatchUnidad")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarPatchUnidad(int unidadMedidaId, [FromBody] UnidadMedidaDto unidadMedidaDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (unidadMedidaDto.Id > 0 && unidadMedidaId != unidadMedidaDto.Id)
        return BadRequest("El ID de la URL no cincide con el ID del cuerpo.");

      var unidad = new UnidadMedida
      {
        Id = unidadMedidaDto.Id,
        Nombre = unidadMedidaDto.Nombre,
        Estado = unidadMedidaDto.Estado
      };

      if (!await _uniRepo.ActualizarUnidadMedidaAsync(unidad))
      {
        ModelState.AddModelError("", $"Algo salio mal actualizado el registro { unidad.Nombre }");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

    [HttpDelete("{unidadMedidaId:int}", Name = "BorrarUnidad")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BorrarUnidad(int unidadMedidaId)
    {
      if (!await _uniRepo.ExisteUnidadMedidaAsync(unidadMedidaId))
        return NotFound();

      var unidad = await _uniRepo.GetUnidadMedidaAsync(unidadMedidaId);

      if (!await _uniRepo.BorrarUnidadMedidaAsync(unidad))
      {
        ModelState.AddModelError("", $"Algo salio mal borrando el registro { unidad.Nombre }");
        return StatusCode(500, ModelState);
      }
      return NoContent();
    }
  }
}
