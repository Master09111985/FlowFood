namespace FlowFood.Models.Dtos
{
  public class ComandaDto
  {
    public int Id { get; set; }
    public string TipoPedido { get; set; } // Ejemplo: "Local", "Plataforma"
    public string? NumeroMesa { get; set; }
    public int NumeroPlato { get; set; } = 1;
    public int Prioridad { get; set; }
    public string? PlataformaNombre { get; set; }
    public string? DireccionEntrega { get; set; }
    public DateTime? HoraEntrega { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; } // Ejemplo: "Cocinando", "Pagado"

    public List<ComandaDetalleDto> Detalles { get; set; }
  }

  public class ComandaDetalleDto
  {
    public int Id { get; set; }
    public int NumeroPlato { get; set; } = 1;
    public int PlatilloId { get; set; }
    public string PlatilloNombre { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string? Notas { get; set; }
  }
}
