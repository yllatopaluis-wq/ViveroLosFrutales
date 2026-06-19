using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record DashboardDto(
    DateTime FechaDesde,
    DateTime FechaHasta,
    DashboardResumenDto Resumen,
    IReadOnlyList<DashboardSerieDto> VentasPorDia,
    IReadOnlyList<DashboardCategoriaImporteDto> GastosPorCategoria,
    IReadOnlyList<DashboardProductoDto> ProductosMasVendidos,
    IReadOnlyList<DashboardComprobanteRecienteDto> UltimosComprobantes,
    IReadOnlyList<DashboardAlertaDto> Alertas);

public record DashboardResumenDto(
    decimal TotalVentas,
    decimal TotalCompras,
    decimal TotalGastos,
    decimal TotalIngresos,
    decimal EstadoCuenta,
    int ComprobantesEmitidos,
    int Cotizaciones,
    int ComprobantesPendientesSunat,
    int ProductosBajoStock,
    int ClientesAtendidos);

public record DashboardSerieDto(DateTime Fecha, decimal Importe);
public record DashboardCategoriaImporteDto(string Categoria, decimal Importe);
public record DashboardProductoDto(string Producto, decimal Cantidad, decimal Importe);
public record DashboardComprobanteRecienteDto(DateTime Fecha, string Numero, string Cliente, decimal Total, EstadoSunat EstadoSunat);
public record DashboardAlertaDto(string Titulo, string Detalle, string Nivel, string Url);
