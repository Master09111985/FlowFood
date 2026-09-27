using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IMotivoBajaRepositorio
  {
    Task<ICollection<MotivoBaja>> GetMotivosAsync();
    Task<MotivoBaja> GetMotivoAsync(int id);
    Task<bool> ExisteMotivoAsync(int id);
    Task<bool> ExisteMotivoXNombreAsync(string nombre);
    Task<bool> CrearMotivoAsync(MotivoBaja motivo);
    Task<bool> ActualizarMotivoAsync(MotivoBaja motivo);
    Task<bool> BorrarMotivoAsync(MotivoBaja motivo);
    Task<bool> GuardarAsync();
  }
}
