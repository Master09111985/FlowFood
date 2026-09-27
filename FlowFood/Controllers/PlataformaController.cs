using Microsoft.AspNetCore.Mvc;
using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;

namespace FlowFood.Controllers
{
  [Route("flowfood/plataforma")]
  [ApiController]
  public class PlataformaController : ControllerBase
  {
    private readonly IPlataformaRepositorio _plaRepo;

    public PlataformaController(IPlataformaRepositorio plaRepo)
    {
      _plaRepo = plaRepo;
    }

    [HttpGet("listarplataformas")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlataformas()
    {
      var listaPlataformas = await _plaRepo.GetPlataformasAsync();
      var listaPlataformasDto = new List<PlataformaDto>();

      foreach (var plataforma in listaPlataformas)
      {
        var plataformaDto = new PlataformaDto
        {
          Id = plataforma.Id,
          Nombre = plataforma.Nombre,
          Estado = plataforma.Estado
        };
        listaPlataformasDto.Add(plataformaDto);
      }
      return Ok(listaPlataformasDto);
    }

    [HttpGet("listarplataformasactivas")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPlataformasActivasAsync()
    {
      // Obtenemos todos las plataformas
      var listaPlataformas = await _plaRepo.GetPlataformasAsync();
      var listaPlataformasDto = new List<PlataformaDto>();

      foreach (var plataforma in listaPlataformas.Where(p => p.Estado == true))
      {
        var plataformaDto = new PlataformaDto
        {
          Id = plataforma.Id,
          Nombre = plataforma.Nombre,
          Estado = plataforma.Estado
        };
        listaPlataformasDto.Add(plataformaDto);
      }
      return Ok(listaPlataformasDto);
    }

    [HttpGet("{plataformaId:int}", Name = "GetPlataforma")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlataforma(int plataformaId)
    {
      var plataforma = await _plaRepo.GetPlataformaAsync(plataformaId);

      if (plataforma == null)
        return NotFound();

      var plataformaDto = new PlataformaDto
      {
        Id = plataforma.Id,
        Nombre = plataforma.Nombre,
        Estado = plataforma.Estado
      };

      return Ok(plataformaDto);
    }

    [HttpPost("crearplataforma")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CrearPlataforma([FromBody] CrearPlataformaDto crearPlataformaDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (crearPlataformaDto == null)
        return BadRequest(ModelState);

      if (await _plaRepo.ExistePlataformaXNombreAsync(crearPlataformaDto.Nombre))
      {
        ModelState.AddModelError("", "La plataforma ya existe");
        return StatusCode(404, ModelState);
      }

      var plataforma = new Plataforma
      {
        Nombre = crearPlataformaDto.Nombre,
        Estado = crearPlataformaDto.Estado
      };

      if (!await _plaRepo.CrearPlataformaAsync(plataforma))
      {
        ModelState.AddModelError("", $"Algo salio mal guardando el registro: {plataforma.Nombre}");
        return StatusCode(500, ModelState);
      }
      return Ok(plataforma);
    }

    [HttpPatch("{plataformaId:int}", Name = "ActualizarPatchPlataforma")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarPatchPlataforma(int plataformaId, [FromBody] PlataformaDto plataformaDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (plataformaDto == null || plataformaId != plataformaDto.Id)
        return BadRequest(ModelState);

      var plataforma = new Plataforma
      {
        Id = plataformaDto.Id,
        Nombre = plataformaDto.Nombre,
        Estado = plataformaDto.Estado
      };

      if (!await _plaRepo.ActualizarPlataformaAsync(plataforma))
      {
        ModelState.AddModelError("", $"Algo salio mal actualizando el registro {plataforma.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

    [HttpDelete("{plataformaId:int}", Name = "BorrarPlataforma")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BorrarPlataforma(int plataformaId)
    {
      if (!await _plaRepo.ExistePlataformaAsync(plataformaId))
        return NotFound();

      var plataforma = await _plaRepo.GetPlataformaAsync(plataformaId);

      if (!await _plaRepo.BorrarPlataformaAsync(plataforma))
      {
        ModelState.AddModelError("", $"Algo salio mal borrando el registro: {plataforma.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }
  }
}
