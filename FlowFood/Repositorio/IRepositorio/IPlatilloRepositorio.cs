using FlowFood.Models;

namespace FlowFood.Repositorio.IRepositorio
{
    public interface IPlatilloRepositorio
    {
        Task<ICollection<Platillo>> GetPlatillosAsync();
        Task<Platillo> GetPlatilloAsync(int id);
        Task<Platillo> GetPlatilloXCodigoAsync(string codigo);
        Task<bool> ExistePlatilloAsync(int id);
        Task<bool> ExistePlatilloXCodigoAsync(string codigo);
        Task<string> GenerarSiguienteCodigoAsync();
        Task<bool> CrearPlatilloAsync(Platillo platillo);
        Task<bool> ActualizarPlatilloAsync(Platillo platillo);
        Task<bool> BorrarPlatilloAsync(Platillo platillo);
        Task<bool> GuardarAsync();
    }
}
