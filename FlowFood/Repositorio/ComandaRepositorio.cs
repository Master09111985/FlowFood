using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class ComandaRepositorio : IComandaRepositorio
  {
    private readonly DataContext _context;

    public ComandaRepositorio(DataContext context)
    {
      _context = context;
    }

    // --- Consultas ---
    public async Task<ICollection<Comanda>> GetComandasAsync()
    {
      return await _context.Comandas
          .Include(c => c.Cliente)
          .Include(c => c.Plataforma)
          .Include(c => c.MetodoPago)
          .Include(c => c.Detalles)
            .ThenInclude(d => d.Platillo) // <-- CLAVE: Traer la info del platillo dentro del detalle
          .OrderByDescending(c => c.FechaRegistro)
          .ToListAsync();
    }

    public async Task<Comanda> GetComandaAsync(int id)
    {
      return await _context.Comandas
          .Include(c => c.Cliente)
          .Include(c => c.Plataforma)
          .Include(c => c.MetodoPago)
          .Include(c => c.Detalles)
            .ThenInclude(d => d.Platillo)
          .FirstOrDefaultAsync(c => c.Id == id);
    }

    // --- Validaciones ---
    public async Task<bool> ExisteComandaAsync(int id)
    {
      return await _context.Comandas.AnyAsync(c => c.Id == id);
    }

    // --- Paga de Comanda --
    public async Task<bool> ActualizarComandaAsync(Comanda comanda)
    {
      _context.Comandas.Update(comanda);
      return await GuardarAsync();
    }

    // --- CRUD y Operaciones ---
    public async Task<bool> CrearComandaAsync(Comanda comanda)
    {
      _context.Comandas.Add(comanda);
      return await GuardarAsync();
    }

    public async Task<bool> ActualizarEstatusComandaAsync(int id, int nuevoEstatus)
    {
      var comanda = await _context.Comandas.FindAsync(id);

      if (comanda != null)
      {
        comanda.Estatus = nuevoEstatus;
        _context.Comandas.Update(comanda);
        return await GuardarAsync();
      }
      return false;
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() > 0 ? true : false;
    }
  }
}
