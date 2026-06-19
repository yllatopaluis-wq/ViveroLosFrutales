namespace ViveroLosFrutales.Domain.Entities;

public class ComprobanteCobroAplicado
{
    public int ComprobanteCobroAplicadoId { get; set; }
    public int EmpresaId { get; set; }
    public int ComprobanteId { get; set; }
    public int CobroClienteId { get; set; }
    public decimal MontoAplicado { get; set; }
    public DateTime FechaAplicacion { get; set; } = DateTime.UtcNow;
    public string UsuarioRegistro { get; set; } = string.Empty;
    public Comprobante? Comprobante { get; set; }
    public CobroCliente? CobroCliente { get; set; }
}
