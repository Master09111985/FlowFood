using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IMetodoPagoRepositorio
  {
    // Para obtener varios registros o solo uno.
    Task<ICollection<MetodoPago>> GetMetodosPagoAsync();
    Task<MetodoPago> GetMetodoPagoAsync(int metodoPagoId);

    // Para validacion
    Task<bool> ExisteMetodoPagoAsync(int id);
    Task<bool> ExisteMetodoPagoXNombreAsync(string nombre);

    // Para el CRUD
    Task<bool> CrearMetodoPagoAsync(MetodoPago metodoPago);
    Task<bool> ActualizarMetodoPagoAsync(MetodoPago metodoPago);
    Task<bool> BorrarMetodoPago(MetodoPago metodoPago);
    Task<bool> GuaradrAsync();
  }
}
