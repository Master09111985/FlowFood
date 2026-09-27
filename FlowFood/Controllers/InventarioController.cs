using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class InventarioController : ControllerBase
  {
    private readonly IInventarioRepositorio _invRepo;

    public InventarioController(IInventarioRepositorio invRepo)
    {
      _invRepo = invRepo;
    }

    // GET: flowfood/Inventario/Listar
    [HttpGet("Listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetInventarios()
    {
      var lista = await _invRepo.GetInventariosAsync();
      return Ok(lista.Select(MapearInventarioDto).ToList());
    }

    // GET: flowfood/Inventario/Buscar/{id}
    [HttpGet("Buscar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInventario(int id)
    {
      if (!await _invRepo.ExisteInventarioAsync(id))
        return NotFound();

      var inventario = await _invRepo.GetInventarioAsync(id);
      return Ok(MapearInventarioDto(inventario));
    }

    // GET: flowfood/Inventario/PorAlmacen/{almacenId}
    [HttpGet("PorAlmacen/{almacenId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetInventariosPorAlmacen(int almacenId)
    {
      var lista = await _invRepo.GetInventariosPorAlmacenAsync(almacenId);
      return Ok(lista.Select(MapearInventarioDto).ToList());
    }

    // GET: flowfood/Inventario/PorProducto/{productoId}
    [HttpGet("PorProducto/{productoId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetInventariosPorProducto(int productoId)
    {
      var lista = await _invRepo.GetInventariosPorProductoAsync(productoId);
      return Ok(lista.Select(MapearInventarioDto).ToList());
    }

    // POST: flowfood/Inventario/Guardar
    [HttpPost("Guardar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GuardarInventario([FromBody] CrearInventarioDto crearInventarioDto)
    {
      if (crearInventarioDto == null)
        return BadRequest(ModelState);

      var inventarioExistente = await _invRepo.GetInventarioXProductoYAlmacenAsync(
          crearInventarioDto.ProductoId, crearInventarioDto.AlmacenId);

      // Caso 1: ya existe el producto en ese almacen -> sumamos la cantidad
      if (inventarioExistente != null)
      {
        // La unidad de medida pertenece al producto, la validamos consultando la tabla anidada
        if (inventarioExistente.Producto != null && inventarioExistente.Producto.unidadId != crearInventarioDto.UnidadMedidaId)
        {
          ModelState.AddModelError("", $"Este producto esta registrado con una unidad de medida distinta. Ajuste la unidad antes de continuar.");
          return StatusCode(400, ModelState);
        }

        inventarioExistente.Cantidad += crearInventarioDto.Cantidad;

        if (!await _invRepo.ActualizarInventarioAsync(inventarioExistente))
        {
          ModelState.AddModelError("", "Algo salio mal actualizando las existencias del inventario");
          return StatusCode(500, ModelState);
        }

        return Ok(MapearInventarioDto(inventarioExistente));
      }


      // Caso 2: no existe todavia -> se crea el registro nuevo
      var nuevoInventario = new Inventario
      {
        Cantidad = crearInventarioDto.Cantidad,
        productoId = crearInventarioDto.ProductoId,
        almacenId = crearInventarioDto.AlmacenId
      };

      if (!await _invRepo.CrearInventarioAsync(nuevoInventario))
      {
        ModelState.AddModelError("", "Algo salio mal al guardar el registro de inventario");
        return StatusCode(500, ModelState);
      }

      var inventarioCreado = await _invRepo.GetInventarioAsync(nuevoInventario.Id);
      return Ok(MapearInventarioDto(inventarioCreado));
    }

    // PUT: flowfood/Inventario/Actualizar/{id}
    // Para correcciones manuales directas (ej. ajuste de un conteo fisico)
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarInventario(int id, [FromBody] InventarioDto inventarioDto)
    {
      if (inventarioDto == null || id != inventarioDto.Id)
        return BadRequest(ModelState);

      if (!await _invRepo.ExisteInventarioAsync(id))
        return NotFound();

      var inventarioActualizar = new Inventario
      {
        Id = inventarioDto.Id,
        Cantidad = inventarioDto.Cantidad,
        productoId = inventarioDto.ProductoId,
        almacenId = inventarioDto.AlmacenId
      };

      if (!await _invRepo.ActualizarInventarioAsync(inventarioActualizar))
      {
        ModelState.AddModelError("", "Algo salió mal actualizando el registro de inventario");
        return StatusCode(500, ModelState);
      }

      return Ok(inventarioDto);
    }

    // DELETE: flowfood/Inventario/Eliminar/{id}
    [HttpDelete("Eliminar/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EliminarInventario(int id)
    {
      if (!await _invRepo.ExisteInventarioAsync(id))
        return NotFound();

      var inventarioAEliminar = await _invRepo.GetInventarioAsync(id);

      if (!await _invRepo.BorrarInventarioAsync(inventarioAEliminar))
      {
        ModelState.AddModelError("", "Algo salió mal borrando el registro de inventario");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

    private InventarioDto MapearInventarioDto(Inventario inventario)
    {
      return new InventarioDto
      {
        Id = inventario.Id,
        Cantidad = inventario.Cantidad,
        ProductoId = inventario.productoId,
        ProductoNombre = inventario.Producto?.Nombre,
        AlmacenId = inventario.almacenId,
        AlmacenNombre = inventario.Almacen?.Nombre,
        UnidadMedidaId = inventario.Producto != null ? inventario.Producto.unidadId : 0,
        UnidadMedidaNombre = inventario.Producto?.UnidadMedida?.Nombre
      };
    }

    // POST: flowfood/Inventario/Trasladar
    [HttpPost("Trasladar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TrasladarInventario([FromBody] TrasladoInventarioDto trasladoDto)
    {
      if (trasladoDto == null || !ModelState.IsValid)
        return BadRequest(ModelState);

      if (trasladoDto.AlmacenOrigenId == trasladoDto.AlmacenDestinoId)
        return BadRequest("El almacen de origen y destino no pueden ser el mismo.");

      // 1. Validar que exista en el almacen de origen y tenga cantidad suficiente
      var inventarioOrigen = await _invRepo.GetInventarioXProductoYAlmacenAsync(trasladoDto.ProductoId, trasladoDto.AlmacenOrigenId);

      if (inventarioOrigen == null)
        return NotFound("El producto no existe en el almacen origen.");

      if (inventarioOrigen.Cantidad < trasladoDto.Cantidad)
      {
        ModelState.AddModelError("", $"Cantidad insuficiente. Solo hay {inventarioOrigen.Cantidad} disponibles en origen");
        return StatusCode(400, ModelState);
      }

      // 2. Descontar del origen
      inventarioOrigen.Cantidad -= trasladoDto.Cantidad;
      if (!await _invRepo.ActualizarInventarioAsync(inventarioOrigen))
        return StatusCode(500, "Error al descontar existencias del almacen origen.");

      // 3. Sumar o crear en el destino
      var inventarioDestino = await _invRepo.GetInventarioXProductoYAlmacenAsync(trasladoDto.ProductoId, trasladoDto.AlmacenDestinoId);

      if (inventarioDestino != null)
      {
        // Ya existe, solo sumamos
        inventarioDestino.Cantidad += trasladoDto.Cantidad;
        if (!await _invRepo.ActualizarInventarioAsync(inventarioDestino))
          return StatusCode(500, "Error al sumar existencias en el almacen destino.");
      }
      else
      {
        // No existe, creamos el registro nuevo
        var nuevoInventario = new Inventario
        {
          productoId = trasladoDto.ProductoId,
          almacenId = trasladoDto.AlmacenDestinoId,
          Cantidad = trasladoDto.Cantidad
        };
        if (!await _invRepo.CrearInventarioAsync(nuevoInventario))
          return StatusCode(500, "Error al crear el nuevo registro en el almacen destino.");
      }

      return Ok(new { mensaje = "Traslado realizado con exito." });
    }
  }
}
