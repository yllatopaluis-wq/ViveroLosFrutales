using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record GastoListDto(int GastoId, DateTime Fecha, string Categoria, string Descripcion, decimal Importe, string MedioPago, string CuentaFinanciera, EstadoRegistro Estado);

public class GastoEditDto
{
    public int GastoId { get; set; }
    public DateTime Fecha { get; set; } = PeruDateTime.Today;
    public int? CategoriaGastoId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string DocumentoReferencia { get; set; } = string.Empty;
    public decimal Importe { get; set; }
    public string MedioPago { get; set; } = string.Empty;
    public int? ClienteId { get; set; }
    public int? CuentaFinancieraId { get; set; }
    public IReadOnlyList<CuentaFinancieraOptionDto> CuentasFinancieras { get; set; } = Array.Empty<CuentaFinancieraOptionDto>();
    public IReadOnlyList<ComprobanteClienteOptionDto> Clientes { get; set; } = Array.Empty<ComprobanteClienteOptionDto>();
    public FormularioConfiguracionDto FormularioConfiguracion { get; set; } = FormularioConfiguracionService.Defaults(FormularioConfiguracionService.TipoGasto);
    public string? Observacion { get; set; }
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}

public record IngresoListDto(int IngresoId, DateTime Fecha, string TipoIngreso, string Descripcion, string MedioPago, string CuentaFinanciera, decimal Importe, EstadoRegistro Estado);

public class IngresoEditDto
{
    public int IngresoId { get; set; }
    public DateTime Fecha { get; set; } = PeruDateTime.Today;
    public int? CategoriaIngresoId { get; set; }
    public string TipoIngreso { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string DocumentoReferencia { get; set; } = string.Empty;
    public decimal Importe { get; set; }
    public string MedioPago { get; set; } = "EFECTIVO";
    public int? ProveedorId { get; set; }
    public int? CuentaFinancieraId { get; set; }
    public IReadOnlyList<CuentaFinancieraOptionDto> CuentasFinancieras { get; set; } = Array.Empty<CuentaFinancieraOptionDto>();
    public IReadOnlyList<ProveedorListDto> Proveedores { get; set; } = Array.Empty<ProveedorListDto>();
    public FormularioConfiguracionDto FormularioConfiguracion { get; set; } = FormularioConfiguracionService.Defaults(FormularioConfiguracionService.TipoIngreso);
    public string? Observacion { get; set; }
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}

public record CategoriaGastoOptionDto(int CategoriaGastoId, string Nombre);
public record CategoriaIngresoOptionDto(int CategoriaIngresoId, string Nombre);





