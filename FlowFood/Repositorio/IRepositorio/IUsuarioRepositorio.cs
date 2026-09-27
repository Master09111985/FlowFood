using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IUsuarioRepositorio
  {
    Task<ICollection<Usuario>> GetUsuariosAsync();
    Task<Usuario> GetUsuarioAsync(int id);
    Task<Usuario> GetUsuarioXNombreAsync(string nombre);
    Task<bool> ExisteUsuarioAsync(int id);
    Task<bool> ExisteUsuarioXNombreAsync(string nombre);
    Task<bool> ExisteUsuarioXNombreEmpleadoAsync(int id);
    Task<bool> CrearUsuarioAsync(Usuario usuario);
    Task<bool> ActualizarUsuarioAsync(Usuario usuario);
    Task<bool> BorrarUsuarioAsync(Usuario usuario);
    Task<bool> GuardarAsync();
  }
}
