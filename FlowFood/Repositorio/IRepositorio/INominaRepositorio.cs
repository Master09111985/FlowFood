using FlowFood.Models.Dtos;

namespace FlowFood.Repositorio.IRepositorio
{
  public interface INominaRepositorio
  {
    Task<List<ReporteNominaDto>> GenerarReporteNominaAsync(DateTime fechaInicio, DateTime fechaFin);
  }
}
