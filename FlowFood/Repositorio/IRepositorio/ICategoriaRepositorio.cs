using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface ICategoriaRepositorio
  {
    // Para obtener varios registros o solo uno
    Task<ICollection<Categoria>> GetCategoriasAsync();
    Task<Categoria> GetCategoriaAsync(int categoriaId);

    // Para validacion
    Task<bool> ExisteCategoriaAsync(int id);
    Task<bool> ExisteCategoriaXNombreAsync(string nombre);

    // Para el CRUD
    Task<bool> CrearCategoriaAsync(Categoria categoria);
    Task<bool> ActualizarCategoriaAsync(Categoria categoria);
    Task<bool> BorrarCategoriaAsync(Categoria categoria);
    Task<bool> GuardarAsync();
  }
}
