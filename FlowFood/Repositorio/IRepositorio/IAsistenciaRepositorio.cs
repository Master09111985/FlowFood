using FlowFood.Models.Dtos;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface IAsistenciaRepositorio
  {
    Task<RespuestaChecadaDto> RegistrarChecadaAsync(string codigoEmpleado);
  }
}
