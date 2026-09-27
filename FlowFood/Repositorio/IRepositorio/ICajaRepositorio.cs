using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface ICajaRepositorio
  {
    // Gestión de Turnos
    Task<CajaTurno> ObtenerTurnoAbiertoAsync(int usuarioCajeroId);
    Task<CajaTurno> ObtenerTurnoPorIdAsync(int turnoId);
    Task<bool> AbrirTurnoAsync(CajaTurno turno);
    Task<bool> CerrarTurnoAsync(CajaTurno turno);

    // Gestión de Movimientos (Ventas, Entradas, Salidas)
    Task<bool> RegistrarMovimientoAsync(MovimientoCaja movimiento);
    Task<IEnumerable<MovimientoCaja>> ObtenerMovimientosTurnoAsync(int turnoId);

    // Validar credenciales del supervisor (Para los pagos a proveedores y corte de caja)
    Task<Usuario> AutenticarSupervisorAsync(string usuario, string password);

    Task<bool> GuardarAsync();
  }
}
