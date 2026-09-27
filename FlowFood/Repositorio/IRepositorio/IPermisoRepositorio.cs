using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IPermisoRepositorio
  {
    Task<ICollection<Permiso>> GetPermisosAsync();
    Task<Permiso> GetPermisoAsync(int id);
    Task<Permiso> GetPermisoXNombreAsync(string nombre);
    Task<bool> ExistePermisoAsync(int id);
    Task<bool> ExistePermisoXNombreAsync(string nombre);
    Task<bool> CrearPermisoAsync(Permiso permiso);
    Task<bool> ActualizarPermisoAsync(Permiso permiso);
    Task<bool> BorrarPermisoAsync(Permiso permiso);
    Task<bool> GuardarAsync();
  }
}
