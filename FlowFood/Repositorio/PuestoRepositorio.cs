using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;

using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class PuestoRepositorio : IPuestoRepositorio
  {
    private readonly DataContext _context;
    public PuestoRepositorio(DataContext context)
    {
      _context = context;
    }

    // Esto es para consultas
    public async Task<ICollection<Puesto>> GetPuestosAsync()
    {
      return await _context.Puestos.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task<Puesto> GetPuestoAsync(int puestoId)
    {
      return await _context.Puestos.FirstOrDefaultAsync(p => p.Id == puestoId);
    }

    // Esto es para validaciones
    public async Task<bool> ExistePuestoAsync(int id)
    {
      return await _context.Puestos.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> ExistePuestoXNombreAsync(string nombre)
    {
      return await _context.Puestos.AnyAsync(p => p.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
    }

    // Esto es para el CRUD
    public async Task<bool> CrearPuestoAsync(Puesto puesto)
    {
      _context.Puestos.Add(puesto);
      return await GuardarAsync();
    }

    // Actualizacion directa sobre la entidad rastreada por EF (sin AsNoTracking, sin SetValues)
    public async Task<bool> ActualizarPuestoAsync(Puesto puesto)
    {
      var puestoExistente = await _context.Puestos.FirstOrDefaultAsync(p => p.Id == puesto.Id);

      if (puestoExistente == null)
        return false; // el registro no existe, esto es un fallo real

      puestoExistente.Nombre = puesto.Nombre;
      puestoExistente.Estado = puesto.Estado;

      await _context.SaveChangesAsync();
      return true; // si el registro existe y no hubo excepcion, la operacion fue exitosa
    }

    public async Task<bool> BorrarPuestoAsync(Puesto puesto)
    {
      _context.Puestos.Remove(puesto);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0;
    }
  }
}
