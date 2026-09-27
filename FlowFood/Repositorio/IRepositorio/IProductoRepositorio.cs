using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IProductoRepositorio
  {
    Task<ICollection<Producto>> GetProductosAsync();
    Task<Producto> GetProductoAsync(int id);
    Task<Producto> GetProductoXNombreAsync(string nombre);
    Task<bool> ExisteProductoAsync(int id);
    Task<bool> ExisteProductoXNombreAsync(string nombre);
    Task<decimal> GetCantidadTotalAsync(int productoId);
    Task<bool> CrearProductoAsync(Producto producto);
    Task<bool> ActualizarProductoAsync(Producto producto);
    Task<bool> BorrarProductoAsync(Producto producto);
    Task<bool> GuardarAsync();
  }
}
