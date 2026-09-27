using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class PermisoRepositorio : IPermisoRepositorio
  {
    private readonly DataContext _context;
    public PermisoRepositorio(DataContext context)
    {
        _context = context;
    }

    // Para consultas
    public async Task<ICollection<Permiso>> GetPermisosAsync()
    {
      return await _context.Permisos.OrderBy(p => p.Nombre).ToListAsync();
    }
    public async Task<Permiso> GetPermisoAsync(int id)
    {
      return await _context.Permisos.FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task<Permiso> GetPermisoXNombreAsync(string nombre)
    {
      return await _context.Permisos.FirstOrDefaultAsync(p => p.Nombre.Trim() == nombre.Trim());
    }

    // Para Validaciones
    public async Task<bool> ExistePermisoAsync(int id)
    {
      return await _context.Permisos.AnyAsync(p => p.Id == id);
    }
    public async Task<bool> ExistePermisoXNombreAsync(string nombre)
    {
      return await _context.Permisos.AnyAsync(p => p.Nombre.Trim() == nombre.Trim());
    }

    // Para el CRUD
    public async Task<bool> CrearPermisoAsync(Permiso permiso)
    {
      _context.Permisos.Add(permiso);
      return await GuardarAsync();
    }
    public async Task<bool> ActualizarPermisoAsync(Permiso permiso)
    {
      var permisoExistente = await _context.Permisos.FindAsync(permiso.Id);

      if (permisoExistente != null)
      {
        _context.Entry(permisoExistente).CurrentValues.SetValues(permiso);
        return await GuardarAsync();
      }

      return false;
    }

    public async Task<bool> BorrarPermisoAsync(Permiso permiso)
    {
      _context.Permisos.Remove(permiso);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
