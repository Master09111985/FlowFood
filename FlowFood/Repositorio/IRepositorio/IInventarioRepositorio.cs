using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IInventarioRepositorio
  {
    Task<ICollection<Inventario>> GetInventariosAsync();
    Task<Inventario> GetInventarioAsync(int id);
    Task<ICollection<Inventario>> GetInventariosPorAlmacenAsync(int almacenId);
    Task<ICollection<Inventario>> GetInventariosPorProductoAsync(int productoId);
    Task<Inventario> GetInventarioXProductoYAlmacenAsync(int productoId, int almacenId);
    Task<bool> ExisteInventarioAsync(int id);
    Task<bool> ExisteInventarioXProductoYAlmacenAsync(int productoId, int almacenId);
    Task<decimal> GetCantidadTotalPorProductoAsync(int productoId);
    Task<bool> CrearInventarioAsync(Inventario inventario);
    Task<bool> ActualizarInventarioAsync(Inventario inventario);
    Task<bool> DescontarCantidadAsync(int inventarioId, decimal cantidad);
    Task<bool> BorrarInventarioAsync(Inventario inventario);
    Task<bool> GuardarAsync();
  }
}
