using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record EmpresaListDto(int EmpresaId, string RUC, string RazonSocial, string NombreComercial, string Telefono, EstadoRegistro Estado);

public record EmpresaMarcaDto(int EmpresaId, string NombreComercial, string RazonSocial, bool TieneLogo);

public class EmpresaEditDto
{
    public int EmpresaId { get; set; }
    public string RUC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? NombreComercial { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string MonedaPredeterminada { get; set; } = "PEN";
    public string? UrlNubefact { get; set; }
    public string? TokenNubefact { get; set; }
    public string? LogoPath { get; set; }
    public byte[]? LogoContenido { get; set; }
    public string? LogoContentType { get; set; }
    public string? LogoNombre { get; set; }
    public string? RepresentanteLegalNombre { get; set; }
    public string? RepresentanteLegalDocumento { get; set; }
    public string? RepresentanteLegalCargo { get; set; }
    public byte[]? FirmaContenido { get; set; }
    public string? FirmaContentType { get; set; }
    public string? FirmaNombre { get; set; }
    public string SerieBoleta { get; set; } = "B001";
    public string SerieFactura { get; set; } = "F001";
    public string SerieNotaCredito { get; set; } = "NC001";
    public string SerieNotaCreditoFactura { get; set; } = "F101";
    public string SerieNotaCreditoBoleta { get; set; } = "B101";
    public string SerieNotaPedido { get; set; } = "NP001";
    public string SerieCotizacion { get; set; } = "C001";
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}
