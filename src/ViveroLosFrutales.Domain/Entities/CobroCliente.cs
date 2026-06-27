using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class CobroCliente : EmpresaEntity
{
    public int CobroClienteId { get; set; }
    public int ClienteId { get; set; }
    public int? NotaPedidoId { get; set; }
    public int? ComprobanteId { get; set; }
    public DateTime FechaCobro { get; set; } = PeruDateTime.Today;
    public decimal Monto { get; set; }
    public string MedioPago { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public new CobroClienteEstado Estado { get; set; } = CobroClienteEstado.ACTIVO;
    public DateTime? FechaAnulacion { get; set; }
    public string UsuarioAnulacion { get; set; } = string.Empty;
    public string MotivoAnulacion { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
    public Cliente? Cliente { get; set; }
    public NotaPedido? NotaPedido { get; set; }
    public Comprobante? Comprobante { get; set; }
    public ICollection<ComprobanteCobroAplicado> Aplicaciones { get; set; } = new List<ComprobanteCobroAplicado>();
}
