using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class UsuarioRepositorio : IUsuarioRepositorio
  {
    private readonly DataContext _context;
    public UsuarioRepositorio(DataContext context)
    {
      _context = context;
    }

    // Consultas
    public async Task<ICollection<Usuario>> GetUsuariosAsync()
    {
      return await _context.Usuarios
          .Include(u => u.Rol)
          .OrderBy(u => u.Nombre)
          .ToListAsync();
    }

    public async Task<Usuario> GetUsuarioAsync(int id)
    {
      return await _context.Usuarios
          .Include(u => u.Rol)
          .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario> GetUsuarioXNombreAsync(string nombre)
    {
      return await _context.Usuarios
          .Include(u => u.Rol)
          .FirstOrDefaultAsync(u => u.Nombre.Trim() == nombre.Trim());
    }

    // Para validaciones
    public async Task<bool> ExisteUsuarioAsync(int id)
    {
      return await _context.Usuarios.AnyAsync(u => u.Id == id);
    }

    public async Task<bool> ExisteUsuarioXNombreAsync(string nombre)
    {
      return await _context.Usuarios.AnyAsync(u => u.Nombre.Trim() == nombre.Trim());
    }

    public async Task<bool> ExisteUsuarioXNombreEmpleadoAsync(int id)
    {
      return await _context.Usuarios.AnyAsync(u => u.empleadoId == id);
    }

    // Para el CRUD
    public async Task<bool> CrearUsuarioAsync(Usuario usuario)
    {
      _context.Usuarios.Add(usuario);
      return await GuardarAsync();
    }

    public async Task<bool> ActualizarUsuarioAsync(Usuario usuario)
    {
      var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuario.Id);
      if (usuarioExistente != null)
        _context.Entry(usuarioExistente).CurrentValues.SetValues(usuario);
      else
        _context.Usuarios.Update(usuario);
      return await GuardarAsync();
    }

    public async Task<bool> BorrarUsuarioAsync(Usuario usuario)
    {
      _context.Usuarios.Remove(usuario);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
