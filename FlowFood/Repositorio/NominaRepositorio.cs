using FlowFood.Data;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
  public class NominaRepositorio : INominaRepositorio
  {
    private readonly DataContext _context;
    // 48 horas a la semana = 2880 minutos
    private const decimal MINUTOS_SEMANALES_BASE = 2880m;

    public NominaRepositorio(DataContext context)
    {
      _context = context;
    }

    public async Task<List<ReporteNominaDto>> GenerarReporteNominaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
      var reporte = new List<ReporteNominaDto>();

      // Ajustamos la fecha fin para abarcar hasta las 23:59:59 de ese día
      var finAjustado = fechaFin.Date.AddDays(1).AddTicks(-1);

      // Obtenemos solo empleados activos
      var empleados = await _context.Empleados.Where(e => e.Estado).ToListAsync();

      // Obtenemos todas las asistencias del rango
      var asistencias = await _context.Asistencias
          .Where(a => a.FechaHora >= fechaInicio.Date && a.FechaHora <= finAjustado)
          .ToListAsync();

      foreach (var empleado in empleados)
      {
        var asistenciasEmpleado = asistencias
            .Where(a => a.EmpleadoId == empleado.Id)
            .OrderBy(a => a.FechaHora)
            .ToList();

        int totalMinutos = 0;

        // Agrupamos por día para no mezclar turnos de fechas diferentes
        var porDias = asistenciasEmpleado.GroupBy(a => a.FechaHora.Date);

        foreach (var dia in porDias)
        {
          var checadas = dia.ToList();

          // Extraemos las 4 checadas según el ID que configuraste
          var entrada = checadas.FirstOrDefault(c => c.TipoChecada == 1);
          var entradaComida = checadas.FirstOrDefault(c => c.TipoChecada == 2);
          var salidaComida = checadas.FirstOrDefault(c => c.TipoChecada == 3);
          var salida = checadas.FirstOrDefault(c => c.TipoChecada == 4);

          // Bloque 1: De la Entrada a la EntradaComida
          if (entrada != null && entradaComida != null)
          {
            totalMinutos += (int)(entradaComida.FechaHora - entrada.FechaHora).TotalMinutes;
          }

          // Bloque 2: De la SalidaComida a la Salida final
          if (salidaComida != null && salida != null)
          {
            totalMinutos += (int)(salida.FechaHora - salidaComida.FechaHora).TotalMinutes;
          }
        }

        // Solo agregamos al reporte a los empleados que sí trabajaron (tienen minutos)
        if (totalMinutos > 0)
        {
          decimal pagoPorMinuto = empleado.SalarioSemanal / MINUTOS_SEMANALES_BASE;

          reporte.Add(new ReporteNominaDto
          {
            EmpleadoId = empleado.Id,
            NombreEmpleado = empleado.Nombre,
            SalarioSemanal = empleado.SalarioSemanal,
            PagoPorMinuto = Math.Round(pagoPorMinuto, 4),
            TotalMinutosTrabajados = totalMinutos,
            TotalAPagar = Math.Round(totalMinutos * pagoPorMinuto, 2),
            TotalAsistencias = asistenciasEmpleado.Count
          });
        }
      }

      return reporte.OrderBy(r => r.NombreEmpleado).ToList();
    }
  }
}
