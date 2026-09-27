using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class PlatilloRepositorio : IPlatilloRepositorio
  {
    private readonly DataContext _context;
    public PlatilloRepositorio(DataContext context)
    {
      _context = context;
    }

    // Consultas
    public async Task<ICollection<Platillo>> GetPlatillosAsync()
    {
      return await _context.Platillos.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task<Platillo> GetPlatilloAsync(int id)
    {
      return await _context.Platillos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Platillo> GetPlatilloXCodigoAsync(string codigo)
    {
      return await _context.Platillos.FirstOrDefaultAsync(p => p.Codigo.Trim() == codigo.Trim());
    }

    // Para validaciones
    public async Task<bool> ExistePlatilloAsync(int id)
    {
      return await _context.Platillos.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> ExistePlatilloXCodigoAsync(string codigo)
    {
      return await _context.Platillos.AnyAsync(p => p.Codigo.Trim() == codigo.Trim());
    }

    // Generacion del codigo consecutivo
    public async Task<string> GenerarSiguienteCodigoAsync()
    {
      var ultimoPlatillo = await _context.Platillos
          .OrderByDescending(p => p.Id)
          .FirstOrDefaultAsync();

      if (ultimoPlatillo == null)
        return "PLA00001";

      var numeroActual = int.Parse(ultimoPlatillo.Codigo.Substring(3));
      var siguienteNumero = numeroActual + 1;

      return $"PLA{siguienteNumero:D5}";
    }

    // Para el CRUD
    public async Task<bool> CrearPlatilloAsync(Platillo platillo)
    {
      _context.Platillos.Add(platillo);
      return await GuardarAsync();
    }

    public async Task<bool> ActualizarPlatilloAsync(Platillo platillo)
    {
      var platilloExistente = await _context.Platillos.FindAsync(platillo.Id);

      if (platilloExistente != null)
      {
        _context.Entry(platilloExistente).CurrentValues.SetValues(platillo);
        return await GuardarAsync();
      }
      return false;
    }

    public async Task<bool> BorrarPlatilloAsync(Platillo platillo)
    {
      _context.Platillos.Remove(platillo);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
