using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IProveedorRepositorio
  {
    // Para obtener varios registros o solo uno
    Task<ICollection<Proveedor>> GetProveedoresAsync();
    Task<Proveedor> GetProveedorAsync(int proveedorId);

    // Para validacion
    Task<bool> ExisteProveedorAsync(int id);
    Task<bool> ExistenteProveedorXNombreAsync(string nombre);

    // Para el CRUD
    Task<bool> CrearProveedorAsync(Proveedor proveedor);
    Task<bool> ActualizarProveedorAsync(Proveedor proveedor);
    Task<bool> BorrarProveedorAsync(Proveedor proveedor);
    Task<bool> GuardarAsync();
  }
}
