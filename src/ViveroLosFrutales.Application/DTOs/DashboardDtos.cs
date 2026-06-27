using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record DashboardDto(
    DateTime FechaDesde,
    DateTime FechaHasta,
    DashboardResumenDto Resumen,
    DashboardResumenDiarioDto Diario,
    IReadOnlyList<DashboardSerieDto> VentasPorDia,
    IReadOnlyList<DashboardSerieDto> ComprasPorDia,
    IReadOnlyList<DashboardCategoriaImporteDto> GastosPorCategoria,
    IReadOnlyList<DashboardProductoDto> ProductosMasVendidos,
    IReadOnlyList<DashboardComprobanteRecienteDto> UltimosComprobantes,
    IReadOnlyList<DashboardMovimientoDto> UltimosMovimientos,
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

public record DashboardResumenDiarioDto(
    DashboardIndicadorDiarioDto Ventas,
    DashboardIndicadorDiarioDto Compras,
    DashboardIndicadorDiarioDto Gastos,
    DashboardIndicadorDiarioDto DevolucionesPendientes);

public record DashboardIndicadorDiarioDto(string Titulo, decimal ValorHoy, decimal ValorAyer, decimal VariacionPorcentaje, bool EsMonto);
public record DashboardSerieDto(DateTime Fecha, decimal Importe);
public record DashboardCategoriaImporteDto(string Categoria, decimal Importe);
public record DashboardProductoDto(string Producto, decimal Cantidad, decimal Importe);
public record DashboardComprobanteRecienteDto(DateTime Fecha, string Numero, string Cliente, decimal Total, EstadoSunat EstadoSunat);
public record DashboardMovimientoDto(DateTime Fecha, string Tipo, string Numero, string Tercero, decimal Total, string Url, string Variante);
public record DashboardAlertaDto(string Titulo, string Detalle, string Nivel, string Url);