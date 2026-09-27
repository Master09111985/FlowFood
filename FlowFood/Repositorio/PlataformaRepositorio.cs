using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class PlataformaRepositorio : IPlataformaRepositorio
  {
    private readonly DataContext _context;

    public PlataformaRepositorio(DataContext context)
    {
      _context = context;
    }

    // Consultas
    public async Task<ICollection<Plataforma>> GetPlataformasAsync()
    {
      return await _context.Plataformas.OrderBy(p => p.Nombre).ToListAsync();
    }
    public async Task<Plataforma> GetPlataformaAsync(int plataformaId)
    {
      return await _context.Plataformas.FirstOrDefaultAsync(p => p.Id == plataformaId);
    }

    // Esto es para validaciones
    public async Task<bool> ExistePlataformaAsync(int id)
    {
      return await _context.Plataformas.AnyAsync(p => p.Id == id);
    }
    public async Task<bool> ExistePlataformaXNombreAsync(string nombre)
    {
      bool valor = await _context.Plataformas.AnyAsync(p => p.Nombre == nombre);
      return valor;
    }

    // Esto es para el CRUD
    public async Task<bool> CrearPlataformaAsync(Plataforma plataforma)
    {
      _context.Plataformas.Add(plataforma);
      return await GuardarAsync();
    }
    public async Task<bool> ActualizarPlataformaAsync(Plataforma plataforma)
    {
      _context.Plataformas.Update(plataforma);
      return await GuardarAsync();
    }
    public async Task<bool> BorrarPlataformaAsync(Plataforma plataforma)
    {
      _context.Plataformas.Remove(plataforma);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() > 0 ? true : false;
    }
  }
}
