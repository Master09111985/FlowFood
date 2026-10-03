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
            var codigoLimpio = codigoEmpleado.Trim();

            // 1. Validar que el código pertenezca a un empleado activo
            var empleado = await _bd.Empleados
                .FirstOrDefaultAsync(e => e.Codigo == codigoLimpio && e.Estado == true);

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
            var finDia = fechaHoy.AddDays(1);

            // 3. Obtener el historial de checadas del empleado de HOY
            var checadasHoy = await _bd.Asistencias
                .Where(a => a.EmpleadoId == empleado.Id && a.FechaHora >= fechaHoy && a.FechaHora < finDia)
                .OrderBy(a => a.FechaHora)
                .ToListAsync();

            // Candado anti-doble escaneo (evita quemar el siguiente turno si escanea 2 veces en menos de 1 minuto)
            var ultimaChecada = checadasHoy.LastOrDefault();
            if (ultimaChecada != null && (horaActual - ultimaChecada.FechaHora).TotalSeconds < 60)
            {
                return new RespuestaChecadaDto
                {
                    NombreEmpleado = empleado.Nombre,
                    NombreChecada = "Escaneo duplicado",
                    FechaHora = horaActual,
                    Mensaje = "Atención: Acabas de registrar tu asistencia hace unos segundos. Espera 1 minuto."
                };
            }

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
                case 2: nombreTurno = "Entrada al Comedor"; break;
                case 3: nombreTurno = "Salida del Comedor"; break;
                case 4: nombreTurno = "Salida"; break;
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