using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IUnidadMedidaRepositorio
  {
    // Para obtener varios registros o solo uno
    Task<ICollection<UnidadMedida>> GetUnidadMedidasAsync();
    Task<UnidadMedida> GetUnidadMedidaAsync(int unidadMedidaId);

    // Para validacion
    Task<bool> ExisteUnidadMedidaAsync(int id);
    Task<bool> ExisteUnidadMedidaXNombreAsync(string nombre);

    // Para el CRUD
    Task<bool> CrearUnidadMedidaAsync(UnidadMedida unidadMedida);
    Task<bool> ActualizarUnidadMedidaAsync(UnidadMedida unidadMedida);
    Task<bool> BorrarUnidadMedidaAsync(UnidadMedida unidadMedida);
    Task<bool> GuardarAsync();
  }
}
