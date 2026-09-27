using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class ProductoController : ControllerBase
  {
    private readonly IProductoRepositorio _prodRepo;
    private readonly IUnidadMedidaRepositorio _uniRepo;

    public ProductoController(IProductoRepositorio prodRepo, IUnidadMedidaRepositorio uniRepo)
    {
      _prodRepo = prodRepo;
      _uniRepo = uniRepo;
    }
    
    // GET: flowfood/Producto/Listar
    [HttpGet("Listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetProductos()
    {
      var listaProductos = await _prodRepo.GetProductosAsync();
      var listaDto = new List<ProductoDto>();

      foreach (var item in listaProductos)
      {
        listaDto.Add(await MapearProductoDtoAsync(item));
      }

      return Ok(listaDto);
    }

    [HttpGet("ListarActivos")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProductosActivosAsync()
    {
      // Obtenemos todos los productos
      var listaProductos = await _prodRepo.GetProductosAsync();
      var listaProductosDto = new List<ProductoDto>();

      foreach (var producto in listaProductos.Where(p => p.Estado == true))
      {
        var productoDto = new ProductoDto
        {
          Id = producto.Id,
          Nombre = producto.Nombre,
          Descripcion = producto.Descripcion,
          FechaRegistro = producto.FechaRegistro,
          Estado = producto.Estado
        };
        listaProductosDto.Add(productoDto);
      }
      return Ok(listaProductosDto);
    }

    // GET: flowfood/Producto/Buscar/{id}
    [HttpGet("Buscar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProducto(int id)
    {
      if (!await _prodRepo.ExisteProductoAsync(id))
        return NotFound();

      var producto = await _prodRepo.GetProductoAsync(id);
      return Ok(await MapearProductoDtoAsync(producto));
    }

    // GET: flowfood/Producto/BuscarPorNombre/{nombre}
    [HttpGet("BuscarPorNombre/{nombre}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductoPorNombre(string nombre)
    {
      if (!await _prodRepo.ExisteProductoXNombreAsync(nombre))
        return NotFound();

      var producto = await _prodRepo.GetProductoXNombreAsync(nombre);
      return Ok(await MapearProductoDtoAsync(producto));
    }

    // POST: flowfood/Producto/Guardar
    [HttpPost("Guardar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GuardarProducto([FromBody] CrearProductoDto crearProductoDto)
    {
      if (crearProductoDto == null)
        return BadRequest(ModelState);

      // Validamos que la Unidad de Medida exista
      if (!await _uniRepo.ExisteUnidadMedidaAsync(crearProductoDto.unidadId))
      {
        ModelState.AddModelError("", "La unidad de medida especificada no existe.");
        return StatusCode(404, ModelState);
      }

      if (await _prodRepo.ExisteProductoXNombreAsync(crearProductoDto.Nombre))
      {
        ModelState.AddModelError("", "Ya existe un producto con ese nombre");
        return StatusCode(400, ModelState);
      }

      var nuevoProducto = new Producto
      {
        Nombre = crearProductoDto.Nombre,
        Descripcion = crearProductoDto.Descripcion,
        unidadId = crearProductoDto.unidadId,
        FechaRegistro = DateTime.UtcNow,
        Estado = true
      };

      if (!await _prodRepo.CrearProductoAsync(nuevoProducto))
      {
        ModelState.AddModelError("", $"Algo salió mal al guardar el registro de {nuevoProducto.Nombre}");
        return StatusCode(500, ModelState);
      }

      // Recien creado, aun no tiene registros en Inventario -> CantidadTotal = 0
      return Ok(await MapearProductoDtoAsync(nuevoProducto));
    }

    // PUT: flowfood/Producto/Actualizar/{id}
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarProducto(int id, [FromBody] ProductoDto productoDto)
    {
      if (productoDto == null || id != productoDto.Id)
        return BadRequest(ModelState);

      if (!await _prodRepo.ExisteProductoAsync(id))
        return NotFound();

      if (!await _uniRepo.ExisteUnidadMedidaAsync(productoDto.unidadId))
      {
        ModelState.AddModelError("", "La Unidad de medida especificada no existe.");
        return StatusCode(404, ModelState);
      }

      var productoActual = await _prodRepo.GetProductoAsync(id);

      var productoActualizar = new Producto
      {
        Id = productoDto.Id,
        Nombre = productoDto.Nombre,
        Descripcion = productoDto.Descripcion,
        unidadId = productoDto.unidadId,
        FechaRegistro = productoActual.FechaRegistro, // no se debe modificar
        Estado = productoDto.Estado
      };

      if (!await _prodRepo.ActualizarProductoAsync(productoActualizar))
      {
        ModelState.AddModelError("", $"Algo salió mal actualizando el registro de {productoActualizar.Nombre}");
        return StatusCode(500, ModelState);
      }

      return Ok(await MapearProductoDtoAsync(productoActualizar));
    }

    // DELETE: flowfood/Producto/Eliminar/{id}
    [HttpDelete("Eliminar/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EliminarProducto(int id)
    {
      if (!await _prodRepo.ExisteProductoAsync(id))
        return NotFound();

      var productoAEliminar = await _prodRepo.GetProductoAsync(id);

      if (!await _prodRepo.BorrarProductoAsync(productoAEliminar))
      {
        ModelState.AddModelError("", $"Algo salió mal borrando el registro de {productoAEliminar.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

    private async Task<ProductoDto> MapearProductoDtoAsync(Producto producto)
    {
      return new ProductoDto
      {
        Id = producto.Id,
        Nombre = producto.Nombre,
        Descripcion = producto.Descripcion,
        unidadId = producto.unidadId,
        unidadNombre = producto.UnidadMedida?.Nombre ?? "Sin asignar",
        FechaRegistro = producto.FechaRegistro,
        Estado = producto.Estado
      };
    }
  }
}
