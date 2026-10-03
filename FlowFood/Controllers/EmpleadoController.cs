using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace FlowFood.Controllers
{
    [Route("flowfood/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly IEmpleadoRepositorio _empRepo;
        private readonly Cloudinary _cloudinary;

        public EmpleadoController(IEmpleadoRepositorio empRepo, Cloudinary cloudinary)
        {
            _empRepo = empRepo;
            _cloudinary = cloudinary;
        }

        // GET: flowfood/Empleado/Listar
        [HttpGet("Listar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetEmpleados()
        {
            try
            {
                var listaEmpleados = await _empRepo.GetEmpleadosAsync();
                var listaDto = listaEmpleados.Select(MapearEmpleadoDto).ToList();
                return Ok(listaDto);
            }
            catch (Exception ex)
            {
                // Esto capturará el error exacto (ej. "Invalid column name", "SqlDateTime overflow", etc.)
                return StatusCode(500, new
                {
                    MensajePrincipal = ex.Message,
                    DetalleInterno = ex.InnerException?.Message,
                    Pista = "El error está al intentar leer los datos de la base de datos en Somee."
                });
            }
        }

        // GET: flowfood/Empleado/Buscar/{id}
        [HttpGet("Buscar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmpleado(int id)
        {
            if (!await _empRepo.ExisteEmpleadoAsync(id))
                return NotFound();

            var empleado = await _empRepo.GetEmpleadoAsync(id);
            return Ok(MapearEmpleadoDto(empleado));
        }

        // GET: flowfood/Empleado/BuscarPorCodigo/{codigo}
        [HttpGet("BuscarPorCodigo/{codigo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

            // Subir imagen directamente a Cloudinary
            var uploadResult = await SubirImagenACloudinaryAsync(crearEmpleadoDto.Foto);
            if (uploadResult == null || uploadResult.Error != null)
            {
                ModelState.AddModelError("", "Error al subir la imagen al servidor en la nube");
                return StatusCode(500, ModelState);
            }

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
                FotoUrl = uploadResult.SecureUrl.ToString(), // URL HTTPS accesible desde Angular
                Estado = true,
                puestoId = crearEmpleadoDto.PuestoId
            };

            if (!await _empRepo.CrearEmpleadoAsync(nuevoEmpleado))
            {
                // Si falla en BD, eliminamos la imagen subida a Cloudinary
                await BorrarImagenDeCloudinaryAsync(uploadResult.PublicId);

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

            var rutaFotoFinal = empleadoActual.FotoUrl;
            string? publicIdFotoVieja = null;
            string? publicIdFotoNueva = null;

            if (actualizarDto.Foto != null && actualizarDto.Foto.Length > 0)
            {
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(actualizarDto.Foto.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("", "Formato de imagen no permitido. Solo JPG o PNG");
                    return StatusCode(400, ModelState);
                }

                // Subir nueva foto
                var uploadResult = await SubirImagenACloudinaryAsync(actualizarDto.Foto);
                if (uploadResult == null || uploadResult.Error != null)
                {
                    ModelState.AddModelError("", "Error al subir la nueva imagen a la nube");
                    return StatusCode(500, ModelState);
                }

                publicIdFotoVieja = ExtraerPublicId(empleadoActual.FotoUrl);
                publicIdFotoNueva = uploadResult.PublicId;
                rutaFotoFinal = uploadResult.SecureUrl.ToString();
            }

            var empleadoActualizar = new Empleado
            {
                Id = actualizarDto.Id,
                Nombre = actualizarDto.Nombre,
                Direccion = actualizarDto.Direccion,
                Telefono = actualizarDto.Telefono,
                Edad = actualizarDto.Edad,
                SalarioSemanal = actualizarDto.SalarioSemanal,
                Codigo = empleadoActual.Codigo,
                FechaContrato = actualizarDto.FechaContrato,
                FechaRegistro = empleadoActual.FechaRegistro,
                FotoUrl = rutaFotoFinal,
                Estado = actualizarDto.Estado,
                puestoId = actualizarDto.PuestoId
            };

            if (!await _empRepo.ActualizarEmpleadoAsync(empleadoActualizar))
            {
                // Si falla BD y se subió foto nueva, borramos la recién subida
                if (!string.IsNullOrEmpty(publicIdFotoNueva))
                    await BorrarImagenDeCloudinaryAsync(publicIdFotoNueva);

                ModelState.AddModelError("", $"Algo salió mal actualizando el registro de {empleadoActualizar.Nombre}");
                return StatusCode(500, ModelState);
            }

            // Si la BD actualizó correctamente, eliminamos la anterior de Cloudinary
            if (!string.IsNullOrEmpty(publicIdFotoVieja))
                await BorrarImagenDeCloudinaryAsync(publicIdFotoVieja);

            return Ok(MapearEmpleadoDto(empleadoActualizar));
        }

        // DELETE: flowfood/Empleado/Eliminar/{id}
        [HttpDelete("Eliminar/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            // Eliminar de Cloudinary usando el PublicId extraído de la URL
            if (!string.IsNullOrEmpty(empleadoAEliminar.FotoUrl))
            {
                var publicId = ExtraerPublicId(empleadoAEliminar.FotoUrl);
                if (!string.IsNullOrEmpty(publicId))
                    await BorrarImagenDeCloudinaryAsync(publicId);
            }

            return NoContent();
        }

        // POST: flowfood/Empleado/SubirFoto (Opcional por si lo consumes individualmente)
        [HttpPost("SubirFoto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubirFoto(IFormFile foto)
        {
            if (foto == null || foto.Length == 0)
                return BadRequest("No se recibió ninguna imagen");

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
                return BadRequest("Formato de imagen no permitido. Solo JPG o PNG");

            var result = await SubirImagenACloudinaryAsync(foto);
            if (result == null || result.Error != null)
                return StatusCode(500, "Error al subir la imagen");

            return Ok(new { fotoUrl = result.SecureUrl.ToString() });
        }

        #region Métodos Auxiliares Cloudinary

        private async Task<ImageUploadResult?> SubirImagenACloudinaryAsync(IFormFile archivo)
        {
            await using var stream = archivo.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(archivo.FileName, stream),
                Folder = "empleados", // Crea la carpeta "empleados" en Cloudinary
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }

        private async Task BorrarImagenDeCloudinaryAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deletionParams);
        }

        // Extrae el PublicId necesario para borrar (ejemplo: "empleados/imagen123")
        // Reemplaza tu método ExtraerPublicId actual por este:
        private string? ExtraerPublicId(string? url)
        {
            if (string.IsNullOrEmpty(url) || !url.Contains("cloudinary.com"))
                return null;

            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');

                // Buscamos el índice donde se encuentra "upload"
                var uploadIndex = Array.FindIndex(segments, s => s.Equals("upload", StringComparison.OrdinalIgnoreCase));
                if (uploadIndex == -1 || uploadIndex + 1 >= segments.Length)
                    return null;

                // Los segmentos después de "upload" pueden incluir opcionalmente la versión (ej. v123456)
                var startIndex = uploadIndex + 1;
                if (segments[startIndex].StartsWith("v") && long.TryParse(segments[startIndex][1..], out _))
                {
                    startIndex++; // Saltamos la versión si existe
                }

                if (startIndex >= segments.Length)
                    return null;

                // Unimos lo que resta y removemos la extensión del archivo (.jpg, .png, etc.)
                var publicIdWithExt = string.Join("/", segments.Skip(startIndex));
                return Path.ChangeExtension(publicIdWithExt, null);
            }
            catch
            {
                return null;
            }
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

        #endregion
    }
}