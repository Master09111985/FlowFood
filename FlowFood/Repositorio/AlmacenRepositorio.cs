using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;

using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class AlmacenRepositorio : IAlmacenRepositorio
  {
    private readonly DataContext _context;
    public AlmacenRepositorio(DataContext context)
    {
      _context = context;
    }

    // Esto es para consultas
    public async Task<ICollection<Almacen>> GetAlmacenesAsync()
    {
      return await _context.Almacenes.OrderBy(a => a.Nombre).ToListAsync();
    }
    public async Task<Almacen> GetAlmacenAsync(int almacenId)
    {
      return await _context.Almacenes.FirstOrDefaultAsync(a => a.Id == almacenId);
    }

    // Esto es para validaciones
    public async Task<bool> ExisteAlmacenAsync(int id)
    {
      return await _context.Almacenes.AnyAsync(a => a.Id == id);
    }
    public async Task<bool> ExistenteAlmacenXNombreAsync(string nombre)
    {
      bool valor = await _context.Almacenes.AnyAsync(a => a.Nombre.ToLower().Trim() == nombre);
      return valor;
    }

    // Esto es para el CRUD
    public async Task<bool> CrearAlmacenAsync(Almacen almacen)
    {
      _context.Almacenes.Add(almacen);
      return await GuardarAsync();
    }
    public async Task<bool> ActualizarAlmacenAsync(Almacen almacen)
    {
      _context.Almacenes.Update(almacen);
      return await GuardarAsync();
    }
    public async Task<bool> BorrarAlmacenAsync(Almacen almacen)
    {
      _context.Almacenes.Remove(almacen);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() > 0 ? true : false;
    } 
  }
}
