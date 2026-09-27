using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Mvc;

namespace FlowFood.Controllers
{
  [Route("flowfood/comanda")]
  [ApiController]
  public class ComandaController : ControllerBase
  {
    private readonly IComandaRepositorio _coRepo;

    public ComandaController(IComandaRepositorio coRepo)
    {
      _coRepo = coRepo;
    }

    // ==========================================
    // GET: Listar Todas (CON DESPERTADOR DE AGENDADOS)
    // ==========================================
    [HttpGet("listar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetComandas()
    {
      var listaComandas = await _coRepo.GetComandasAsync();
      bool huboCambios = false;

      // Fijamos la hora exacta de Aguascalientes (UTC-6) de forma permanente
      var horaActual = DateTime.UtcNow.AddHours(-6); //DateTime.Now

      // 1. REVISAMOS EL RELOJ PARA LOS PEDIDOS AGENDADOS
      foreach (var c in listaComandas)
      {
        // Si está en Estatus 0 (Agendado) y tiene hora programada
        if (c.Estatus == 0 && c.FechaHoraAgendada.HasValue)
        {
          // Calculamos la diferencia de tiempo entre la entrega y la hora fijada
          var tiempoFaltante = c.FechaHoraAgendada.Value - horaActual;

          // Si faltan 60 minutos o menos, o si la hora ya pasó (tiempoFaltante negativo)
          if (tiempoFaltante.TotalMinutes <= 60)
          {
            c.Estatus = 1; // ¡Lo mandamos a Cocinando!
            await _coRepo.ActualizarComandaAsync(c);
            huboCambios = true;
          }
        }
      }

      // 2. Si despertamos algún pedido, volvemos a consultar la DB para tener la lista fresca
      if (huboCambios)
      {
        listaComandas = await _coRepo.GetComandasAsync();
      }

      var listaDto = listaComandas
        .OrderByDescending(c => c.Prioridad)
        .ThenBy(c => c.FechaRegistro)
        .Select(MapearComandaDto).ToList();
      return Ok(listaDto);
    }

    // ==========================================
    // GET: Buscar por ID
    // ==========================================
    [HttpGet("buscar/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetComanda(int id)
    {
      if (!await _coRepo.ExisteComandaAsync(id))
        return NotFound();

      var comanda = await _coRepo.GetComandaAsync(id);
      return Ok(MapearComandaDto(comanda));
    }

    // ==========================================
    // POST: Crear Comanda
    // ==========================================
    [HttpPost("crearcomanda")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CrearComanda([FromBody] CrearComandaDto crearDto)
    {
      if (crearDto == null || !ModelState.IsValid)
        return BadRequest(ModelState);

      if (crearDto.Detalles == null || crearDto.Detalles.Count == 0)
      {
        ModelState.AddModelError("", "La comanda debe contener al menos un platillo.");
        return StatusCode(400, ModelState);
      }

      // 1. Validaciones Condicionales
      switch (crearDto.TipoPedido)
      {
        case 1: if (string.IsNullOrEmpty(crearDto.NumeroMesa)) return BadRequest("Falta el número de mesa."); break;
        case 2: if (string.IsNullOrEmpty(crearDto.NombreClienteLlevar)) return BadRequest("Falta el nombre del cliente."); break;
        case 3: if (crearDto.ClienteId == null || crearDto.ClienteId <= 0) return BadRequest("Seleccione un cliente para el domicilio."); break;
        case 4:
          if (crearDto.ClienteId == null || crearDto.ClienteId <= 0) return BadRequest("Seleccione un cliente para la entrega.");
          if (crearDto.FechaHoraAgendada == null) return BadRequest("Falta la fecha y hora agendada.");
          break;
        case 5: if (crearDto.PlataformaId == null || crearDto.PlataformaId <= 0) return BadRequest("Seleccione la plataforma de origen."); break;
      }

      // 2. Calcular Totales Seguros
      decimal totalCalculado = 0;
      var detallesList = new List<ComandaDetalle>();

      foreach (var item in crearDto.Detalles)
      {
        var subtotalItem = item.Cantidad * item.PrecioUnitario;
        totalCalculado += subtotalItem;

        detallesList.Add(new ComandaDetalle
        {
          PlatilloId = item.PlatilloId,
          Cantidad = item.Cantidad,
          PrecioUnitario = item.PrecioUnitario,
          Subtotal = subtotalItem,
          Notas = item.Notas,
          NumeroPlato = item.NumeroPlato // <--- INYECTADO AL GUARDAR EN BD
        });
      }

      int estatusInicial = (crearDto.TipoPedido == 4) ? 0 : 1;

      var nuevaComanda = new Comanda
      {
        TipoPedido = crearDto.TipoPedido,
        NumeroMesa = crearDto.NumeroMesa,
        NombreClienteLlevar = crearDto.NombreClienteLlevar,
        ClienteId = crearDto.ClienteId,
        FechaHoraAgendada = crearDto.FechaHoraAgendada,
        PlataformaId = crearDto.PlataformaId,
        Estatus = estatusInicial,
        Subtotal = totalCalculado,
        Total = totalCalculado,
        FechaRegistro = DateTime.UtcNow,
        Detalles = detallesList
      };

      if (!await _coRepo.CrearComandaAsync(nuevaComanda))
      {
        ModelState.AddModelError("", "Ocurrió un error al guardar la comanda.");
        return StatusCode(500, ModelState);
      }

      return Ok(new { mensaje = "Comanda creada exitosamente", comandaId = nuevaComanda.Id, estatus = estatusInicial });
    }

    // ==========================================
    // PATCH: Cambiar Estatus
    // ==========================================
    [HttpPatch("cambiarestatus/{comandaId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CambiarEstatus(int comandaId, [FromBody] int nuevoEstatus)
    {
      if (!await _coRepo.ExisteComandaAsync(comandaId))
        return NotFound();

      if (nuevoEstatus < 0 || nuevoEstatus > 3)
        return BadRequest("El estatus debe estar entre 0 y 3.");

      if (!await _coRepo.ActualizarEstatusComandaAsync(comandaId, nuevoEstatus))
      {
        ModelState.AddModelError("", "Error al actualizar el estatus de la comanda.");
        return StatusCode(500, ModelState);
      }

      return Ok(new { mensaje = "Estatus actualizado correctamente." });
    }

    // ==========================================
    // POST: Agregar platillos a comanda existente
    // ==========================================
    [HttpPost("{comandaId:int}/agregar-platillos")]
    public async Task<IActionResult> AgregarPlatillos(int comandaId, [FromBody] List<CrearComandaDetalleDto> nuevosDetalles)
    {
      var comanda = await _coRepo.GetComandaAsync(comandaId);
      if (comanda == null) return NotFound("Comanda no encontrada.");

      // AJUSTE CRÍTICO: Solo bloqueamos si ya está pagada (Estatus 3). Si está Entregada (2), sí podemos agregar.
      if (comanda.Estatus == 3)
        return BadRequest("No se pueden agregar platillos a una cuenta que ya fue cobrada.");

      decimal totalNuevos = 0;

      foreach (var item in nuevosDetalles)
      {
        var subtotalItem = item.Cantidad * item.PrecioUnitario;
        totalNuevos += subtotalItem;

        comanda.Detalles.Add(new ComandaDetalle
        {
          PlatilloId = item.PlatilloId,
          Cantidad = item.Cantidad,
          PrecioUnitario = item.PrecioUnitario,
          Subtotal = subtotalItem,
          Notas = item.Notas,
          NumeroPlato = item.NumeroPlato // El plato que eligió el mesero (1, 2, 3...)
        });
      }

      comanda.Subtotal += totalNuevos;
      comanda.Total += totalNuevos;

      // MAGIA KDS: Regresamos la comanda a "Cocinando" (1) y le subimos la prioridad a 1.
      comanda.Estatus = 1;
      comanda.Prioridad = 1;

      if (!await _coRepo.ActualizarComandaAsync(comanda))
        return StatusCode(500, "Error al agregar los platillos.");

      return Ok(new { mensaje = "Platillos agregados, comanda enviada a cocina con prioridad." });
    }

    // ==========================================
    // PATCH: Pagar Comanda (Caja Registradora)
    // ==========================================
    [HttpPatch("pagar/{comandaId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PagarComanda(int comandaId, [FromBody] int metodoPagoId)
    {
      if (!await _coRepo.ExisteComandaAsync(comandaId))
        return NotFound();

      var comanda = await _coRepo.GetComandaAsync(comandaId);

      if (comanda == null)
        return NotFound();

      // Asignamos el método de pago y pasamos el estatus a 3 (Pagado)
      comanda.MetodoPagoId = metodoPagoId;
      comanda.Estatus = 3;

      // Guardamos la comanda actualizada
      if (!await _coRepo.ActualizarComandaAsync(comanda))
      {
        ModelState.AddModelError("", "Error al procesar el pago de la comanda en la base de datos.");
        return StatusCode(500, ModelState);
      }

      return Ok(new { mensaje = "Cuenta cobrada exitosamente." });
    }

    // ==========================================
    // METODO PRIVADO: Mapeo manual a DTO
    // ==========================================
    private ComandaDto MapearComandaDto(Comanda comanda)
    {
      return new ComandaDto
      {
        Id = comanda.Id,
        TipoPedido = ObtenerNombreTipoPedido(comanda.TipoPedido),
        NumeroMesa = comanda.NumeroMesa,
        PlataformaNombre = comanda.Plataforma?.Nombre,
        DireccionEntrega = comanda.Cliente?.Direccion, // Asumiendo que Cliente tiene Dirección
        HoraEntrega = comanda.FechaHoraAgendada,
        Subtotal = comanda.Subtotal,
        Total = comanda.Total,
        FechaRegistro = comanda.FechaRegistro,
        Estado = ObtenerNombreEstatus(comanda.Estatus),
        Prioridad = comanda.Prioridad,
        Detalles = comanda.Detalles?.Select(d => new ComandaDetalleDto
        {
          Id = d.Id,
          PlatilloId = d.PlatilloId,
          PlatilloNombre = d.Platillo?.Nombre, // Esto funciona gracias al .ThenInclude()
          Cantidad = d.Cantidad,
          PrecioUnitario = d.PrecioUnitario,
          Subtotal = d.Subtotal,
          Notas = d.Notas,
          NumeroPlato = d.NumeroPlato // <--- INYECTADO AL RESPONDER A ANGULAR
        }).ToList() ?? new List<ComandaDetalleDto>()
      };
    }

    private string ObtenerNombreTipoPedido(int tipo) => tipo switch
    {
      1 => "Local",
      2 => "Llevar",
      3 => "Domicilio",
      4 => "Agendado",
      5 => "Plataforma",
      _ => "Desconocido"
    };

    private string ObtenerNombreEstatus(int estatus) => estatus switch
    {
      0 => "Agendado",
      1 => "Cocinando",
      2 => "Entregado",
      3 => "Pagado",
      _ => "Desconocido"
    };
  }
}
