using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class InventarioRepositorio : IInventarioRepositorio
  {
    private readonly DataContext _context;
    public InventarioRepositorio(DataContext context)
    {
      _context = context;
    }

    // Consultas
    public async Task<ICollection<Inventario>> GetInventariosAsync()
    {
      return await _context.Inventarios
          .Include(i => i.Almacen)
          .Include(i => i.Producto)
          .ThenInclude(p => p.UnidadMedida)
          .OrderBy(i => i.Producto.Nombre)
          .ToListAsync();
    }

    public async Task<Inventario> GetInventarioAsync(int id)
    {
      return await _context.Inventarios
          .Include(i => i.Almacen)
          .Include(i => i.Producto)
          .ThenInclude(p => p.UnidadMedida)
          .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<ICollection<Inventario>> GetInventariosPorAlmacenAsync(int almacenId)
    {
      return await _context.Inventarios
          .Include(i => i.Producto)
          .ThenInclude(p => p.UnidadMedida)
          .Where(i => i.almacenId == almacenId)
          .ToListAsync();
    }

    public async Task<ICollection<Inventario>> GetInventariosPorProductoAsync(int productoId)
    {
      return await _context.Inventarios
          .Include(i => i.Almacen)
          .Include(i => i.Producto)
          .ThenInclude(p => p.UnidadMedida)
          .Where(i => i.productoId == productoId)
          .ToListAsync();
    }

    public async Task<Inventario> GetInventarioXProductoYAlmacenAsync(int productoId, int almacenId)
    {
      return await _context.Inventarios
          .Include(i => i.Almacen)
          .Include(i => i.Producto)
          .ThenInclude(p => p.UnidadMedida)
          .FirstOrDefaultAsync(i => i.productoId == productoId && i.almacenId == almacenId);
    }

    // Para validaciones
    public async Task<bool> ExisteInventarioAsync(int id)
    {
      return await _context.Inventarios.AnyAsync(i => i.Id == id);
    }

    public async Task<bool> ExisteInventarioXProductoYAlmacenAsync(int productoId, int almacenId)
    {
      return await _context.Inventarios.AnyAsync(i => i.productoId == productoId && i.almacenId == almacenId);
    }

    // Cantidad total sumando todos los almacenes (usada por ProductoController)
    public async Task<decimal> GetCantidadTotalPorProductoAsync(int productoId)
    {
      return await _context.Inventarios
          .Where(i => i.productoId == productoId)
          .SumAsync(i => (decimal?)i.Cantidad) ?? 0;
    }

    // Para el CRUD
    public async Task<bool> CrearInventarioAsync(Inventario inventario)
    {
      _context.Inventarios.Add(inventario);
      return await GuardarAsync();
    }

    public async Task<bool> ActualizarInventarioAsync(Inventario inventario)
    {
      var inventarioExistente = await _context.Inventarios.AsNoTracking().FirstOrDefaultAsync(i => i.Id == inventario.Id);
      if (inventarioExistente != null)
        _context.Entry(inventarioExistente).CurrentValues.SetValues(inventario);
      else
        _context.Inventarios.Update(inventario);
      return await GuardarAsync();
    }

    // Usado por BajaController al registrar una baja
    public async Task<bool> DescontarCantidadAsync(int inventarioId, decimal cantidad)
    {
      var inventario = await _context.Inventarios.FirstOrDefaultAsync(i => i.Id == inventarioId);
      if (inventario == null || inventario.Cantidad < cantidad)
        return false;

      inventario.Cantidad -= cantidad;
      return await GuardarAsync();
    }

    public async Task<bool> BorrarInventarioAsync(Inventario inventario)
    {
      _context.Inventarios.Remove(inventario);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
