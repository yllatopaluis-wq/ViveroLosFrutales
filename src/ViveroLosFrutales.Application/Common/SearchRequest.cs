namespace ViveroLosFrutales.Application.Common;

public class SearchRequest
{
    public string? Search { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public int? Estado { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}