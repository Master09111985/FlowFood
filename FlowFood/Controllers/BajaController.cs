using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class BajaController : ControllerBase
  {
    private readonly IBajaRepositorio _bajaRepo;
    private readonly IInventarioRepositorio _invRepo;
    private readonly IMotivoBajaRepositorio _motivoRepo;

    public BajaController(
        IBajaRepositorio bajaRepo,
        IInventarioRepositorio invRepo,
        IMotivoBajaRepositorio motivoRepo)
    {
      _bajaRepo = bajaRepo;
      _invRepo = invRepo;
      _motivoRepo = motivoRepo;
    }

    [HttpGet("Listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBajas()
    {
      var lista = await _bajaRepo.GetBajasAsync();
      return Ok(lista.Select(MapearBajaDto).ToList());
    }

    [HttpGet("Buscar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBaja(int id)
    {
      var baja = await _bajaRepo.GetBajaAsync(id);
      if (baja == null)
        return NotFound();

      return Ok(MapearBajaDto(baja));
    }

    [HttpPost("Guardar")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GuardarBaja([FromBody] CrearBajaDto bajaDto)
    {
      if (bajaDto == null || !ModelState.IsValid)
        return BadRequest(ModelState);

      // 1. Validar que el motivo de baja exista y esté activo
      var motivo = await _motivoRepo.GetMotivoAsync(bajaDto.MotivoBajaId);
      if (motivo == null || !motivo.Estado)
      {
        ModelState.AddModelError("", "El motivo de baja especificado no existe o está inactivo.");
        return StatusCode(404, ModelState);
      }

      // 2. Buscar el registro de inventario usando Producto y Almacén
      var inventario = await _invRepo.GetInventarioXProductoYAlmacenAsync(bajaDto.ProductoId, bajaDto.AlmacenId);

      if (inventario == null)
      {
        ModelState.AddModelError("", "El producto no se encuentra registrado en el almacén especificado.");
        return StatusCode(404, ModelState);
      }

      // 3. Validar que la cantidad en inventario sea suficiente
      if (inventario.Cantidad < bajaDto.Cantidad)
      {
        ModelState.AddModelError("", $"No hay suficientes existencias. Disponibles: {inventario.Cantidad}");
        return StatusCode(400, ModelState);
      }

      // 4. Descontar la cantidad del inventario utilizando el método que ya tenías
      if (!await _invRepo.DescontarCantidadAsync(inventario.Id, bajaDto.Cantidad))
      {
        ModelState.AddModelError("", "Ocurrió un error al intentar descontar las existencias del inventario.");
        return StatusCode(500, ModelState);
      }

      // 5. Crear el registro en la bitácora de bajas
      var nuevaBaja = new Baja
      {
        InventarioId = inventario.Id, // El puente del que hablamos
        MotivoBajaId = bajaDto.MotivoBajaId,
        Cantidad = bajaDto.Cantidad,
        FechaBaja = DateTime.UtcNow,
        Comentarios = bajaDto.Comentarios
      };

      if (!await _bajaRepo.CrearBajaAsync(nuevaBaja))
      {
        // Si llegamos a este punto y falla, el inventario ya se descontó. 
        // En una API más robusta, aquí usaríamos una Transacción de SQL (IDbContextTransaction)
        ModelState.AddModelError("", "Se descontó el inventario, pero ocurrió un error al guardar la bitácora de la baja.");
        return StatusCode(500, ModelState);
      }

      // Retornar el DTO mapeado de la baja recién creada
      var bajaCreada = await _bajaRepo.GetBajaAsync(nuevaBaja.Id);
      return Ok(MapearBajaDto(bajaCreada));
    }

    // Método privado para mantener el código limpio al mapear el DTO aplanado
    private BajaDto MapearBajaDto(Baja baja)
    {
      return new BajaDto
      {
        Id = baja.Id,
        Cantidad = baja.Cantidad,
        FechaBaja = baja.FechaBaja,
        Comentarios = baja.Comentarios ?? "", // Si es nulo, enviamos cadena vacía
        InventarioId = baja.InventarioId,
        ProductoNombre = baja.Inventario?.Producto?.Nombre ?? "Desconocido",
        AlmacenNombre = baja.Inventario?.Almacen?.Nombre ?? "Desconocido",
        UnidadMedidaNombre = baja.Inventario?.Producto?.UnidadMedida?.Nombre ?? "Desconocida",
        MotivoBajaId = baja.MotivoBajaId,
        MotivoBajaNombre = baja.MotivoBaja?.Nombre ?? "Desconocido"
      };
    }
  }
}
