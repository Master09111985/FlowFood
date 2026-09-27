using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IEmpleadoRepositorio
  {
    Task<ICollection<Empleado>> GetEmpleadosAsync();
    Task<Empleado> GetEmpleadoAsync(int id);
    Task<Empleado> GetEmpleadoXCodigoAsync(string codigo);
    Task<bool> ExisteEmpleadoAsync(int id);
    Task<bool> ExisteEmpleadoXCodigoAsync(string codigo);
    Task<string> GenerarSiguienteCodigoAsync();
    Task<bool> CrearEmpleadoAsync(Empleado empleado);
    Task<bool> ActualizarEmpleadoAsync(Empleado empleado);
    Task<bool> BorrarEmpleadoAsync(Empleado empleado);
    Task<bool> GuardarAsync();
  }
}
