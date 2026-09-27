using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class RolController : ControllerBase
  {
    private readonly IRolRepositorio _rolRepo;
    private readonly IPermisoRepositorio _permRepo;
    private readonly IRolPermisoRepositorio _rolPermRepo;

    public RolController(IRolRepositorio rolRepo, IPermisoRepositorio permRepo, IRolPermisoRepositorio rolPermRepo)
    {
      _rolRepo = rolRepo;
      _permRepo = permRepo;
      _rolPermRepo = rolPermRepo;
    }

    // GET: flowfood/Rol/Listar
    [HttpGet("Listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoles()
    {
      var listaRoles = await _rolRepo.GetRolesAsync();
      var listaDto = new List<RolDto>();

      foreach (var item in listaRoles)
      {
        listaDto.Add(new RolDto
        {
          Id = item.Id,
          Nombre = item.Nombre,
          Categoria = item.Categoria,
          Funcion = item.Funcion
        });
      }

      return Ok(listaDto);
    }

    // GET: flowfood/Rol/BuscarPorNombre/{nombre}
    [HttpGet("BuscarPorNombre/{nombre}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRolPorNombre(string nombre)
    {
      if (!await _rolRepo.ExisteRolXNombreAsync(nombre))
        return NotFound();

      var rol = await _rolRepo.GetRolXNombreAsync(nombre);
      var rolDto = new RolDto
      {
        Id = rol.Id,
        Nombre = rol.Nombre,
        Categoria = rol.Categoria,
        Funcion = rol.Funcion
      };

      return Ok(rolDto);
    }

    // POST: flowfood/Rol/Guardar
    [HttpPost("Guardar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GuardarRol([FromBody] CrearRolDto crearRolDto)
    {
      if (crearRolDto == null)
        return BadRequest(ModelState);

      // Validamos que no se repita el nombre del rol
      if (await _rolRepo.ExisteRolXNombreAsync(crearRolDto.Nombre))
      {
        ModelState.AddModelError("", "Ya existe un rol con ese nombre");
        return StatusCode(400, ModelState);
      }

      var nuevoRol = new Rol
      {
        Nombre = crearRolDto.Nombre,
        Categoria = crearRolDto.Categoria,
        Funcion = crearRolDto.Funcion
      };

      if (!await _rolRepo.CrearRolAsync(nuevoRol))
      {
        ModelState.AddModelError("", $"Algo salió mal al guardar el registro de {nuevoRol.Nombre}");
        return StatusCode(500, ModelState);
      }

      // Creamos automaticamente la relacion RolPermiso con TODOS los permisos existentes,
      // todos deshabilitados por defecto, para que el rol nunca quede "incompleto"
      var permisosExistentes = await _permRepo.GetPermisosAsync();
      if (permisosExistentes.Any())
      {
        var nuevosRolPermisos = permisosExistentes.Select(p => new RolPermiso
        {
          rolId = nuevoRol.Id,
          permisoId = p.Id,
          Habilitado = false
        }).ToList();

        await _rolPermRepo.CrearRolPermisosAsync(nuevosRolPermisos);
      }

      var rolCreadoDto = new RolDto
      {
        Id = nuevoRol.Id,
        Nombre = nuevoRol.Nombre,
        Categoria = nuevoRol.Categoria,
        Funcion = nuevoRol.Funcion
      };

      return Ok(rolCreadoDto);
    }

    // PUT: flowfood/Rol/Actualizar/{id}
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarRol(int id, [FromBody] RolDto rolDto)
    {
      if (rolDto == null || id != rolDto.Id)
        return BadRequest(ModelState);

      if (!await _rolRepo.ExisteRolAsync(id))
        return NotFound();

      var rolActualizar = new Rol
      {
        Id = rolDto.Id,
        Nombre = rolDto.Nombre,
        Categoria = rolDto.Categoria,
        Funcion = rolDto.Funcion
      };

      if (!await _rolRepo.ActualizarRolAsync(rolActualizar))
      {
        ModelState.AddModelError("", $"Algo salió mal actualizando el registro de {rolActualizar.Nombre}");
        return StatusCode(500, ModelState);
      }

      return Ok(rolDto);
    }

    // DELETE: flowfood/Rol/Eliminar/{id}
    [HttpDelete("Eliminar/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EliminarRol(int id)
    {
      if (!await _rolRepo.ExisteRolAsync(id))
        return NotFound();

      var rolAEliminar = await _rolRepo.GetRolAsync(id);

      if (!await _rolRepo.BorrarRolAsync(rolAEliminar))
      {
        ModelState.AddModelError("", $"Algo salió mal borrando el registro de {rolAEliminar.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }
  }
}
