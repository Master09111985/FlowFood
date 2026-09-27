using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IRolPermisoRepositorio
  {
    Task<ICollection<RolPermiso>> GetRolPermisosAsync();
    Task<RolPermiso> GetRolPermisoAsync(int id);
    Task<ICollection<RolPermiso>> GetRolPermisosPorRolAsync(int rolId);
    Task<RolPermiso> GetRolPermisoAsync(int rolId, int permisoId);
    Task<bool> ExisteRolPermisoAsync(int id);
    Task<bool> ExisteRolPermisoAsync(int rolId, int permisoId);
    Task<bool> CrearRolPermisoAsync(RolPermiso rolPermiso);
    Task<bool> CrearRolPermisosAsync(ICollection<RolPermiso> rolPermisos); // insercion masiva
    Task<bool> ActualizarRolPermisoAsync(RolPermiso rolPermiso);
    Task<bool> BorrarRolPermisoAsync(RolPermiso rolPermiso);
    Task<bool> GuardarAsync();
  }
}
