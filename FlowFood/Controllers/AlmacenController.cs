using Microsoft.AspNetCore.Mvc;

using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;


namespace FlowFood.Controllers
{
  [Route("flowfood/almacen")]
  [ApiController]
  public class AlmacenController : ControllerBase
  {
    private readonly IAlmacenRepositorio _alRepo;

    public AlmacenController(IAlmacenRepositorio alRepo)
    {
      _alRepo = alRepo;
    }

    [HttpGet("listaralmacenes")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAlmacenes()
    {
      var listaAlmacenes = await _alRepo.GetAlmacenesAsync();
      var listaAlmacenesDto = new List<AlmacenDto>();

      foreach (var almacen  in listaAlmacenes)
      {
        var almacenDto = new AlmacenDto
        {
          Id = almacen.Id,
          Nombre = almacen.Nombre,
          Descripcion = almacen.Descripcion,
          Estado = almacen.Estado
        };
        listaAlmacenesDto.Add(almacenDto);
      }
      return Ok(listaAlmacenesDto);
    }

    [HttpGet("listaralmacenesactivos")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAlmacenesActivosAsync()
    {
      // Obtenemos todos los almacenes
      var listaAlmacenes = await _alRepo.GetAlmacenesAsync();
      var listaAlmacenesDto = new List<AlmacenDto>();

      foreach (var almacen in listaAlmacenes.Where(a => a.Estado == true))
      {
        var almacenDto = new AlmacenDto
        {
          Id = almacen.Id,
          Nombre = almacen.Nombre,
          Descripcion = almacen.Descripcion,
          Estado = almacen.Estado
        };
        listaAlmacenesDto.Add(almacenDto);
      }
      return Ok(listaAlmacenesDto);
    }

    [HttpGet("{almacenId:int}", Name = "GetAlmacen")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAlmacen(int almacenId)
    {
      var almacen = await _alRepo.GetAlmacenAsync(almacenId);

      if (almacen == null)
        return NotFound();

      var almacenDto = new AlmacenDto
      {
        Id = almacen.Id,
        Nombre = almacen.Nombre,
        Descripcion = almacen.Descripcion,
        Estado = almacen.Estado
      };

      return Ok(almacenDto);
    }

    [HttpPost("crearalmacen")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CrearAlmacen([FromBody] CrearAlmacenDto crearAlmacenDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (crearAlmacenDto == null)
        return BadRequest(ModelState);

      if (await _alRepo.ExistenteAlmacenXNombreAsync(crearAlmacenDto.Nombre))
      {
        ModelState.AddModelError("", "El almacen ya existe");
        return StatusCode(404, ModelState);
      }

      var almacen = new Almacen
      {
        Nombre = crearAlmacenDto.Nombre,
        Descripcion = crearAlmacenDto.Descripcion,
        Estado = crearAlmacenDto.Estado
      };

      if (!await _alRepo.CrearAlmacenAsync(almacen))
      {
        ModelState.AddModelError("", $"Algo salio mal guardando el registro: {almacen.Nombre}");
        return StatusCode(500, ModelState);
      }
      return Ok(almacen);
    }

    [HttpPatch("{almacenId:int}", Name = "ActualizarPatchAlmacen")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarPatchAlmacen(int almacenId, [FromBody] AlmacenDto almacenDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (almacenDto == null || almacenId != almacenDto.Id)
        return BadRequest(ModelState);

      var almacen = new Almacen
      {
        Id = almacenDto.Id,
        Nombre = almacenDto.Nombre,
        Descripcion = almacenDto.Descripcion,
        Estado = almacenDto.Estado,
      };

      if (!await _alRepo.ActualizarAlmacenAsync(almacen))
      {
        ModelState.AddModelError("", $"Algo salio mal actualizando el registro {almacen.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

    [HttpDelete("{almacenId:int}", Name = "BorrarAlmacen")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BorrarAlmacen(int almacenId)
    {
      if (!await _alRepo.ExisteAlmacenAsync(almacenId))
        return NotFound();

      var almacen = await _alRepo.GetAlmacenAsync(almacenId);

      if (!await _alRepo.BorrarAlmacenAsync(almacen))
      {
        ModelState.AddModelError("", $"Algo salio mal borrando el registro {almacen.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }
  }
}
