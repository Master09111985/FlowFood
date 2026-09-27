using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class MotivoBajaRepositorio : IMotivoBajaRepositorio
  {
    private readonly DataContext _context;

    public MotivoBajaRepositorio(DataContext context)
    {
      _context = context;
    }

    public async Task<ICollection<MotivoBaja>> GetMotivosAsync()
    {
      return await _context.MotivosBaja.OrderBy(m => m.Nombre).ToListAsync();
    }

    public async Task<MotivoBaja> GetMotivoAsync(int id)
    {
      return await _context.MotivosBaja.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> ExisteMotivoAsync(int id)
    {
      return await _context.MotivosBaja.AnyAsync(m => m.Id == id);
    }

    public async Task<bool> ExisteMotivoXNombreAsync(string nombre)
    {
      return await _context.MotivosBaja.AnyAsync(m => m.Nombre.Trim().ToLower() == nombre.Trim().ToLower());
    }

    public async Task<bool> CrearMotivoAsync(MotivoBaja motivo)
    {
      _context.MotivosBaja.Add(motivo);
      return await GuardarAsync();
    }

    public async Task<bool> ActualizarMotivoAsync(MotivoBaja motivo)
    {
      var motivoExistente = await _context.MotivosBaja.FindAsync(motivo.Id);
      if (motivoExistente != null)
      {
        _context.Entry(motivoExistente).CurrentValues.SetValues(motivo);
        return await GuardarAsync();
      }
      return false;
    }

    public async Task<bool> BorrarMotivoAsync(MotivoBaja motivo)
    {
      _context.MotivosBaja.Remove(motivo);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
