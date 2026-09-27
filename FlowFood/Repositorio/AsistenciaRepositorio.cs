using FlowFood.Data;
using FlowFood.Models;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FlowFood.Repositorio
{
  public class AsistenciaRepositorio : IAsistenciaRepositorio
  {
    private readonly DataContext _bd;

    public AsistenciaRepositorio(DataContext bd)
    {
      _bd = bd;
    }

    public async Task<RespuestaChecadaDto> RegistrarChecadaAsync(string codigoEmpleado)
    {
      // 1. Validar que el código pertenezca a un empleado activo
      var empleado = await _bd.Empleados
          .FirstOrDefaultAsync(e => e.Codigo == codigoEmpleado && e.Estado == true);

      if (empleado == null)
      {
        return new RespuestaChecadaDto
        {
          Mensaje = "Error: Código no válido o empleado inactivo.",
          NombreEmpleado = "Desconocido"
        };
      }

      // 2. Fijar la fecha y hora actual en Aguascalientes (UTC-6)
      var horaActual = DateTime.UtcNow.AddHours(-6);
      var fechaHoy = horaActual.Date;

      // 3. Obtener el historial de checadas del empleado de HOY
      var checadasHoy = await _bd.Asistencias
          .Where(a => a.EmpleadoId == empleado.Id && a.FechaHora.Date == fechaHoy)
          .OrderBy(a => a.FechaHora)
          .ToListAsync();

      // 4. Determinar la lógica de los 4 turnos
      int numeroChecada = checadasHoy.Count + 1;
      string nombreTurno = "";

      if (numeroChecada > 4)
      {
        return new RespuestaChecadaDto
        {
          NombreEmpleado = empleado.Nombre,
          NombreChecada = "Jornada Completa",
          FechaHora = horaActual,
          Mensaje = "Atención: Ya completaste tus 4 registros de hoy."
        };
      }

      switch (numeroChecada)
      {
        case 1: nombreTurno = "Entrada"; break;
        case 2: nombreTurno = "Salida a Comida"; break;
        case 3: nombreTurno = "Regreso de Comida"; break;
        case 4: nombreTurno = "Salida Final"; break;
      }

      // 5. Guardar la nueva asistencia
      var nuevaAsistencia = new Asistencia
      {
        EmpleadoId = empleado.Id,
        FechaHora = horaActual,
        TipoChecada = numeroChecada
      };

      _bd.Asistencias.Add(nuevaAsistencia);
      await _bd.SaveChangesAsync();

      // 6. Retornar el DTO para que Angular muestre el Toast
      return new RespuestaChecadaDto
      {
        NombreEmpleado = empleado.Nombre,
        NombreChecada = nombreTurno,
        FechaHora = horaActual,
        Mensaje = $"¡{nombreTurno} registrada correctamente!"
      };
    }
  }
}
