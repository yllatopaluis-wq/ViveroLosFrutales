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
