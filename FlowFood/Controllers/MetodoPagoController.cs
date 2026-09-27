using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[Controller]")]
  [ApiController]
  public class MetodoPagoController : ControllerBase
  {
    private readonly IMetodoPagoRepositorio _meRepo;

    public MetodoPagoController(IMetodoPagoRepositorio merepo)
    {
        _meRepo = merepo;
    }

    [HttpGet("listarmetodosdepago")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMetodosPagoAsync()
    {
      var listaMetodosPago = await _meRepo.GetMetodosPagoAsync();
      var listaMetodosPagoDto = new List<MetodoPagoDto>();

      foreach (var metodoPago in listaMetodosPago)
      {
        var metodoPagoDto = new MetodoPagoDto
        {
          Id = metodoPago.Id,
          Nombre = metodoPago.Nombre,
          Estado = metodoPago.Estado,
        };
        listaMetodosPagoDto.Add(metodoPagoDto);
      }
      return Ok(listaMetodosPagoDto);
    }

    [HttpGet("listarmetodospagoactivos")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetMetodosPagoActivosAsync()
    {
      // Obtenemos todos los metodos de pago
      var listaMetodosPago = await _meRepo.GetMetodosPagoAsync();
      var listaMetodosPagoDto = new List<MetodoPagoDto>();

      foreach (var metodoPago in listaMetodosPago.Where(m => m.Estado == true))
      {
        var metodoPagosDto = new MetodoPagoDto
        {
          Id = metodoPago.Id,
          Nombre = metodoPago.Nombre,
          Estado= metodoPago.Estado
        };
        listaMetodosPagoDto.Add(metodoPagosDto);
      }
      return Ok(listaMetodosPagoDto);
    }

    [HttpGet("{metodoPagoId:int}", Name = "GetMetodoPago")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMetodoPagoAsync(int metodoPagoId)
    {
      var metodoPago = await _meRepo.GetMetodoPagoAsync(metodoPagoId);

      if (metodoPago == null)
        return NotFound();

      var metodoPagoDto = new MetodoPagoDto
      {
        Id = metodoPago.Id,
        Nombre = metodoPago.Nombre,
        Estado = metodoPago.Estado
      };

      return Ok(metodoPagoDto);
    }

    [HttpPost("crearmetodopago")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CrearMetodoPago([FromBody] CrearMetodoPagoDto crearMetodoPagoDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (crearMetodoPagoDto == null)
        return BadRequest(ModelState);

      if (await _meRepo.ExisteMetodoPagoXNombreAsync(crearMetodoPagoDto.Nombre))
      {
        ModelState.AddModelError("", "El Metodo de Pago ya existe");
        return StatusCode(404, ModelState);
      }

      var metodoPago = new MetodoPago
      {
        Nombre = crearMetodoPagoDto.Nombre,
        Estado = crearMetodoPagoDto.Estado
      };

      if (!await _meRepo.CrearMetodoPagoAsync(metodoPago))
      {
        ModelState.AddModelError("", $"Algo salio mal guardando el metodo de pago.");
        return StatusCode(500, ModelState);
      }
      return Ok(metodoPago);
    }

    [HttpPatch("{metodoPagoId:int}", Name = "ActualizarPatchMetodoPago")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarPatchMetodoPago(int metodoPagoId, [FromBody] MetodoPagoDto metodoPagoDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (metodoPagoDto == null || metodoPagoId != metodoPagoDto.Id)
        return BadRequest(ModelState);

      var metodoPago = new MetodoPago
      {
        Id = metodoPagoDto.Id,
        Nombre = metodoPagoDto.Nombre,
        Estado = metodoPagoDto.Estado
      };

      if (!await _meRepo.ActualizarMetodoPagoAsync(metodoPago))
      {
        ModelState.AddModelError("", $"Algo salio mal actualizando el registro {metodoPago.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent ();
    }

    [HttpDelete("{metodoPagoId:int}", Name = "BorrarMetodoPago")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BorrarMetodoPago(int metodoPagoId)
    {
      if (!await _meRepo.ExisteMetodoPagoAsync(metodoPagoId))
        return NotFound();

      var metodoPago = await _meRepo.GetMetodoPagoAsync(metodoPagoId);

      if (!await _meRepo.BorrarMetodoPago(metodoPago))
      {
        ModelState.AddModelError("", $"Algo salio mal borrando el registro {metodoPago.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent ();
    }
  }
}
