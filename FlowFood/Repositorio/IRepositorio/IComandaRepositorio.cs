using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IComandaRepositorio
  {
    // Consultas
    Task<ICollection<Comanda>> GetComandasAsync();
    Task<Comanda> GetComandaAsync(int id);

    // Validaciones
    Task<bool> ExisteComandaAsync(int id);

    // Pago de Comanda
    Task<bool> ActualizarComandaAsync(Comanda comanda);

    // CRUD y Operaciones
    Task<bool> CrearComandaAsync(Comanda comanda);
    Task<bool> ActualizarEstatusComandaAsync(int id, int nuevoEstatus);
    Task<bool> GuardarAsync();
  }
}
