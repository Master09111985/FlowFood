using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FlowFood.Controllers
{
    [Route("flowfood/[controller]")]
    [ApiController]
    public class CajaController : ControllerBase
    {
        private readonly ICajaRepositorio _cajaRepo;
        private readonly IComandaRepositorio _comandaRepo;

        public CajaController(ICajaRepositorio cajaRepo, IComandaRepositorio comandaRepo)
        {
            _cajaRepo = cajaRepo;
            _comandaRepo = comandaRepo;
        }

        // ==========================================
        // 1. VERIFICAR TURNO Y ABRIR CAJA (FONDO)
        // ==========================================
        [HttpGet("turno-abierto/{cajeroId:int}")]
        public async Task<IActionResult> GetTurnoAbierto(int cajeroId)
        {
            var turno = await _cajaRepo.ObtenerTurnoAbiertoAsync(cajeroId);
            if (turno == null) return NotFound(new { mensaje = "No hay turno abierto. Requiere fondo inicial." });

            return Ok(turno);
        }

        [HttpPost("abrir")]
        public async Task<IActionResult> AbrirTurno([FromBody] AbrirTurnoDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var turnoExistente = await _cajaRepo.ObtenerTurnoAbiertoAsync(dto.UsuarioCajeroId);
            if (turnoExistente != null)
                return BadRequest("El usuario ya tiene un turno abierto.");

            var nuevoTurno = new CajaTurno
            {
                UsuarioCajeroId = dto.UsuarioCajeroId,
                FondoInicial = dto.FondoInicial,
                FechaApertura = DateTime.UtcNow,
                EstaAbierta = true
            };

            if (!await _cajaRepo.AbrirTurnoAsync(nuevoTurno))
                return StatusCode(500, "Error al abrir la caja.");

            return Ok(nuevoTurno);
        }

        // ==========================================
        // 2. COBRAR COMANDA Y REGISTRAR MOVIMIENTO (SIMPLE O MIXTO)
        // ==========================================
        [HttpPost("cobrar")]
        public async Task<IActionResult> CobrarComanda([FromBody] CobrarComandaDto dto)
        {
            // Verificamos si la comanda existe
            var comanda = await _comandaRepo.GetComandaAsync(dto.ComandaId);
            if (comanda == null) return NotFound("Comanda no encontrada.");

            // Verificamos si el cajero tiene caja abierta
            var turno = await _cajaRepo.ObtenerTurnoAbiertoAsync(dto.UsuarioCajeroId);
            if (turno == null) return BadRequest("No tienes una caja abierta para cobrar.");

            // Si viene una lista de pagos (Cobro Mixto), validamos que sume el total y registramos cada parte
            if (dto.Pagos != null && dto.Pagos.Count > 0)
            {
                var sumaPagos = dto.Pagos.Sum(p => p.Monto);
                if (Math.Abs(sumaPagos - comanda.Total) > 0.01m)
                    return BadRequest("La suma de los pagos mixtos no coincide con el total de la comanda.");

                foreach (var pago in dto.Pagos.Where(p => p.Monto > 0))
                {
                    var mov = new MovimientoCaja
                    {
                        CajaTurnoId = turno.Id,
                        TipoMovimiento = 1, // 1 = Entrada (Venta)
                        Monto = pago.Monto,
                        Fecha = DateTime.UtcNow,
                        MetodoPagoId = pago.MetodoPagoId,
                        ComandaId = comanda.Id
                    };
                    await _cajaRepo.RegistrarMovimientoAsync(mov);
                }

                // En la comanda guardamos el método con el mayor monto como referencia principal
                comanda.MetodoPagoId = dto.Pagos.OrderByDescending(p => p.Monto).First().MetodoPagoId;
            }
            else
            {
                // Cobro Simple (1 solo método por el total completo)
                var movimiento = new MovimientoCaja
                {
                    CajaTurnoId = turno.Id,
                    TipoMovimiento = 1, // 1 = Entrada (Venta)
                    Monto = comanda.Total,
                    Fecha = DateTime.UtcNow,
                    MetodoPagoId = dto.MetodoPagoId,
                    ComandaId = comanda.Id
                };

                await _cajaRepo.RegistrarMovimientoAsync(movimiento);
                comanda.MetodoPagoId = dto.MetodoPagoId;
            }

            // Actualizamos la comanda a Pagado
            comanda.Estatus = 3; // Pagado
            await _comandaRepo.ActualizarComandaAsync(comanda);

            return Ok(new { mensaje = "Cobro procesado y registrado en caja exitosamente." });
        }

        // ==========================================
        // 3. PAGO A PROVEEDOR (SALIDA CON AUTORIZACIÓN)
        // ==========================================
        [HttpPost("pago-proveedor")]
        public async Task<IActionResult> PagoProveedor([FromBody] PagoProveedorCajaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var turno = await _cajaRepo.ObtenerTurnoPorIdAsync(dto.TurnoId);
            if (turno == null || !turno.EstaAbierta)
                return BadRequest("El turno de caja no es válido o ya está cerrado.");

            // Validar credenciales de supervisor
            var supervisor = await _cajaRepo.AutenticarSupervisorAsync(dto.SupervisorUsuario, dto.SupervisorPassword);
            if (supervisor == null)
                return Unauthorized("Credenciales de supervisor inválidas o no tiene permisos.");

            var movimiento = new MovimientoCaja
            {
                CajaTurnoId = turno.Id,
                TipoMovimiento = 2, // 2 = Salida (Pago Proveedor)
                Monto = dto.Monto,
                Fecha = DateTime.UtcNow,
                MetodoPagoId = 1, // Asumimos Efectivo
                ProveedorId = dto.ProveedorId,
                UsuarioAutorizaId = supervisor.Id
            };

            if (!await _cajaRepo.RegistrarMovimientoAsync(movimiento))
                return StatusCode(500, "Error al registrar la salida de caja.");

            return Ok(new { mensaje = "Pago a proveedor autorizado y registrado." });
        }

        // ==========================================
        // 4. CORTE DE CAJA Y GENERACIÓN DE TICKET
        // ==========================================
        [HttpPost("cerrar")]
        public async Task<IActionResult> CerrarTurno([FromBody] CerrarTurnoDto dto)
        {
            var turno = await _cajaRepo.ObtenerTurnoPorIdAsync(dto.TurnoId);
            if (turno == null || !turno.EstaAbierta)
                return BadRequest("El turno no existe o ya fue cerrado.");

            // CANDADO: Verificar que no existan comandas en Cocina (1) o Entregadas sin cobrar (2)
            var comandasActivas = await _comandaRepo.GetComandasAsync();
            if (comandasActivas.Any(c => c.Estatus == 1 || c.Estatus == 2))
                return BadRequest("No se puede realizar el corte: aún hay comandas en preparación en cocina o pendientes por cobrar.");

            // Validar credenciales
            var supervisor = await _cajaRepo.AutenticarSupervisorAsync(dto.SupervisorUsuario, dto.SupervisorPassword);
            if (supervisor == null)
                return Unauthorized("Credenciales de supervisor inválidas.");

            // Obtener todos los movimientos del turno y el catálogo de métodos de pago desde CajaRepo
            var movimientos = await _cajaRepo.ObtenerMovimientosTurnoAsync(turno.Id);
            var metodosPago = await _cajaRepo.ObtenerMetodosPagoAsync();

            int idEfectivo = metodosPago.FirstOrDefault(mp => mp.Nombre.Contains("efectivo", StringComparison.OrdinalIgnoreCase))?.Id ?? 1;
            int idTarjeta = metodosPago.FirstOrDefault(mp => mp.Nombre.Contains("tarjeta", StringComparison.OrdinalIgnoreCase))?.Id ?? 2;
            int idTransferencia = metodosPago.FirstOrDefault(mp => mp.Nombre.Contains("transferencia", StringComparison.OrdinalIgnoreCase))?.Id ?? 3;

            var entradas = movimientos.Where(m => m.TipoMovimiento == 1).ToList();

            // Cálculos desglosados por Método de Pago usando MetodoPagoId
            decimal ventasEfectivo = entradas.Where(m => m.MetodoPagoId == idEfectivo).Sum(m => m.Monto);
            decimal ventasTarjeta = entradas.Where(m => m.MetodoPagoId == idTarjeta).Sum(m => m.Monto);
            decimal ventasTransferencia = entradas.Where(m => m.MetodoPagoId == idTransferencia).Sum(m => m.Monto);

            decimal totalVentasGeneral = ventasEfectivo + ventasTarjeta + ventasTransferencia;
            decimal salidasProveedores = movimientos.Where(m => m.TipoMovimiento == 2).Sum(m => m.Monto);

            // Fórmula del cajón físico: Fondo + Entradas Efectivo - Salidas Efectivo
            decimal efectivoCalculado = turno.FondoInicial + ventasEfectivo - salidasProveedores;

            turno.FechaCierre = DateTime.UtcNow;
            turno.UsuarioSupervisorId = supervisor.Id;
            turno.EfectivoCalculado = efectivoCalculado;
            turno.EfectivoReportado = dto.EfectivoReportado;
            turno.Diferencia = dto.EfectivoReportado - efectivoCalculado;
            turno.EstaAbierta = false;

            if (!await _cajaRepo.CerrarTurnoAsync(turno))
                return StatusCode(500, "Error al procesar el corte de caja.");

            // Preparamos la respuesta para el Ticket de Angular
            var ticketRespuesta = new TicketCorteDto
            {
                TurnoId = turno.Id,
                NombreSupervisor = supervisor.Nombre,
                FechaApertura = turno.FechaApertura,
                FechaCierre = turno.FechaCierre.Value,
                FondoInicial = turno.FondoInicial,
                TotalVentasEfectivo = ventasEfectivo,
                TotalVentasTarjeta = ventasTarjeta,
                TotalVentasTransferencia = ventasTransferencia,
                TotalVentasGeneral = totalVentasGeneral,
                TotalPagosProveedores = salidasProveedores,
                EfectivoCalculadoSistema = efectivoCalculado,
                EfectivoFisicoReportado = dto.EfectivoReportado,
                Diferencia = turno.Diferencia
            };

            return Ok(ticketRespuesta);
        }
    }

    // DTOs auxiliares para el cobro simple y mixto
    public class DetallePagoMixtoDto
    {
        public int MetodoPagoId { get; set; }
        public decimal Monto { get; set; }
    }

    public class CobrarComandaDto
    {
        [Required] public int ComandaId { get; set; }
        public int MetodoPagoId { get; set; }
        [Required] public int UsuarioCajeroId { get; set; } // Quien cobra
        public List<DetallePagoMixtoDto>? Pagos { get; set; } // Opcional para cobro mixto
    }
}