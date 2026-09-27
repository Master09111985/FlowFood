using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IClienteRepositorio
  {
    Task<ICollection<Cliente>> GetClientesAsync();
    Task<Cliente> GetClienteAsync(int id);
    Task<Cliente> GetClienteXTelefonoAsync(string telefono);

    Task<bool> ExisteClienteAsync(int id);
    Task<bool> ExisteClienteXTelefonoAsync(string telefono);

    Task<bool> CrearClienteAsync(Cliente cliente);
    Task<bool> ActualizarClienteAsync(Cliente cliente);
    Task<bool> BorrarClienteAsync(Cliente cliente);
    Task<bool> GuardarAsync();
  }
}
