using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class CajaRepositorio : ICajaRepositorio
  {
    private readonly DataContext _context;

    public CajaRepositorio(DataContext context)
    {
      _context = context;
    }

    public async Task<CajaTurno> ObtenerTurnoAbiertoAsync(int usuarioCajeroId)
    {
      return await _context.CajaTurnos
          .FirstOrDefaultAsync(c => c.UsuarioCajeroId == usuarioCajeroId && c.EstaAbierta);
    }

    public async Task<CajaTurno> ObtenerTurnoPorIdAsync(int turnoId)
    {
      return await _context.CajaTurnos.FirstOrDefaultAsync(c => c.Id == turnoId);
    }

    public async Task<bool> AbrirTurnoAsync(CajaTurno turno)
    {
      await _context.CajaTurnos.AddAsync(turno);
      return await GuardarAsync();
    }

    public async Task<bool> CerrarTurnoAsync(CajaTurno turno)
    {
      _context.CajaTurnos.Update(turno);
      return await GuardarAsync();
    }

    public async Task<bool> RegistrarMovimientoAsync(MovimientoCaja movimiento)
    {
      await _context.MovimientosCaja.AddAsync(movimiento);
      return await GuardarAsync();
    }

    public async Task<IEnumerable<MovimientoCaja>> ObtenerMovimientosTurnoAsync(int turnoId)
    {
      return await _context.MovimientosCaja
          .Where(m => m.CajaTurnoId == turnoId)
          .OrderBy(m => m.Fecha)
          .ToListAsync();
    }

    public async Task<Usuario> AutenticarSupervisorAsync(string nombreUsuario, string password)
    {
      // Busca al usuario
      var usuarioDb = await _context.Usuarios
          .Include(u => u.Rol)
          .FirstOrDefaultAsync(u => u.Nombre == nombreUsuario && u.Estado == true);

      if (usuarioDb == null) return null;

      // Aquí deberías usar tu lógica de desencriptación/bcrypt que usas en el Login normal
      // Si la contraseña coincide y su rol es apto (ej. Rol "Administrador" o ID 1), lo devuelves
      // if (!BCrypt.Net.BCrypt.Verify(password, usuarioDb.Password)) return null;

      // Verifica que tenga permisos de supervisor (Ajusta la validación de Rol según tu base de datos)
      if (usuarioDb.Rol.Nombre != "Administrador" && usuarioDb.Rol.Nombre != "Supervisor")
        return null;

      return usuarioDb;
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() > 0;
    }
  }
}
