using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record NotaPedidoListDto(
    int NotaPedidoId,
    string Numero,
    DateTime Fecha,
    string Cliente,
    decimal Total,
    decimal TotalCobrado,
    decimal SaldoPendiente,
    NotaPedidoEstado EstadoDocumento,
    EstadoPagoNotaPedido EstadoPago,
    string EstadoDevolucion,
    int? ComprobanteId);

public class NotaPedidoEditDto
{
    public int NotaPedidoId { get; set; }
    public int? CotizacionId { get; set; }
    public int ClienteId { get; set; }
    public string Serie { get; set; } = string.Empty;
    public int Correlativo { get; set; }
    public DateTime Fecha { get; set; } = PeruDateTime.Today;
    public List<NotaPedidoDetalleEditDto> Detalles { get; set; } = new() { new NotaPedidoDetalleEditDto() };
}

public class NotaPedidoDetalleEditDto
{
    public int ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

public class NotaPedidoFormDataDto
{
    public NotaPedidoEditDto NotaPedido { get; set; } = new();
    public IReadOnlyList<ComprobanteClienteOptionDto> Clientes { get; set; } = Array.Empty<ComprobanteClienteOptionDto>();
    public IReadOnlyList<ComprobanteProductoOptionDto> Productos { get; set; } = Array.Empty<ComprobanteProductoOptionDto>();
}

public record AnularNotaPedidoResultadoDto(bool GeneroDevolucion, decimal MontoDevolucion, string Mensaje);

public record NotaPedidoDetalleDto(string Producto, decimal Cantidad, decimal PrecioUnitario, decimal Subtotal, decimal Igv, decimal Total);

public class NotaPedidoDetalleViewDto
{
    public int NotaPedidoId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int ClienteId { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public TipoDocumentoCliente ClienteTipoDocumento { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Igv { get; set; }
    public decimal Total { get; set; }
    public decimal TotalCobrado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public NotaPedidoEstado EstadoDocumento { get; set; }
    public EstadoPagoNotaPedido EstadoPago { get; set; }
    public int? ComprobanteId { get; set; }
    public IReadOnlyList<NotaPedidoDetalleDto> Detalles { get; set; } = Array.Empty<NotaPedidoDetalleDto>();
    public IReadOnlyList<CobroClienteListDto> Cobros { get; set; } = Array.Empty<CobroClienteListDto>();
    public IReadOnlyList<ComprobanteListDto> Comprobantes { get; set; } = Array.Empty<ComprobanteListDto>();
}

public class RegistrarCobroDto
{
    public int? NotaPedidoId { get; set; }
    public int? ComprobanteId { get; set; }
    public DateTime FechaCobro { get; set; } = PeruDateTime.Today;
    public decimal Monto { get; set; }
    public string MedioPago { get; set; } = "EFECTIVO";
    public string Observacion { get; set; } = string.Empty;
}

public record CobroClienteListDto(
    int CobroClienteId,
    DateTime FechaCobro,
    string Cliente,
    string Referencia,
    decimal Monto,
    string MedioPago,
    CobroClienteEstado Estado,
    string Observacion);

public record MovimientoCajaListDto(
    int MovimientoCajaId,
    DateTime Fecha,
    TipoMovimientoCaja TipoMovimiento,
    OrigenMovimientoCaja Origen,
    string OrigenDescripcion,
    string ClienteProveedor,
    string Documento,
    string MedioPago,
    decimal Monto,
    EstadoRegistro Estado);

public record CajaResumenDto(decimal Ingresos, decimal Egresos, decimal Saldo, int Movimientos);

public class CajaIndexDto
{
    public IReadOnlyList<MovimientoCajaListDto> Movimientos { get; set; } = Array.Empty<MovimientoCajaListDto>();
    public CajaResumenDto Resumen { get; set; } = new(0, 0, 0, 0);
    public int Total { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class EstadoCuentaClienteDto
{
    public int ClienteId { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public decimal TotalPedidos { get; set; }
    public decimal TotalCobrado { get; set; }
    public decimal SaldoPendiente { get; set; }
    public IReadOnlyList<NotaPedidoListDto> Pedidos { get; set; } = Array.Empty<NotaPedidoListDto>();
    public IReadOnlyList<CobroClienteListDto> Cobros { get; set; } = Array.Empty<CobroClienteListDto>();
    public IReadOnlyList<ComprobanteListDto> Comprobantes { get; set; } = Array.Empty<ComprobanteListDto>();
}

public record DevolucionListDto(
    int DevolucionId,
    DateTime FechaGeneracion,
    TipoTerceroDevolucion TipoTercero,
    string Tercero,
    OrigenDevolucion Origen,
    string OrigenDescripcion,
    string DocumentoOrigen,
    decimal MontoOriginal,
    decimal MontoDevuelto,
    decimal MontoPendiente,
    EstadoDevolucion Estado,
    bool PuedeRegistrarDevolucion,
    bool PuedeAnular);

public class DevolucionDetalleDto
{
    public int DevolucionId { get; set; }
    public DateTime FechaGeneracion { get; set; }
    public TipoTerceroDevolucion TipoTercero { get; set; }
    public string Tercero { get; set; } = string.Empty;
    public OrigenDevolucion Origen { get; set; }
    public string OrigenDescripcion { get; set; } = string.Empty;
    public string DocumentoOrigen { get; set; } = string.Empty;
    public decimal MontoOriginal { get; set; }
    public decimal MontoDevuelto { get; set; }
    public decimal MontoPendiente { get; set; }
    public EstadoDevolucion Estado { get; set; }
    public string Observacion { get; set; } = string.Empty;
    public string MotivoGeneracion { get; set; } = string.Empty;
}

public class RegistrarDevolucionDto
{
    public int DevolucionId { get; set; }
    public TipoTerceroDevolucion TipoTercero { get; set; }
    public string Tercero { get; set; } = string.Empty;
    public string DocumentoOrigen { get; set; } = string.Empty;
    public decimal MontoPendiente { get; set; }
    public decimal MontoDevolver { get; set; }
    public string MedioDevolucion { get; set; } = "EFECTIVO";
    public DateTime Fecha { get; set; } = PeruDateTime.Today;
    public string Observacion { get; set; } = string.Empty;
}
