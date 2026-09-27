using FlowFood.Models.Dtos;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class CategoriaController : ControllerBase
  {
    private readonly ICategoriaRepositorio _caRepo;
    public CategoriaController(ICategoriaRepositorio caRepo)
    {
        _caRepo = caRepo;
    }

    [HttpGet("listarcategorias")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategorias()
    {
      var listaCategorias = await _caRepo.GetCategoriasAsync();
      var listaCategoriasDto = new List<CategoriaDto>();

      foreach (var categoria in listaCategorias)
      {
        var categoriaDto = new CategoriaDto
        {
          Id = categoria.Id,
          Nombre = categoria.Nombre,
          Descripcion = categoria.Descripcion,
          Estado = categoria.Estado
        };
        listaCategoriasDto.Add(categoriaDto);
      }
      return Ok(listaCategoriasDto);
    }

    [HttpGet("listarcategoriasactivas")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetCategoriasActivasAsync()
    {
      // Obtenemos todas las categorias
      var listaCategorias = await _caRepo.GetCategoriasAsync();
      var listaCategoriasDto = new List<CategoriaDto>();

      foreach (var categoria in listaCategorias)
      {
        var categoriaDto = new CategoriaDto
        {
          Id = categoria.Id,
          Nombre = categoria.Nombre,
          Descripcion = categoria.Descripcion,
          Estado= categoria.Estado
        };
        listaCategoriasDto.Add(categoriaDto);
      }
      return Ok(listaCategoriasDto);
    }

    [HttpPost("crearcategoria")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (crearCategoriaDto == null)
        return BadRequest(ModelState);

      if (await _caRepo.ExisteCategoriaXNombreAsync(crearCategoriaDto.Nombre))
      {
        ModelState.AddModelError("", "La categoria ya existe.");
        return StatusCode(404, ModelState);
      }

      var categoria = new Categoria
      {
        Nombre = crearCategoriaDto.Nombre,
        Descripcion = crearCategoriaDto.Descripcion,
        Estado = crearCategoriaDto.Estado
      };

      if (!await _caRepo.CrearCategoriaAsync(categoria))
      {
        ModelState.AddModelError("", $"Algo salio mal guardando el registro. {categoria.Nombre}");
        return StatusCode(500, ModelState);
      }
      return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);
    }

    [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (categoriaDto == null || categoriaId != categoriaDto.Id)
        return BadRequest(ModelState);

      var categoria = new Categoria
      {
        Nombre = categoriaDto.Nombre,
        Descripcion = categoriaDto.Descripcion,
        Estado = categoriaDto.Estado
      };

      if (!await _caRepo.ActualizarCategoriaAsync(categoria))
      {
        ModelState.AddModelError("", $"Algo salio mal actualizando el registro { categoria.Nombre }");
        return StatusCode(500, ModelState);
      }
      return NoContent();
    }

    [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BorrarCategoria(int categoriaId)
    {
      if (!await _caRepo.ExisteCategoriaAsync(categoriaId))
        return NotFound();

      var categoria = await _caRepo.GetCategoriaAsync(categoriaId);

      if (!await _caRepo.BorrarCategoriaAsync(categoria))
      {
        ModelState.AddModelError("", $"Algo salio mal borrando el registro. { categoria.Nombre }");
        return StatusCode(500, ModelState);
      }
      return NoContent();
    }
  }
}
