using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class PermisoController : ControllerBase
  {
    private readonly IPermisoRepositorio _permRepo;

    public PermisoController(IPermisoRepositorio permRepo)
    {
        _permRepo = permRepo;
    }

    // GET: flowfood/Permiso/Listar
    [HttpGet("Listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPermisos()
    {
      var listaPermisos = await _permRepo.GetPermisosAsync();
      var listaDto = new List<PermisoDto>();

      foreach (var item in listaPermisos)
      {
        listaDto.Add(new PermisoDto
        {
          Id = item.Id,
          Nombre = item.Nombre,
          Descripcion = item.Descripcion
        });
      }

      return Ok(listaDto);
    }

    // GET: flowfood/Permiso/BuscarPorNombre/{nombre}
    [HttpGet("BuscarPorNombre/{nombre}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPermisoPorNombre(string nombre)
    {
      if (!await _permRepo.ExistePermisoXNombreAsync(nombre))
        return NotFound();

      var permiso = await _permRepo.GetPermisoXNombreAsync(nombre);
      var permisoDto = new PermisoDto
      {
        Id = permiso.Id,
        Nombre = permiso.Nombre,
        Descripcion = permiso.Descripcion
      };

      return Ok(permisoDto);
    }

    // POST: flowfood/Permiso/Guardar
    [HttpPost("Guardar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GuardarPermiso([FromBody] CrearPermisoDto crearPermisoDto)
    {
      if (crearPermisoDto == null)
        return BadRequest(ModelState);

      // Validamos que no se repita el nombre del permiso
      if (await _permRepo.ExistePermisoXNombreAsync(crearPermisoDto.Nombre))
      {
        ModelState.AddModelError("", "Ya existe un permiso con ese nombre");
        return StatusCode(400, ModelState);
      }

      var nuevoPermiso = new Permiso
      {
        Nombre = crearPermisoDto.Nombre,
        Descripcion = crearPermisoDto.Descripcion
      };

      if (!await _permRepo.CrearPermisoAsync(nuevoPermiso))
      {
        ModelState.AddModelError("", $"Algo salió mal al guardar el registro de {nuevoPermiso.Nombre}");
        return StatusCode(500, ModelState);
      }

      return Ok(nuevoPermiso);
    }

    // PUT: flowfood/Permiso/Actualizar/{id}
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarPermiso(int id, [FromBody] PermisoDto permisoDto)
    {
      if (permisoDto == null || id != permisoDto.Id)
        return BadRequest(ModelState);

      if (!await _permRepo.ExistePermisoAsync(id))
        return NotFound();

      var permisoActualizar = new Permiso
      {
        Id = permisoDto.Id,
        Nombre = permisoDto.Nombre,
        Descripcion = permisoDto.Descripcion
      };

      if (!await _permRepo.ActualizarPermisoAsync(permisoActualizar))
      {
        ModelState.AddModelError("", $"Algo salió mal actualizando el registro de {permisoActualizar.Nombre}");
        return StatusCode(500, ModelState);
      }

      return Ok(permisoActualizar);
    }

    // DELETE: flowfood/Permiso/Eliminar/{id}
    [HttpDelete("Eliminar/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EliminarPermiso(int id)
    {
      if (!await _permRepo.ExistePermisoAsync(id))
        return NotFound();

      var permisoAEliminar = await _permRepo.GetPermisoAsync(id);

      if (!await _permRepo.BorrarPermisoAsync(permisoAEliminar))
      {
        ModelState.AddModelError("", $"Algo salió mal borrando el registro de {permisoAEliminar.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }
  }
}
