using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IPuestoRepositorio
  {
    // Para obtener varios registros o solo uno
    Task<ICollection<Puesto>> GetPuestosAsync();
    Task<Puesto> GetPuestoAsync(int puestoId);

    // Para validacion
    Task<bool> ExistePuestoAsync(int id);
    Task<bool> ExistePuestoXNombreAsync(string nombre);

    // Para el CRUD
    Task<bool> CrearPuestoAsync(Puesto puesto);
    Task<bool> ActualizarPuestoAsync(Puesto puesto);
    Task<bool> BorrarPuestoAsync(Puesto puesto);
    Task<bool> GuardarAsync();
  }
}
