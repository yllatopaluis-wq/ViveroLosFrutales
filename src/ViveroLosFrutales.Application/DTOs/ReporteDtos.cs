using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public static class ReporteGeneralIndicadores
{
    public const string Resultado = "Resultado";
    public const string Ventas = "Ventas";
    public const string Ingresos = "Ingresos";
    public const string Gastos = "Gastos";
    public const string Compras = "Compras";

    public static readonly string[] Todos = { Resultado, Ventas, Ingresos, Gastos, Compras };
}

public record ReporteGeneralMesDto(
    int Mes,
    string Nombre,
    IReadOnlyList<decimal> Valores);

public record ReporteGeneralDashboardMesDto(
    int Mes,
    string Nombre,
    string NombreCorto,
    decimal Ventas,
    decimal Ingresos,
    decimal Gastos,
    decimal Compras,
    decimal Resultado);

public record ReporteGeneralKpiDto(
    string Titulo,
    decimal Valor,
    string Subtitulo,
    decimal? VariacionPorcentaje,
    string Variante,
    bool EsMoneda = true);

public record ReporteGeneralAnualDto(
    int Anio,
    decimal Ventas,
    decimal Ingresos,
    decimal Gastos,
    decimal Compras,
    decimal Resultado,
    decimal? VariacionResultadoPorcentaje);

public class ReporteGeneralDto
{
    public int AnioDesde { get; set; }
    public int AnioHasta { get; set; }
    public string Indicador { get; set; } = ReporteGeneralIndicadores.Resultado;
    public IReadOnlyList<int> Anios { get; set; } = Array.Empty<int>();
    public IReadOnlyList<ReporteGeneralMesDto> Meses { get; set; } = Array.Empty<ReporteGeneralMesDto>();
    public IReadOnlyList<ReporteGeneralDashboardMesDto> DashboardMeses { get; set; } = Array.Empty<ReporteGeneralDashboardMesDto>();
    public IReadOnlyList<ReporteGeneralAnualDto> ResumenAnual { get; set; } = Array.Empty<ReporteGeneralAnualDto>();
    public IReadOnlyList<ReporteGeneralKpiDto> Kpis { get; set; } = Array.Empty<ReporteGeneralKpiDto>();
    public decimal TotalVentas { get; set; }
    public decimal TotalIngresos { get; set; }
    public decimal TotalGastos { get; set; }
    public decimal TotalCompras { get; set; }
    public decimal SaldoCaja { get; set; }
    public decimal TicketPromedio { get; set; }
    public int NumeroComprobantes { get; set; }
    public int ClientesAtendidos { get; set; }
    public decimal ProductosVendidos { get; set; }
    public decimal ResultadoTotal => TotalVentas + TotalIngresos - TotalGastos - TotalCompras;
}
public class ReporteNotasPedidoRequest : SearchRequest
{
    public string? Numero { get; set; }
    public EstadoPagoNotaPedido? EstadoPago { get; set; }
    public NotaPedidoEstado? EstadoDocumento { get; set; }
}

public record ReporteNotaPedidoRowDto(
    int NotaPedidoId,
    DateTime Fecha,
    string Numero,
    string Cliente,
    string NumeroDocumento,
    NotaPedidoEstado EstadoDocumento,
    EstadoPagoNotaPedido EstadoPago,
    string Vendedor,
    string TipoVenta,
    decimal Subtotal,
    decimal Igv,
    decimal Total,
    decimal TotalCobrado,
    decimal SaldoPendiente,
    string Observacion);

public record ReporteNotasPedidoResumenDto(
    int TotalNotas,
    decimal TotalVendido,
    decimal TotalCobrado,
    decimal SaldoPendiente,
    int NotasPendientes,
    int NotasParciales,
    int NotasPagadas,
    int NotasAnuladas);

public class ReporteNotasPedidoDto
{
    public ReporteNotasPedidoRequest Request { get; set; } = new();
    public PagedResult<ReporteNotaPedidoRowDto> Notas { get; set; } = new();
    public ReporteNotasPedidoResumenDto Resumen { get; set; } = new(0, 0, 0, 0, 0, 0, 0, 0);
}
public class ReporteComprobantesRequest : SearchRequest
{
    public TipoComprobante? TipoComprobante { get; set; }
    public string? Serie { get; set; }
    public string? Numero { get; set; }
    public string? Cliente { get; set; }
    public EstadoSunat? EstadoSunat { get; set; }
    public EstadoRegistro? EstadoComprobante { get; set; }
    public string? MedioPago { get; set; }
    public string? Vendedor { get; set; }
}

public record ReporteComprobanteRowDto(
    int ComprobanteId,
    DateTime Fecha,
    TipoComprobante TipoComprobante,
    string SerieNumero,
    string Cliente,
    string NumeroDocumento,
    string Moneda,
    decimal Gravado,
    decimal Igv,
    decimal Exonerado,
    decimal Total,
    decimal TotalCobrado,
    decimal SaldoPendiente,
    EstadoSunat EstadoSunat,
    EstadoRegistro Estado,
    string Vendedor,
    string MedioPago);

public record ReporteComprobantesResumenDto(
    int TotalComprobantes,
    decimal TotalImporte,
    decimal TotalIgv,
    decimal TotalGravado,
    decimal TotalExonerado,
    decimal TotalCancelado,
    decimal TotalPorCobrar);

public class ReporteComprobantesDto
{
    public ReporteComprobantesRequest Request { get; set; } = new();
    public PagedResult<ReporteComprobanteRowDto> Comprobantes { get; set; } = new();
    public ReporteComprobantesResumenDto Resumen { get; set; } = new(0, 0, 0, 0, 0, 0, 0);
    public IReadOnlyList<string> Series { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> MediosPago { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Vendedores { get; set; } = Array.Empty<string>();
}
public class ReporteMovimientoCajaRequest : SearchRequest
{
    public TipoMovimientoCaja? TipoMovimiento { get; set; }
    public OrigenMovimientoCaja? Origen { get; set; }
    public string? MedioPago { get; set; }
    public int? CuentaFinancieraId { get; set; }
}

public record ReporteMovimientoCajaRowDto(
    int MovimientoCajaId,
    DateTime Fecha,
    TipoMovimientoCaja TipoMovimiento,
    OrigenMovimientoCaja Origen,
    string OrigenDescripcion,
    string ClienteProveedor,
    string Documento,
    string MedioPago,
    string CuentaFinanciera,
    decimal Monto,
    string Descripcion);

public record ReporteMovimientoCajaResumenDto(
    int TotalMovimientos,
    decimal TotalIngresos,
    decimal TotalEgresos,
    decimal Saldo,
    int MovimientosIngreso,
    int MovimientosEgreso);

public class ReporteMovimientoCajaDto
{
    public ReporteMovimientoCajaRequest Request { get; set; } = new();
    public PagedResult<ReporteMovimientoCajaRowDto> Movimientos { get; set; } = new();
    public ReporteMovimientoCajaResumenDto Resumen { get; set; } = new(0, 0, 0, 0, 0, 0);
    public IReadOnlyList<string> MediosPago { get; set; } = Array.Empty<string>();
    public IReadOnlyList<CuentaFinancieraOptionDto> CuentasFinancieras { get; set; } = Array.Empty<CuentaFinancieraOptionDto>();
}
