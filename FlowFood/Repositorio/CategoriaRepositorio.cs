using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class CategoriaRepositorio : ICategoriaRepositorio
  {
    private readonly DataContext _context;
    public CategoriaRepositorio(DataContext context)
    {
        _context = context;
    }

    // Esto es para consultas
    public async Task<ICollection<Categoria>> GetCategoriasAsync()
    {
      return await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync();
    }
    public async Task<Categoria> GetCategoriaAsync(int categoriaId)
    {
      return await _context.Categorias.FirstOrDefaultAsync(c => c.Id == categoriaId);
    }

    // Esto es para validaciones
    public async Task<bool> ExisteCategoriaAsync(int id)
    {
      return await _context.Categorias.AnyAsync(c => c.Id == id);
    }
    public async Task<bool> ExisteCategoriaXNombreAsync(string nombre)
    {
      return await _context.Categorias.AnyAsync(c => c.Nombre.ToLower().Trim() == nombre);
    }

    // Esto es para el CRUD
    public async Task<bool> CrearCategoriaAsync(Categoria categoria)
    {
      _context.Categorias.Add(categoria);
      return await GuardarAsync();
    }
    public async Task<bool> ActualizarCategoriaAsync(Categoria categoria)
    {
      var categoriaExistente = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.Id == categoria.Id);
      if (categoriaExistente != null)
        _context.Entry(categoriaExistente).CurrentValues.SetValues(categoria);
      else
        _context.Categorias.Update(categoria);

      return await GuardarAsync();
    }
    public async Task<bool> BorrarCategoriaAsync(Categoria categoria)
    {
      _context.Categorias.Remove(categoria);
      return await GuardarAsync();
    }

    public async Task<bool> GuardarAsync()
    {
      return await _context.SaveChangesAsync() >= 0 ? true : false;
    }
  }
}
