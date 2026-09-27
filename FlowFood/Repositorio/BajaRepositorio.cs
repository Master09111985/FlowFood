using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class BajaRepositorio : IBajaRepositorio
  {
    private readonly DataContext _context;

    public BajaRepositorio(DataContext context)
    {
      _context = context;
    }

    public async Task<ICollection<Baja>> GetBajasAsync()
    {
      return await _context.Bajas
          .Include(b => b.MotivoBaja)
          .Include(b => b.Inventario)
            .ThenInclude(i => i.Producto)
                .ThenInclude(p => p.UnidadMedida)
          .Include(b => b.Inventario)
            .ThenInclude(i => i.Almacen)
          .OrderByDescending(b => b.FechaBaja) // Las bajas más recientes primero
          .ToListAsync();
    }

    public async Task<Baja> GetBajaAsync(int id)
    {
      return await _context.Bajas
          .Include(b => b.MotivoBaja)
          .Include(b => b.Inventario)
            .ThenInclude(i => i.Producto)
                .ThenInclude(p => p.UnidadMedida)
          .Include(b => b.Inventario)
            .ThenInclude(i => i.Almacen)
          .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<ICollection<Baja>> GetBajasPorProductoAsync(int productoId)
    {
      return await _context.Bajas
          .Include(b => b.MotivoBaja)
          .Include(b => b.Inventario)
            .ThenInclude(i => i.Producto)
                .ThenInclude(p => p.UnidadMedida)
          .Include(b => b.Inventario)
            .ThenInclude(i => i.Almacen)
          .Where(b => b.Inventario.productoId == productoId)
          .OrderByDescending(b => b.FechaBaja)
          .ToListAsync();
    }

    public async Task<ICollection<Baja>> GetBajasPorAlmacenAsync(int almacenId)
    {
      return await _context.Bajas
          .Include(b => b.MotivoBaja)
          .Include(b => b.Inventario)
            .ThenInclude(i => i.Producto)
                .ThenInclude(p => p.UnidadMedida)
          .Include(b => b.Inventario)
            .ThenInclude(i => i.Almacen)
          .Where(b => b.Inventario.almacenId == almacenId)
          .OrderByDescending(b => b.FechaBaja)
          .ToListAsync();
    }

    public async Task<bool> ExisteBajaAsync(int id)
    {
      return await _context.Bajas.AnyAsync(b => b.Id == id);
    }

    public async Task<bool> CrearBajaAsync(Baja baja)
    {
      _context.Bajas.Add(baja);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
