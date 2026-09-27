using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class ProductoRepositorio : IProductoRepositorio
  {
    private readonly DataContext _context;

    public ProductoRepositorio(DataContext context)
    {
      _context = context;
    }

    // Consultas
    public async Task<ICollection<Producto>> GetProductosAsync()
    {
      return await _context.Productos
          .Include(p => p.UnidadMedida)
          .OrderBy(p => p.Nombre)
          .ToListAsync();
    }

    public async Task<Producto> GetProductoAsync(int id)
    {
      return await _context.Productos
          .Include(p => p.UnidadMedida)
          .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Producto> GetProductoXNombreAsync(string nombre)
    {
      return await _context.Productos
          .Include(p => p.UnidadMedida)
          .FirstOrDefaultAsync(p => p.Nombre.Trim() == nombre.Trim());
    }

    // Para validaciones
    public async Task<bool> ExisteProductoAsync(int id)
    {
      return await _context.Productos.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> ExisteProductoXNombreAsync(string nombre)
    {
      return await _context.Productos.AnyAsync(p => p.Nombre.Trim() == nombre.Trim());
    }

    // Cantidad total sumando todos los almacenes (calculada, no almacenada)
    public async Task<decimal> GetCantidadTotalAsync(int productoId)
    {
      return await _context.Inventarios
          .Where(i => i.productoId == productoId)
          .SumAsync(i => (decimal?)i.Cantidad) ?? 0;
    }

    // Para el CRUD
    public async Task<bool> CrearProductoAsync(Producto producto)
    {
      _context.Productos.Add(producto);
      return await GuardarAsync();
    }

    public async Task<bool> ActualizarProductoAsync(Producto producto)
    {
      // Para actualizar usamos el FindAsync
      var productoExistente = await _context.Productos.FindAsync(producto.Id);
      if (productoExistente != null)
      {
        _context.Entry(productoExistente).CurrentValues.SetValues(producto);
        return await GuardarAsync();
      }
      return false;
    }

    public async Task<bool> BorrarProductoAsync(Producto producto)
    {
      _context.Productos.Remove(producto);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
