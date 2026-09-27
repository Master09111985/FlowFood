using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class EmpleadoRepositorio : IEmpleadoRepositorio
  {
    private readonly DataContext _context;
    public EmpleadoRepositorio(DataContext context)
    {
      _context = context;
    }

    // Consultas
    public async Task<ICollection<Empleado>> GetEmpleadosAsync()
    {
      return await _context.Empleados
          .Include(e => e.Puesto)
          .OrderBy(e => e.Nombre)
          .ToListAsync();
    }

    public async Task<Empleado> GetEmpleadoAsync(int id)
    {
      return await _context.Empleados
          .Include(e => e.Puesto)
          .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Empleado> GetEmpleadoXCodigoAsync(string codigo)
    {
      return await _context.Empleados
          .Include(e => e.Puesto)
          .FirstOrDefaultAsync(e => e.Codigo.Trim() == codigo.Trim());
    }

    // Para validaciones
    public async Task<bool> ExisteEmpleadoAsync(int id)
    {
      return await _context.Empleados.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> ExisteEmpleadoXCodigoAsync(string codigo)
    {
      return await _context.Empleados.AnyAsync(e => e.Codigo.Trim() == codigo.Trim());
    }

    // Generacion del codigo consecutivo EMP-00001, EMP-00002, ...
    public async Task<string> GenerarSiguienteCodigoAsync()
    {
      var ultimoEmpleado = await _context.Empleados
          .OrderByDescending(e => e.Id)
          .FirstOrDefaultAsync();

      if (ultimoEmpleado == null)
        return "EMP-00001";

      // Tomamos la parte numerica despues del guion y la incrementamos
      var partesCodigo = ultimoEmpleado.Codigo.Split('-');
      var numeroActual = int.Parse(partesCodigo[1]);
      var siguienteNumero = numeroActual + 1;

      return $"EMP-{siguienteNumero:D5}";
    }

    // Para el CRUD
    public async Task<bool> CrearEmpleadoAsync(Empleado empleado)
    {
      _context.Empleados.Add(empleado);
      return await GuardarAsync();
    }

    public async Task<bool> ActualizarEmpleadoAsync(Empleado empleado)
    {
      var empleadoExistente = await _context.Empleados.AsNoTracking().FirstOrDefaultAsync(e => e.Id == empleado.Id);
      if (empleadoExistente != null)
        _context.Entry(empleadoExistente).CurrentValues.SetValues(empleado);
      else
        _context.Empleados.Update(empleado);
      return await GuardarAsync();
    }

    public async Task<bool> BorrarEmpleadoAsync(Empleado empleado)
    {
      _context.Empleados.Remove(empleado);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
