using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Versioning;

namespace FlowFood.Repositorio
{
  public class UnidadMedidaRepositorio : IUnidadMedidaRepositorio
  {
    private readonly DataContext _context;
    public UnidadMedidaRepositorio(DataContext context)
    {
        _context = context;
    }

    // Esto para consultas
    public async Task<ICollection<UnidadMedida>> GetUnidadMedidasAsync()
    {
      return await _context.UnidadMedidas.OrderBy(u => u.Nombre).ToListAsync();
    }
    public async Task<UnidadMedida> GetUnidadMedidaAsync(int unidadMedidaId)
    {
      return await _context.UnidadMedidas.FirstOrDefaultAsync(u => u.Id == unidadMedidaId);
    }

    // Esto es para validaciones
    public async Task<bool> ExisteUnidadMedidaAsync(int id)
    {
      return await _context.UnidadMedidas.AnyAsync(u => u.Id == id);
    }
    public async Task<bool> ExisteUnidadMedidaXNombreAsync(string nombre)
    {
      bool valor = await _context.UnidadMedidas.AnyAsync(u => u.Nombre.ToLower().Trim() == nombre);
      return valor;
    }

    // Esto es para el CRUD
    public async Task<bool> CrearUnidadMedidaAsync(UnidadMedida unidadMedida)
    {
      _context.UnidadMedidas.Add(unidadMedida);
      return await GuardarAsync();
    }
    public async Task<bool> ActualizarUnidadMedidaAsync(UnidadMedida unidadMedida)
    {
      _context.UnidadMedidas.Update(unidadMedida);
      return await GuardarAsync();
    }
    public async Task<bool> BorrarUnidadMedidaAsync(UnidadMedida unidadMedida)
    {
      _context.UnidadMedidas.Remove(unidadMedida);
      return await GuardarAsync();
    }
    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() > 0 ? true : false;
    }
  }
}
