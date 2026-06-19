using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record ProveedorListDto(int ProveedorId, TipoDocumentoCliente TipoDocumento, string NumeroDocumento, string RazonSocial, string NombreComercial, string Telefono, EstadoRegistro Estado);

public class ProveedorEditDto
{
    public int ProveedorId { get; set; }
    public TipoDocumentoCliente TipoDocumento { get; set; } = TipoDocumentoCliente.RUC;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}
