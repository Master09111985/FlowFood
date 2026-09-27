using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class ProveedorRepositorio : IProveedorRepositorio
  {
    private readonly DataContext _context;
    public ProveedorRepositorio(DataContext context)
    {
        _context = context;
    }

    // Esto es para consultas
    public async Task<ICollection<Proveedor>> GetProveedoresAsync()
    {
      return await _context.Proveedores.OrderBy(p => p.Nombre).ToListAsync();
    }
    public async Task<Proveedor> GetProveedorAsync(int proveedorId)
    {
      return await _context.Proveedores.FirstOrDefaultAsync(p => p.Id == proveedorId);
    }

    // Esto es para validaciones
    public async Task<bool> ExisteProveedorAsync(int id)
    {
      return await _context.Proveedores.AnyAsync(p => p.Id == id);
    }
    public async Task<bool> ExistenteProveedorXNombreAsync(string nombre)
    {
      bool valor = await _context.Proveedores.AnyAsync(p => p.Nombre.ToLower().Trim() == nombre);
      return valor;
    }

    // Esto es para el CRUD
    public async Task<bool> CrearProveedorAsync(Proveedor proveedor)
    {
      _context.Proveedores.Add(proveedor);
      return await GuardarAsync();
    }
    public async Task<bool> ActualizarProveedorAsync(Proveedor proveedor)
    {
      _context.Proveedores.Update(proveedor);
      return await GuardarAsync();
    }
    public async Task<bool> BorrarProveedorAsync(Proveedor proveedor)
    {
      _context.Proveedores.Remove(proveedor);
      return await GuardarAsync();
    }
    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() > 0 ? true : false;
    }
  }
}
