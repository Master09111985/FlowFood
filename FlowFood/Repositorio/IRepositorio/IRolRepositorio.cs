using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IRolRepositorio
  {
    Task<ICollection<Rol>> GetRolesAsync();
    Task<Rol> GetRolAsync(int id);
    Task<Rol> GetRolXNombreAsync(string nombre);
    Task<bool> ExisteRolAsync(int id);
    Task<bool> ExisteRolXNombreAsync(string nombre);
    Task<bool> CrearRolAsync(Rol rol);
    Task<bool> ActualizarRolAsync(Rol rol);
    Task<bool> BorrarRolAsync(Rol rol);
    Task<bool> GuardarAsync();
  }
}
