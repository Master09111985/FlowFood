using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class RolRepositorio : IRolRepositorio
  {
    private readonly DataContext _context;
    public RolRepositorio(DataContext context)
    {
      _context = context;
    }

    public async Task<ICollection<Rol>> GetRolesAsync()
    {
      return await _context.Roles.OrderBy(r => r.Nombre).ToListAsync();
    }
    public async Task<Rol> GetRolAsync(int id)
    {
      return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<Rol> GetRolXNombreAsync(string nombre)
    {
      return await _context.Roles.FirstOrDefaultAsync(r => r.Nombre.Trim() == nombre.Trim());
    }

    // Para validaciones
    public async Task<bool> ExisteRolAsync(int id)
    {
      return await _context.Roles.AnyAsync(r => r.Id == id);
    }
    public async Task<bool> ExisteRolXNombreAsync(string nombre)
    {
      return await _context.Roles.AnyAsync(r => r.Nombre.Trim() == nombre.Trim());
    }

    // Para el CRUD
    public async Task<bool> CrearRolAsync(Rol rol)
    {
      _context.Roles.Add(rol);
      return await GuardarAsync();
    }
    public async Task<bool> ActualizarRolAsync(Rol rol)
    {
      var rolExistente = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == rol.Id);
      if (rolExistente != null)
        _context.Entry(rolExistente).CurrentValues.SetValues(rol);
      else
        _context.Roles.Update(rol);
      return await GuardarAsync();
    }

    public async Task<bool> BorrarRolAsync(Rol rol)
    {
      _context.Roles.Remove(rol);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
