using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class UsuarioController : ControllerBase
  {
    private readonly IUsuarioRepositorio _usuRepo;

    public UsuarioController(IUsuarioRepositorio usuRepo)
    {
        _usuRepo = usuRepo;
    }

    // GET: flowfood/Usuario/Listar
    [HttpGet("Listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsuarios()
    {
      var listaUsuarios = await _usuRepo.GetUsuariosAsync();
      var listaDto = new List<UsuarioDto>();

      foreach (var item in listaUsuarios)
      {
        listaDto.Add(new UsuarioDto
        {
          Id = item.Id,
          Nombre = item.Nombre,
          FechaRegistro = item.FechaRegistro,
          Estado = item.Estado,
          RolId = item.rolId,
          RolNombre = item.Rol?.Nombre,
          EmpleadoId = item.empleadoId,
          EmpleadoNombre = item.Empleado?.Nombre
        });
      }
      return Ok(listaDto);
    }

    // GET: flowfood/Usuario/BuscarPorNombre/{nombre}
    [HttpGet("BuscarPorNombre/{nombre}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsuarioPorNombre(string nombre)
    {
      if (!await _usuRepo.ExisteUsuarioXNombreAsync(nombre))
        return NotFound();

      var usuario = await _usuRepo.GetUsuarioXNombreAsync(nombre);
      var usuarioDto = new UsuarioDto
      {
        Id = usuario.Id,
        Nombre = usuario.Nombre,
        FechaRegistro = usuario.FechaRegistro,
        Estado = usuario.Estado,
        RolId = usuario.rolId,
        RolNombre = usuario.Rol?.Nombre,
        EmpleadoId = usuario.empleadoId,
        EmpleadoNombre = usuario.Empleado?.Nombre
      };
      return Ok(usuarioDto);
    }

    // POST: flowfood/Usuario/Login
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
      if (loginDto == null || string.IsNullOrEmpty(loginDto.Nombre) || string.IsNullOrEmpty(loginDto.Password))
        return BadRequest(new { mensaje = "Usuario y password son requeridos" });

      var usuario = await _usuRepo.GetUsuarioXNombreAsync(loginDto.Nombre);

      if (usuario == null || !usuario.Estado)
        return Unauthorized(new { mensaje = "Usuario no encontrado o inactivo" });

      bool esPasswordValido = BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.Password);

      if (!esPasswordValido)
        return Unauthorized(new { mensaje = "Credenciales incorrectas" });

      var usuarioLoginDto = new UsuarioDto
      {
        Id = usuario.Id,
        Nombre = usuario.Nombre,
        RolId = usuario.rolId,
        RolNombre = usuario.Rol?.Nombre,
        EmpleadoId = usuario.empleadoId,
        EmpleadoNombre = usuario.Empleado?.Nombre,
        Estado = usuario.Estado
      };
      return Ok(usuarioLoginDto);
    }

    // POST: flowfood/Usuario/Guardar
    [HttpPost("Guardar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GuardarUsuario([FromBody] CrearUsuarioDto crearUsuarioDto)
    {
      // Verificamos que no venga vacio
      if (crearUsuarioDto == null)
        return BadRequest(ModelState);

      // Validamos que no se repita el nombre del usuario
      if (await _usuRepo.ExisteUsuarioXNombreAsync(crearUsuarioDto.Nombre))
      {
        ModelState.AddModelError("", "Ya existe el usuario que intentas guardar.");
        return StatusCode(400, ModelState);
      }

      // Validamos que el usuario no se le asigne a un empleado que ya tiene cuenta
      if (await _usuRepo.ExisteUsuarioXNombreEmpleadoAsync(crearUsuarioDto.EmpleadoId))
      {
        ModelState.AddModelError("", "Este empleado ya cuenta con un usuario");
        return StatusCode(400, ModelState);
      }

      var nuevoUsuario = new Usuario
      {
        Nombre = crearUsuarioDto.Nombre,
        Password = BCrypt.Net.BCrypt.HashPassword(crearUsuarioDto.Password),
        FechaRegistro = DateTime.UtcNow,
        Estado = true,
        rolId = crearUsuarioDto.RolId,
        empleadoId = crearUsuarioDto.EmpleadoId
      };

      if (!await _usuRepo.CrearUsuarioAsync(nuevoUsuario))
      {
        ModelState.AddModelError("", $"Algo salio mal al guardar el registro de {nuevoUsuario.Nombre}");
        return StatusCode(500, ModelState);
      }

      // Regresamos el DTO seguro
      var usuarioCreado = new UsuarioDto
      {
        Id = nuevoUsuario.Id,
        Nombre = nuevoUsuario.Nombre,
        FechaRegistro = nuevoUsuario.FechaRegistro,
        Estado = nuevoUsuario.Estado,
        RolId = nuevoUsuario.rolId,
        EmpleadoId = nuevoUsuario.empleadoId
      };

      return Ok(usuarioCreado);
    }

    // PUT: flowfood/Usuario/Actualizar/{id}
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] UsuarioDto usuarioDto)
    {
      if (usuarioDto == null || id != usuarioDto.Id)
        return BadRequest(ModelState);

      if (!await _usuRepo.ExisteUsuarioAsync(id))
        return NotFound();

      // Como UsuarioDto no contiene el Password(por seguridad)
      // Recuperamos el usuario actual para conservar el hash existente
      var usuarioActual = await _usuRepo.GetUsuarioAsync(id);

      var usuarioActualizar = new Usuario
      {
        Id = usuarioDto.Id,
        Nombre = usuarioDto.Nombre,
        Password = usuarioActual.Password, // se conserva, no se toca aquí
        FechaRegistro = usuarioActual.FechaRegistro, // no se debe modificar
        Estado = usuarioDto.Estado,
        rolId = usuarioDto.RolId,
        empleadoId = usuarioDto.EmpleadoId
      };

      if (!await _usuRepo.ActualizarUsuarioAsync(usuarioActualizar))
      {
        ModelState.AddModelError("", $"Algo salio mal actualizando el registro de {usuarioActualizar.Nombre}");
        return StatusCode(500, ModelState);
      }

      return Ok(usuarioDto);
    }

    // DELETE: flowfood/Usuario/Eliminar/{id}
    [HttpDelete("Eliminar/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EliminarUsuario(int id)
    {
      if (!await _usuRepo.ExisteUsuarioAsync(id))
        return NotFound();

      var usuarioAEliminar = await _usuRepo.GetUsuarioAsync(id);

      if (!await _usuRepo.BorrarUsuarioAsync(usuarioAEliminar))
      {
        ModelState.AddModelError("", $"Algo salio mal borrando el registro de {usuarioAEliminar.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }
  }
}
