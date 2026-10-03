using FlowFood.Data;
using FlowFood.Models.Dtos;
using FlowFood.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace FlowFood.Repositorio
{
    public class NominaRepositorio : INominaRepositorio
    {
        private readonly DataContext _context;

        // 48 horas a la semana (6 días de 8 horas) = 2880 minutos
        private const decimal MINUTOS_SEMANALES_BASE = 2880m;
        private const int MINUTOS_JORNADA_DIARIA = 480; // 8 horas por día
        private static readonly TimeSpan HORA_ENTRADA_OFICIAL = new TimeSpan(8, 0, 0); // 8:00 AM

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
                int totalMinutosRetardo = 0;

                // Agrupamos por día para no mezclar turnos de fechas diferentes
                var porDias = asistenciasEmpleado.GroupBy(a => a.FechaHora.Date);

                foreach (var dia in porDias)
                {
                    var checadas = dia.ToList();

                    // Extraemos las 4 checadas: 1=Entrada, 2=Entrada al Comedor, 3=Salida del Comedor, 4=Salida
                    var entrada = checadas.FirstOrDefault(c => c.TipoChecada == 1);
                    var entradaComida = checadas.FirstOrDefault(c => c.TipoChecada == 2);
                    var salidaComida = checadas.FirstOrDefault(c => c.TipoChecada == 3);
                    var salida = checadas.FirstOrDefault(c => c.TipoChecada == 4);

                    if (entrada == null) continue;

                    // Hora oficial de entrada de ese día: 8:00 AM
                    DateTime horaOficialDia = dia.Key.Add(HORA_ENTRADA_OFICIAL);

                    // 1. Calculamos retardo si checó a partir de las 8:01 AM
                    int retardoDia = 0;
                    if (entrada.FechaHora > horaOficialDia)
                    {
                        retardoDia = (int)Math.Floor((entrada.FechaHora - horaOficialDia).TotalMinutes);
                        totalMinutosRetardo += retardoDia;
                    }

                    // Si llegó antes de las 8:00 AM, el conteo efectivo inicia a las 8:00 AM
                    DateTime inicioEfectivo = entrada.FechaHora < horaOficialDia ? horaOficialDia : entrada.FechaHora;

                    int minutosDia = 0;

                    // Bloque 1: De la Entrada (o 8:00 AM) a la Entrada al Comedor
                    if (entradaComida != null && entradaComida.FechaHora > inicioEfectivo)
                    {
                        minutosDia += (int)Math.Floor((entradaComida.FechaHora - inicioEfectivo).TotalMinutes);
                    }

                    // Bloque 2: De la Salida del Comedor a la Salida final
                    if (salidaComida != null && salida != null && salida.FechaHora > salidaComida.FechaHora)
                    {
                        minutosDia += (int)Math.Floor((salida.FechaHora - salidaComida.FechaHora).TotalMinutes);
                    }
                    // Caso alternativo: Si ese día solo registró Entrada (1) y Salida (4) sin comedor
                    else if (entradaComida == null && salidaComida == null && salida != null && salida.FechaHora > inicioEfectivo)
                    {
                        minutosDia += (int)Math.Floor((salida.FechaHora - inicioEfectivo).TotalMinutes);
                    }

                    // Topamos los minutos del día a (480 min - retardoDia) para que el retardo de las 8:00 AM
                    // siempre se descuente proporcionalmente y no se compense saliendo más tarde
                    int maximoPermitidoDia = Math.Max(0, MINUTOS_JORNADA_DIARIA - retardoDia);
                    if (minutosDia > maximoPermitidoDia)
                    {
                        minutosDia = maximoPermitidoDia;
                    }

                    totalMinutos += minutosDia;
                }

                // Solo agregamos al reporte a los empleados que registraron minutos trabajados
                if (totalMinutos > 0 || totalMinutosRetardo > 0)
                {
                    decimal pagoPorMinuto = empleado.SalarioSemanal / MINUTOS_SEMANALES_BASE;
                    decimal descuentoPorRetardos = Math.Round(totalMinutosRetardo * pagoPorMinuto, 2);
                    decimal totalAPagar = Math.Round(totalMinutos * pagoPorMinuto, 2);

                    reporte.Add(new ReporteNominaDto
                    {
                        EmpleadoId = empleado.Id,
                        NombreEmpleado = empleado.Nombre,
                        SalarioSemanal = empleado.SalarioSemanal,
                        PagoPorMinuto = Math.Round(pagoPorMinuto, 4),
                        TotalMinutosTrabajados = totalMinutos,
                        MinutosRetardo = totalMinutosRetardo,
                        DescuentoRetardos = descuentoPorRetardos,
                        TotalAPagar = totalAPagar,
                        TotalAsistencias = asistenciasEmpleado.Count
                    });
                }
            }

            return reporte.OrderBy(r => r.NombreEmpleado).ToList();
        }
    }
}