namespace FlowFood.Models.Dtos
{
  public class TicketCorteDto
  {
    public int TurnoId { get; set; }
    public string NombreCajero { get; set; }
    public string NombreSupervisor { get; set; }
    public DateTime FechaApertura { get; set; }
    public DateTime FechaCierre { get; set; }

    public decimal FondoInicial { get; set; }
    public decimal TotalVentasEfectivo { get; set; }
    public decimal TotalVentasTarjeta { get; set; }
    public decimal TotalPagosProveedores { get; set; }

    public decimal EfectivoCalculadoSistema { get; set; }
    public decimal EfectivoFisicoReportado { get; set; }
    public decimal Diferencia { get; set; } // Negativo = Faltante, Positivo = Sobrante
  }
}
