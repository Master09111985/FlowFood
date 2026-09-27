using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class EmpleadoController : ControllerBase
  {
    private readonly IEmpleadoRepositorio _empRepo;

    public EmpleadoController(IEmpleadoRepositorio empRepo)
    {
      _empRepo = empRepo;
    }

    // GET: flowfood/Empleado/Listar
    [HttpGet("Listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEmpleados()
    {
      var listaEmpleados = await _empRepo.GetEmpleadosAsync();
      var listaDto = new List<EmpleadoDto>();

      foreach (var item in listaEmpleados)
      {
        listaDto.Add(MapearEmpleadoDto(item));
      }

      return Ok(listaDto);
    }

    // GET: flowfood/Empleado/Buscar/{id}
    [HttpGet("Buscar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpleado(int id)
    {
      if (!await _empRepo.ExisteEmpleadoAsync(id))
        return NotFound();

      var empleado = await _empRepo.GetEmpleadoAsync(id);
      return Ok(MapearEmpleadoDto(empleado));
    }

    // GET: flowfood/Empleado/BuscarPorCodigo/{codigo}
    // Este es el que usara el lector de codigo de barras del gafete
    [HttpGet("BuscarPorCodigo/{codigo}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmpleadoPorCodigo(string codigo)
    {
      if (!await _empRepo.ExisteEmpleadoXCodigoAsync(codigo))
        return NotFound();

      var empleado = await _empRepo.GetEmpleadoXCodigoAsync(codigo);
      return Ok(MapearEmpleadoDto(empleado));
    }

    // POST: flowfood/Empleado/Guardar
    [HttpPost("Guardar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GuardarEmpleado([FromForm] CrearEmpleadoDto crearEmpleadoDto)
    {
      if (crearEmpleadoDto == null)
        return BadRequest(ModelState);

      // Validamos la imagen
      if (crearEmpleadoDto.Foto == null || crearEmpleadoDto.Foto.Length == 0)
      {
        ModelState.AddModelError("", "Debe adjuntar una foto del empleado");
        return StatusCode(400, ModelState);
      }

      var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
      var extension = Path.GetExtension(crearEmpleadoDto.Foto.FileName).ToLowerInvariant();

      if (!extensionesPermitidas.Contains(extension))
      {
        ModelState.AddModelError("", "Formato de imagen no permitido. Solo JPG o PNG");
        return StatusCode(400, ModelState);
      }

      // Guardamos la foto fisicamente
      var carpetaDestino = @"C:\LosDeCamaron\empleados";
      if (!Directory.Exists(carpetaDestino))
        Directory.CreateDirectory(carpetaDestino);

      var nombreArchivo = $"{Guid.NewGuid()}{extension}";
      var rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

      using (var stream = new FileStream(rutaCompleta, FileMode.Create))
      {
        await crearEmpleadoDto.Foto.CopyToAsync(stream);
      }

      var rutaRelativa = $"/fotos-empleados/{nombreArchivo}";

      // Generamos el codigo consecutivo
      var codigoGenerado = await _empRepo.GenerarSiguienteCodigoAsync();

      var nuevoEmpleado = new Empleado
      {
        Nombre = crearEmpleadoDto.Nombre,
        Direccion = crearEmpleadoDto.Direccion,
        Telefono = crearEmpleadoDto.Telefono,
        Edad = crearEmpleadoDto.Edad,
        SalarioSemanal = crearEmpleadoDto.SalarioSemanal,
        Codigo = codigoGenerado,
        FechaContrato = crearEmpleadoDto.FechaContrato,
        FechaRegistro = DateTime.UtcNow,
        FotoUrl = rutaRelativa,
        Estado = true,
        puestoId = crearEmpleadoDto.PuestoId
      };

      if (!await _empRepo.CrearEmpleadoAsync(nuevoEmpleado))
      {
        // Si algo sale mal guardando en BD, borramos la foto huerfana que ya se guardo en disco
        if (System.IO.File.Exists(rutaCompleta))
          System.IO.File.Delete(rutaCompleta);

        ModelState.AddModelError("", $"Algo salió mal al guardar el registro de {nuevoEmpleado.Nombre}");
        return StatusCode(500, ModelState);
      }

      return Ok(MapearEmpleadoDto(nuevoEmpleado));
    }

    // PUT: flowfood/Empleado/Actualizar/{id}
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarEmpleado(int id, [FromForm] ActualizarEmpleadoDto actualizarDto)
    {
      if (actualizarDto == null || id != actualizarDto.Id)
        return BadRequest(ModelState);

      if (!await _empRepo.ExisteEmpleadoAsync(id))
        return NotFound();

      var empleadoActual = await _empRepo.GetEmpleadoAsync(id);

      // Por defecto conservamos la foto actual
      var rutaFotoFinal = empleadoActual.FotoUrl;
      string rutaFotoAnteriorParaBorrar = null;

      // Si viene una foto nueva, la validamos y guardamos
      if (actualizarDto.Foto != null && actualizarDto.Foto.Length > 0)
      {
        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(actualizarDto.Foto.FileName).ToLowerInvariant();

        if (!extensionesPermitidas.Contains(extension))
        {
          ModelState.AddModelError("", "Formato de imagen no permitido. Solo JPG o PNG");
          return StatusCode(400, ModelState);
        }

        // --- RUTA MODIFICADA A C: ---
        var carpetaDestino = @"C:\LosDeCamaron\empleados";
        if (!Directory.Exists(carpetaDestino))
          Directory.CreateDirectory(carpetaDestino);

        var nombreArchivo = $"{Guid.NewGuid()}{extension}";
        var rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
        {
          await actualizarDto.Foto.CopyToAsync(stream);
        }

        // Guardamos la referencia de la foto vieja para borrarla DESPUES de que
        // el update en BD sea exitoso (evita borrar la foto vieja si algo falla)
        rutaFotoAnteriorParaBorrar = empleadoActual.FotoUrl;

        // Mantenemos la ruta relativa para la base de datos y Angular
        rutaFotoFinal = $"/fotos-empleados/{nombreArchivo}";
      }

      var empleadoActualizar = new Empleado
      {
        Id = actualizarDto.Id,
        Nombre = actualizarDto.Nombre,
        Direccion = actualizarDto.Direccion,
        Telefono = actualizarDto.Telefono,
        Edad = actualizarDto.Edad,
        SalarioSemanal = actualizarDto.SalarioSemanal,
        Codigo = empleadoActual.Codigo,           // nunca cambia
        FechaContrato = actualizarDto.FechaContrato,
        FechaRegistro = empleadoActual.FechaRegistro, // nunca cambia
        FotoUrl = rutaFotoFinal,
        Estado = actualizarDto.Estado,
        puestoId = actualizarDto.PuestoId
      };

      if (!await _empRepo.ActualizarEmpleadoAsync(empleadoActualizar))
      {
        // Si fallo el update y habiamos subido una foto nueva, la borramos (quedaria huerfana)
        if (rutaFotoAnteriorParaBorrar != null)
        {
          // --- EXTRACCIÓN Y BORRADO DESDE C: ---
          var nombreArchivoNuevo = Path.GetFileName(rutaFotoFinal);
          var rutaFisicaNueva = Path.Combine(@"C:\LosDeCamaron\empleados", nombreArchivoNuevo);

          if (System.IO.File.Exists(rutaFisicaNueva))
            System.IO.File.Delete(rutaFisicaNueva);
        }

        ModelState.AddModelError("", $"Algo salió mal actualizando el registro de {empleadoActualizar.Nombre}");
        return StatusCode(500, ModelState);
      }

      // Si el update fue exitoso y habia una foto anterior, ahora si la borramos
      if (rutaFotoAnteriorParaBorrar != null)
      {
        // --- EXTRACCIÓN Y BORRADO DESDE C: ---
        var nombreArchivoAnterior = Path.GetFileName(rutaFotoAnteriorParaBorrar);
        var rutaFisicaAnterior = Path.Combine(@"C:\LosDeCamaron\empleados", nombreArchivoAnterior);

        if (System.IO.File.Exists(rutaFisicaAnterior))
          System.IO.File.Delete(rutaFisicaAnterior);
      }

      return Ok(MapearEmpleadoDto(empleadoActualizar));
    }

    // DELETE: flowfood/Empleado/Eliminar/{id}
    [HttpDelete("Eliminar/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EliminarEmpleado(int id)
    {
      if (!await _empRepo.ExisteEmpleadoAsync(id))
        return NotFound();

      var empleadoAEliminar = await _empRepo.GetEmpleadoAsync(id);

      if (!await _empRepo.BorrarEmpleadoAsync(empleadoAEliminar))
      {
        ModelState.AddModelError("", $"Algo salió mal borrando el registro de {empleadoAEliminar.Nombre}");
        return StatusCode(500, ModelState);
      }

      if (!string.IsNullOrEmpty(empleadoAEliminar.FotoUrl))
      {
        var nombreArchivoEliminar = Path.GetFileName(empleadoAEliminar.FotoUrl);
        var rutaFisicaEliminar = Path.Combine(@"C:\LosDeCamaron\empleados", nombreArchivoEliminar);

        if (System.IO.File.Exists(rutaFisicaEliminar))
          System.IO.File.Delete(rutaFisicaEliminar);
      }

      return NoContent();
    }

    // POST: flowfood/Empleado/SubirFoto
    [HttpPost("SubirFoto")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubirFoto(IFormFile foto)
    {
      if (foto == null || foto.Length == 0)
        return BadRequest("No se recibió ninguna imagen");

      // Validamos que sea una imagen
      var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
      var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

      if (!extensionesPermitidas.Contains(extension))
        return BadRequest("Formato de imagen no permitido. Solo JPG o PNG");

      // Carpeta donde se guardan las fotos (dentro de wwwroot)
      var carpetaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fotos-empleados");
      if (!Directory.Exists(carpetaDestino))
        Directory.CreateDirectory(carpetaDestino);

      // Nombre unico para evitar sobreescribir archivos con el mismo nombre
      var nombreArchivo = $"{Guid.NewGuid()}{extension}";
      var rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

      using (var stream = new FileStream(rutaCompleta, FileMode.Create))
      {
        await foto.CopyToAsync(stream);
      }

      // Esta es la ruta que guardas en el campo FotoUrl del Empleado
      var rutaRelativa = $"/fotos-empleados/{nombreArchivo}";

      return Ok(new { fotoUrl = rutaRelativa });
    }

    private EmpleadoDto MapearEmpleadoDto(Empleado empleado)
    {
      return new EmpleadoDto
      {
        Id = empleado.Id,
        Nombre = empleado.Nombre,
        Direccion = empleado.Direccion,
        Telefono = empleado.Telefono,
        Edad = empleado.Edad,
        SalarioSemanal = empleado.SalarioSemanal,
        Codigo = empleado.Codigo,
        FechaContrato = empleado.FechaContrato,
        FechaRegistro = empleado.FechaRegistro,
        FotoUrl = empleado.FotoUrl,
        Estado = empleado.Estado,
        PuestoId = empleado.puestoId,
        PuestoNombre = empleado.Puesto?.Nombre
      };
    }
  }
}
