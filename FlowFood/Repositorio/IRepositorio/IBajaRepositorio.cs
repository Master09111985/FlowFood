using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IBajaRepositorio
  {
    Task<ICollection<Baja>> GetBajasAsync();
    Task<Baja> GetBajaAsync(int id);
    Task<ICollection<Baja>> GetBajasPorProductoAsync(int productoId);
    Task<ICollection<Baja>> GetBajasPorAlmacenAsync(int almacenId);
    Task<bool> ExisteBajaAsync(int id);
    Task<bool> CrearBajaAsync(Baja baja);
    Task<bool> GuardarAsync();
  }
}
