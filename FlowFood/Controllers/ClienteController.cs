using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/[controller]")]
  [ApiController]
  public class ClienteController : ControllerBase
  {
    private readonly IClienteRepositorio _cliRepo;

    public ClienteController(IClienteRepositorio cliRepo)
    {
      _cliRepo = cliRepo;
    }

    // GET: flowfood/Cliente/Listar
    [HttpGet("Listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetClientes()
    {
      var listaClientes = await _cliRepo.GetClientesAsync();
      var listaDto = new List<ClienteDto>();

      foreach (var item in listaClientes)
      {
        listaDto.Add(new ClienteDto
        {
          Id = item.Id,
          Nombre = item.Nombre,
          Telefono = item.Telefono,
          Direccion = item.Direccion,
          Referencias = item.Referencias
        });
      }

      return Ok(listaDto);
    }

    // GET: flowfood/Cliente/BuscarPorTelefono/{telefono}
    [HttpGet("BuscarPorTelefono/{telefono}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClientePorTelefono(string telefono)
    {
      if (!await _cliRepo.ExisteClienteXTelefonoAsync(telefono))
        return NotFound();

      var cliente = await _cliRepo.GetClienteXTelefonoAsync(telefono);
      var clienteDto = new ClienteDto
      {
        Id = cliente.Id,
        Nombre = cliente.Nombre,
        Telefono = cliente.Telefono,
        Direccion = cliente.Direccion,
        Referencias = cliente.Referencias
      };

      return Ok(clienteDto);
    }

    // POST: flowfood/Cliente/Guardar
    [HttpPost("Guardar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GuardarCliente([FromBody] CrearClienteDto crearClienteDto)
    {
      if (crearClienteDto == null)
        return BadRequest(ModelState);

      // Validamos que no se repita el numero de telefono
      if (await _cliRepo.ExisteClienteXTelefonoAsync(crearClienteDto.Telefono))
      {
        ModelState.AddModelError("", "Ya existe un cliente con ese número de teléfono");
        return StatusCode(400, ModelState);
      }

      var nuevoCliente = new Cliente
      {
        Nombre = crearClienteDto.Nombre,
        Telefono = crearClienteDto.Telefono,
        Direccion = crearClienteDto.Direccion,
        Referencias = crearClienteDto.Referencias
      };

      if (!await _cliRepo.CrearClienteAsync(nuevoCliente))
      {
        ModelState.AddModelError("", $"Algo salió mal al guardar el registro de {nuevoCliente.Nombre}");
        return StatusCode(500, ModelState);
      }

      return Ok(nuevoCliente);
    }

    // PUT: flowfood/Cliente/Actualizar/{id}
    [HttpPut("Actualizar/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActualizarCliente(int id, [FromBody] ClienteDto clienteDto)
    {
      if (clienteDto == null || id != clienteDto.Id)
        return BadRequest(ModelState);

      var clienteActual = await _cliRepo.GetClienteAsync(id);

      if (clienteActual == null)
        return NotFound();

      clienteActual.Nombre = clienteDto.Nombre;
      clienteActual.Telefono = clienteDto.Telefono;
      clienteActual.Direccion = clienteDto.Direccion;
      clienteActual.Referencias = clienteDto.Referencias;

      if (!await _cliRepo.ActualizarClienteAsync(clienteActual))
      {
        ModelState.AddModelError("", $"Algo salió mal actualizando el registro de {clienteActual.Nombre}");
        return StatusCode(500, ModelState);
      }

      return Ok(clienteActual);
    }

    // DELETE: flowfood/Cliente/Eliminar/{id}
    [HttpDelete("Eliminar/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EliminarCliente(int id)
    {
      if (!await _cliRepo.ExisteClienteAsync(id))
        return NotFound();

      var clienteAEliminar = await _cliRepo.GetClienteAsync(id);

      if (!await _cliRepo.BorrarClienteAsync(clienteAEliminar))
      {
        ModelState.AddModelError("", $"Algo salió mal borrando el registro de {clienteAEliminar.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }
  }
}
