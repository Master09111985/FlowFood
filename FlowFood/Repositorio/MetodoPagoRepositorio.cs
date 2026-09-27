using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class MetodoPagoRepositorio : IMetodoPagoRepositorio
  {
    private readonly DataContext _context;
    public MetodoPagoRepositorio(DataContext context)
    {
        _context = context;
    }

    // Esto es para consultas
    public async Task<ICollection<MetodoPago>> GetMetodosPagoAsync()
    {
      return await _context.MetodosPago.OrderBy(m => m.Nombre).ToListAsync();
    }
    public async Task<MetodoPago> GetMetodoPagoAsync(int metodoPagoId)
    {
      return await _context.MetodosPago.FirstOrDefaultAsync(m => m.Id == metodoPagoId);
    }

    // Esto es para validaciones
    public async Task<bool> ExisteMetodoPagoAsync(int id)
    {
      return await _context.MetodosPago.AnyAsync(m => m.Id == id);
    }
    public async Task<bool> ExisteMetodoPagoXNombreAsync(string nombre)
    {
      bool valor = await _context.MetodosPago.AnyAsync(m => m.Nombre == nombre);
      return valor;
    }

    // Esto es para el CRUD
    public async Task<bool> CrearMetodoPagoAsync(MetodoPago metodoPago)
    {
      _context.MetodosPago.Add(metodoPago);
      return await GuaradrAsync();
    }
    public async Task<bool> ActualizarMetodoPagoAsync(MetodoPago metodoPago)
    {
      _context.MetodosPago.Update(metodoPago);
      return await GuaradrAsync();
    }
    public async Task<bool> BorrarMetodoPago(MetodoPago metodoPago)
    {
      _context.MetodosPago.Remove(metodoPago);
      return await GuaradrAsync();
    }

    public async Task<bool> GuaradrAsync()
    {
      return await _context.SaveChangesAsync() > 0 ? true : false;
    }
  }
}
