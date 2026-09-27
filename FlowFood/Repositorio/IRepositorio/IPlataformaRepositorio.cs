using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IPlataformaRepositorio
  {
    Task<ICollection<Plataforma>> GetPlataformasAsync();
    Task<Plataforma> GetPlataformaAsync(int id);
    Task<bool> ExistePlataformaAsync(int id);
    Task<bool> ExistePlataformaXNombreAsync(string nombre);
    Task<bool> CrearPlataformaAsync(Plataforma plataforma);
    Task<bool> ActualizarPlataformaAsync(Plataforma plataforma);
    Task<bool> BorrarPlataformaAsync(Plataforma plataforma);
    Task<bool> GuardarAsync();
  }
}
