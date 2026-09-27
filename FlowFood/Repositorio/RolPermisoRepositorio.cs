using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class RolPermisoRepositorio : IRolPermisoRepositorio
  {
    private readonly DataContext _context;
    public RolPermisoRepositorio(DataContext context)
    {
        _context = context;
    }

    public async Task<ICollection<RolPermiso>> GetRolPermisosAsync()
    {
      return await _context.RolPermisos
                .Include(rp => rp.Rol)
                .Include(rp => rp.Permiso)
                .ToListAsync();
    }
    public async Task<RolPermiso> GetRolPermisoAsync(int id)
    {
      return await _context.RolPermisos
                .Include(rp => rp.Rol)
                .Include(rp => rp.Permiso)
                .FirstOrDefaultAsync(rp => rp.Id == id);
    }
    public async Task<ICollection<RolPermiso>> GetRolPermisosPorRolAsync(int rolId)
    {
      return await _context.RolPermisos
                .Include(rp => rp.Permiso)
                .Where(rp => rp.rolId == rolId)
                .ToListAsync();
    }
    public async Task<RolPermiso> GetRolPermisoAsync(int rolId, int permisoId)
    {
      return await _context.RolPermisos
                .FirstOrDefaultAsync(rp => rp.rolId == rolId && rp.permisoId == permisoId);
    }

    // Para Validaciones
    public async Task<bool> ExisteRolPermisoAsync(int id)
    {
      return await _context.RolPermisos.AnyAsync(rp => rp.Id == id);
    }
    public async Task<bool> ExisteRolPermisoAsync(int rolId, int permisoId)
    {
      return await _context.RolPermisos.AnyAsync(rp => rp.rolId == rolId && rp.permisoId == permisoId);
    }

    // Para el CRUD
    public async Task<bool> CrearRolPermisoAsync(RolPermiso rolPermiso)
    {
      _context.RolPermisos.Add(rolPermiso);
      return await GuardarAsync();
    }
    public async Task<bool> CrearRolPermisosAsync(ICollection<RolPermiso> rolPermisos)
    {
      _context.RolPermisos.AddRange(rolPermisos);
      return await GuardarAsync();
    }
    public async Task<bool> ActualizarRolPermisoAsync(RolPermiso rolPermiso)
    {
      var rolPermisoExistente = await _context.RolPermisos.AsNoTracking().FirstOrDefaultAsync(rp => rp.Id == rolPermiso.Id);
      if (rolPermisoExistente != null)
        _context.Entry(rolPermisoExistente).CurrentValues.SetValues(rolPermiso);
      else
        _context.RolPermisos.Update(rolPermiso);
      return await GuardarAsync();
    }
    public async Task<bool> BorrarRolPermisoAsync(RolPermiso rolPermiso)
    {
      _context.RolPermisos.Remove(rolPermiso);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
