using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class RolPermisoController : ControllerBase
  {
    private readonly IRolPermisoRepositorio _rolPermRepo;
    private readonly IRolRepositorio _rolRepo;
    private readonly IPermisoRepositorio _permRepo;

    public RolPermisoController(IRolPermisoRepositorio rolPermRepo, IRolRepositorio rolRepo, IPermisoRepositorio permRepo)
    {
      _rolPermRepo = rolPermRepo;
      _rolRepo = rolRepo;
      _permRepo = permRepo;
    }

    // GET: flowfood/RolPermiso/Listar
    [HttpGet("Listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRolPermisos()
    {
      var lista = await _rolPermRepo.GetRolPermisosAsync();
      var listaDto = new List<RolPermisoDto>();

      foreach (var item in lista)
      {
        listaDto.Add(new RolPermisoDto
        {
          Id = item.Id,
          RolId = item.rolId,
          PermisoId = item.permisoId,
          PermisoNombre = item.Permiso?.Nombre,
          PermisoDescripcion = item.Permiso?.Descripcion,
          Habilitado = item.Habilitado
        });
      }

      return Ok(listaDto);
    }

    // GET: flowfood/RolPermiso/PorRol/{rolId}
    // Este es el endpoint clave para Angular: regresa el mapa de permisos listo para usar
    [HttpGet("PorRol/{rolId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMapaPermisosPorRol(int rolId)
    {
      if (!await _rolRepo.ExisteRolAsync(rolId))
        return NotFound();

      var rol = await _rolRepo.GetRolAsync(rolId);
      var rolPermisos = await _rolPermRepo.GetRolPermisosPorRolAsync(rolId);

      var mapaDto = new MapaPermisosDto
      {
        RolId = rol.Id,
        RolNombre = rol.Nombre,
        Permisos = rolPermisos.ToDictionary(
            rp => rp.Permiso.Nombre,
            rp => rp.Habilitado
        )
      };

      return Ok(mapaDto);
    }

    // Actualiza TODOS los permisos de un rol en una sola peticion (ideal para un form de checkboxes)
    [HttpPut("ActualizarPorRol")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarPermisosDeRol([FromBody] ActualizarPermisosRolDto actualizarDto)
    {
      if (actualizarDto == null || actualizarDto.Permisos == null || !actualizarDto.Permisos.Any())
        return BadRequest(ModelState);

      if (!await _rolRepo.ExisteRolAsync(actualizarDto.RolId))
        return NotFound();

      foreach (var permisoActualizar in actualizarDto.Permisos)
      {
        var rolPermisoExistente = await _rolPermRepo.GetRolPermisoAsync(actualizarDto.RolId, permisoActualizar.PermisoId);

        if (rolPermisoExistente == null)
        {
          // Si no existe la relacion todavia (rol o permiso creados despues), la creamos
          var nuevoRolPermiso = new RolPermiso
          {
            rolId = actualizarDto.RolId,
            permisoId = permisoActualizar.PermisoId,
            Habilitado = permisoActualizar.Habilitado
          };
          await _rolPermRepo.CrearRolPermisoAsync(nuevoRolPermiso);
        }
        else
        {
          rolPermisoExistente.Habilitado = permisoActualizar.Habilitado;
          await _rolPermRepo.ActualizarRolPermisoAsync(rolPermisoExistente);
        }
      }

      return Ok(actualizarDto);
    }

    // DELETE: flowfood/RolPermiso/Eliminar/{id}
    [HttpDelete("Eliminar/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EliminarRolPermiso(int id)
    {
      if (!await _rolPermRepo.ExisteRolPermisoAsync(id))
        return NotFound();

      var rolPermisoAEliminar = await _rolPermRepo.GetRolPermisoAsync(id);

      if (!await _rolPermRepo.BorrarRolPermisoAsync(rolPermisoAEliminar))
      {
        ModelState.AddModelError("", "Algo salió mal borrando el registro");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }
  }
}
