using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record GastoListDto(int GastoId, DateTime Fecha, string Categoria, string Descripcion, decimal Importe, string MedioPago, EstadoRegistro Estado);

public class GastoEditDto
{
    public int GastoId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public int? CategoriaGastoId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Importe { get; set; }
    public string MedioPago { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}

public record IngresoListDto(int IngresoId, DateTime Fecha, string TipoIngreso, string Descripcion, string MedioPago, decimal Importe, EstadoRegistro Estado);

public class IngresoEditDto
{
    public int IngresoId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Today;
    public int? CategoriaIngresoId { get; set; }
    public string TipoIngreso { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Importe { get; set; }
    public string MedioPago { get; set; } = "EFECTIVO";
    public string Observacion { get; set; } = string.Empty;
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}

public record CategoriaGastoOptionDto(int CategoriaGastoId, string Nombre);
public record CategoriaIngresoOptionDto(int CategoriaIngresoId, string Nombre);
