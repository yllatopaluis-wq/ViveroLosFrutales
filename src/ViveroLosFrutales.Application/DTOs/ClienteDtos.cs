using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record ClienteListDto(int ClienteId, string NombreCompleto, TipoDocumentoCliente TipoDocumento, string NumeroDocumento, string Direccion, string Telefono, EstadoRegistro Estado);

public class ClienteEditDto
{
    public int ClienteId { get; set; }
    public TipoDocumentoCliente TipoDocumento { get; set; } = TipoDocumentoCliente.DNI;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}
