using Microsoft.AspNetCore.Mvc;

using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;

namespace FlowFood.Controllers
{
  [Route("flowfood/proveedor")]
  [ApiController]
  public class ProveedorController : ControllerBase
  {
    private readonly IProveedorRepositorio _proRepo;

    public ProveedorController(IProveedorRepositorio proRepo)
    {
        _proRepo = proRepo;
    }

    [HttpGet("listarproveedores")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProveedores()
    {
      var listarProveedores = await _proRepo.GetProveedoresAsync();
      var listaProveedoresDto = new List<ProveedorDto>();

      foreach (var proveedor in listarProveedores)
      {
        var proveedorDto = new ProveedorDto
        {
          Id = proveedor.Id,
          Nombre = proveedor.Nombre,
          Comentario = proveedor.Comentario,
          Estado = proveedor.Estado
        };
        listaProveedoresDto.Add(proveedorDto);
      }
      return Ok(listaProveedoresDto);
    }

    [HttpGet("listarproveedoresactivos")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProveedoresActivosAsync()
    {
      // Obtenemos todos los proveedores
      var listaProveedores = await _proRepo.GetProveedoresAsync();
      var listaProveedoresDto = new List<ProveedorDto>();

      foreach (var proveedor in listaProveedores.Where(p => p.Estado == true))
      {
        var proveedorDto = new ProveedorDto
        {
          Id = proveedor.Id,
          Nombre = proveedor.Nombre,
          Comentario = proveedor.Comentario,
          Estado = proveedor.Estado
        };
        listaProveedoresDto.Add(proveedorDto);
      }
      return Ok(listaProveedoresDto);
    }

    [HttpGet("{proveedorId:int}", Name = "GetProveedor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProveedor(int proveedorId)
    {
      var proveedor = await _proRepo.GetProveedorAsync(proveedorId);

      if (proveedor == null)
        return NotFound();

      var proveedorDto = new ProveedorDto
      {
        Id = proveedor.Id,
        Nombre = proveedor.Nombre,
        Comentario = proveedor.Comentario,
        Estado = proveedor.Estado
      };

      return Ok(proveedorDto);
    }

    [HttpPost("crearproveedor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CrearProveedor([FromBody] CrearProveedorDto crearProveedorDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (crearProveedorDto == null)
        return BadRequest(ModelState);

      if (await _proRepo.ExistenteProveedorXNombreAsync(crearProveedorDto.Nombre))
      {
        ModelState.AddModelError("", "El proveedor ya existe");
        return StatusCode(404, ModelState);
      }

      var proveedor = new Proveedor
      {
        Nombre = crearProveedorDto.Nombre,
        Comentario = crearProveedorDto.Comentario,
        Estado = crearProveedorDto.Estado
      };

      if  (!await _proRepo.CrearProveedorAsync(proveedor))
      {
        ModelState.AddModelError("", $"Algo salio mal guardando el registro: {proveedor.Nombre}");
        StatusCode(500, ModelState);
      }
      return Ok(proveedor);
    }

    [HttpPatch("{proveedorId:int}", Name = "ActualizarPatchProveedor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarPatchProveedor(int proveedorId, [FromBody] ProveedorDto proveedorDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (proveedorDto == null || proveedorId != proveedorDto.Id)
        return BadRequest(ModelState);

      var proveedor = new Proveedor
      {
        Id = proveedorDto.Id,
        Nombre = proveedorDto.Nombre,
        Comentario = proveedorDto.Comentario,
        Estado = proveedorDto.Estado
      };

      if (!await _proRepo.ActualizarProveedorAsync(proveedor))
      {
        ModelState.AddModelError("", $"Algo salio mal actualizando el registro: {proveedor.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

    [HttpDelete("{proveedorId:int}", Name = "BorrarProveedor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BorrarProveedor(int proveedorId)
    {
      if (!await _proRepo.ExisteProveedorAsync(proveedorId))
        return NotFound();

      var proveedor = await _proRepo.GetProveedorAsync(proveedorId);

      if (!await _proRepo.BorrarProveedorAsync(proveedor))
      {
        ModelState.AddModelError("", $"Algo salio mal borrando el registro: {proveedor.Nombre}");
        return StatusCode(500, ModelState);
      }
      return NoContent();
    }
  }
}
