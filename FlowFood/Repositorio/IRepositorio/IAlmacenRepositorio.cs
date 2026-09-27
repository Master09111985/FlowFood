using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IAlmacenRepositorio
  {
    // Para obtener varios registros o solo uno
    Task<ICollection<Almacen>> GetAlmacenesAsync();
    Task<Almacen> GetAlmacenAsync(int almacenId);

    // Para validacion
    Task<bool> ExisteAlmacenAsync(int id);
    Task<bool> ExistenteAlmacenXNombreAsync(string nombre);

    // Para el CRUD
    Task<bool> CrearAlmacenAsync(Almacen almacen);
    Task<bool> ActualizarAlmacenAsync(Almacen almacen);
    Task<bool> BorrarAlmacenAsync(Almacen almacen);
    Task<bool> GuardarAsync();
  }
}
