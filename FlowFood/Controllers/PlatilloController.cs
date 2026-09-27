using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace FlowFood.Controllers
{
    [Route("flowfood/[controller]")]
    [ApiController]
    public class PlatilloController : ControllerBase
    {
        private readonly IPlatilloRepositorio _platRepo;
        private readonly Cloudinary _cloudinary;

        public PlatilloController(IPlatilloRepositorio platRepo, Cloudinary cloudinary)
        {
            _platRepo = platRepo;
            _cloudinary = cloudinary;
        }

        [HttpGet("Listar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetPlatillos()
        {
            var listaPlatillos = await _platRepo.GetPlatillosAsync();
            var listaDto = listaPlatillos.Select(MapearPlatilloDto).ToList();

            return Ok(listaDto);
        }

        [HttpGet("Buscar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlatillo(int id)
        {
            if (!await _platRepo.ExistePlatilloAsync(id))
                return NotFound();

            var platillo = await _platRepo.GetPlatilloAsync(id);
            return Ok(MapearPlatilloDto(platillo));
        }

        [HttpGet("BuscarPorCodigo/{codigo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlatilloPorCodigo(string codigo)
        {
            if (!await _platRepo.ExistePlatilloXCodigoAsync(codigo))
                return NotFound();

            var platillo = await _platRepo.GetPlatilloXCodigoAsync(codigo);
            return Ok(MapearPlatilloDto(platillo));
        }

        [HttpPost("Guardar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GuardarPlatillo([FromForm] CrearPlatilloDto crearPlatilloDto)
        {
            if (crearPlatilloDto == null)
                return BadRequest(ModelState);

            if (crearPlatilloDto.Foto == null || crearPlatilloDto.Foto.Length == 0)
            {
                ModelState.AddModelError("", "Debe adjuntar una foto del platillo");
                return StatusCode(400, ModelState);
            }

            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(crearPlatilloDto.Foto.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
            {
                ModelState.AddModelError("", "Formato de imagen no permitido. Solo JPG o PNG");
                return StatusCode(400, ModelState);
            }

            // Subir imagen a la carpeta "platillos" en Cloudinary
            var uploadResult = await SubirImagenACloudinaryAsync(crearPlatilloDto.Foto);
            if (uploadResult == null || uploadResult.Error != null)
            {
                ModelState.AddModelError("", "Error al subir la imagen del platillo a la nube");
                return StatusCode(500, ModelState);
            }

            var codigoGenerado = await _platRepo.GenerarSiguienteCodigoAsync();

            var nuevoPlatillo = new Platillo
            {
                Nombre = crearPlatilloDto.Nombre,
                Descripcion = crearPlatilloDto.Descripcion,
                Precio = crearPlatilloDto.Precio,
                Codigo = codigoGenerado,
                FotoUrl = uploadResult.SecureUrl.ToString(),
                FechaRegistro = DateTime.UtcNow,
                Estado = true
            };

            if (!await _platRepo.CrearPlatilloAsync(nuevoPlatillo))
            {
                // Si la BD falla, revertimos y borramos la foto recién subida a Cloudinary
                await BorrarImagenDeCloudinaryAsync(uploadResult.PublicId);

                ModelState.AddModelError("", $"Algo salió mal al guardar el registro de {nuevoPlatillo.Nombre}");
                return StatusCode(500, ModelState);
            }

            return Ok(MapearPlatilloDto(nuevoPlatillo));
        }

        [HttpPut("Actualizar/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarPlatillo(int id, [FromForm] ActualizarPlatilloDto actualizarDto)
        {
            if (actualizarDto == null || id != actualizarDto.Id)
                return BadRequest(ModelState);

            if (!await _platRepo.ExistePlatilloAsync(id))
                return NotFound();

            var platilloActual = await _platRepo.GetPlatilloAsync(id);

            var rutaFotoFinal = platilloActual.FotoUrl;
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

                // Subir nueva foto a Cloudinary
                var uploadResult = await SubirImagenACloudinaryAsync(actualizarDto.Foto);
                if (uploadResult == null || uploadResult.Error != null)
                {
                    ModelState.AddModelError("", "Error al subir la nueva imagen a la nube");
                    return StatusCode(500, ModelState);
                }

                publicIdFotoVieja = ExtraerPublicId(platilloActual.FotoUrl);
                publicIdFotoNueva = uploadResult.PublicId;
                rutaFotoFinal = uploadResult.SecureUrl.ToString();
            }

            var platilloActualizar = new Platillo
            {
                Id = actualizarDto.Id,
                Nombre = actualizarDto.Nombre,
                Descripcion = actualizarDto.Descripcion,
                Precio = actualizarDto.Precio,
                Codigo = platilloActual.Codigo,
                FotoUrl = rutaFotoFinal,
                FechaRegistro = platilloActual.FechaRegistro,
                Estado = actualizarDto.Estado
            };

            if (!await _platRepo.ActualizarPlatilloAsync(platilloActualizar))
            {
                // Si la BD falló, destruimos la foto nueva recién subida
                if (!string.IsNullOrEmpty(publicIdFotoNueva))
                    await BorrarImagenDeCloudinaryAsync(publicIdFotoNueva);

                ModelState.AddModelError("", $"Algo salió mal actualizando el registro de {platilloActualizar.Nombre}");
                return StatusCode(500, ModelState);
            }

            // Si la BD actualizó correctamente, eliminamos la foto anterior de Cloudinary
            if (!string.IsNullOrEmpty(publicIdFotoVieja))
                await BorrarImagenDeCloudinaryAsync(publicIdFotoVieja);

            return Ok(MapearPlatilloDto(platilloActualizar));
        }

        [HttpDelete("Eliminar/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EliminarPlatillo(int id)
        {
            if (!await _platRepo.ExistePlatilloAsync(id))
                return NotFound();

            var platilloAEliminar = await _platRepo.GetPlatilloAsync(id);

            if (!await _platRepo.BorrarPlatilloAsync(platilloAEliminar))
            {
                ModelState.AddModelError("", $"Algo salió mal borrando el registro de {platilloAEliminar.Nombre}");
                return StatusCode(500, ModelState);
            }

            // Borrar de Cloudinary usando su URL
            if (!string.IsNullOrEmpty(platilloAEliminar.FotoUrl))
            {
                var publicId = ExtraerPublicId(platilloAEliminar.FotoUrl);
                if (!string.IsNullOrEmpty(publicId))
                    await BorrarImagenDeCloudinaryAsync(publicId);
            }

            return NoContent();
        }

        #region Métodos Auxiliares Cloudinary

        private async Task<ImageUploadResult?> SubirImagenACloudinaryAsync(IFormFile archivo)
        {
            await using var stream = archivo.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(archivo.FileName, stream),
                Folder = "platillos", // Asigna la carpeta "platillos" en Cloudinary
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            return await _cloudinary.UploadAsync(uploadParams);
        }

        private async Task BorrarImagenDeCloudinaryAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deletionParams);
        }

        private string? ExtraerPublicId(string? url)
        {
            if (string.IsNullOrEmpty(url) || !url.Contains("cloudinary.com"))
                return null;

            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/');
                var index = Array.FindIndex(segments, s => s.StartsWith("v") && long.TryParse(s[1..], out _));
                if (index != -1 && index + 1 < segments.Length)
                {
                    var pathWithExt = string.Join("/", segments.Skip(index + 1));
                    return Path.ChangeExtension(pathWithExt, null);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private PlatilloDto MapearPlatilloDto(Platillo platillo)
        {
            return new PlatilloDto
            {
                Id = platillo.Id,
                Nombre = platillo.Nombre,
                Descripcion = platillo.Descripcion,
                Precio = platillo.Precio,
                Codigo = platillo.Codigo,
                FotoUrl = platillo.FotoUrl,
                FechaRegistro = platillo.FechaRegistro,
                Estado = platillo.Estado
            };
        }

        #endregion
    }
}