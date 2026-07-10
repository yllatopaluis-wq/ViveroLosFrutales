using System.ComponentModel.DataAnnotations;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record ClienteListDto(int ClienteId, string NombreCompleto, TipoDocumentoCliente TipoDocumento, string NumeroDocumento, string Direccion, string Telefono, string Email, EstadoRegistro Estado);

public class ClienteEditDto
{
    public int ClienteId { get; set; }
    public TipoDocumentoCliente TipoDocumento { get; set; } = TipoDocumentoCliente.DNI;

    [Required(ErrorMessage = "El numero de documento es obligatorio.")]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El cliente es obligatorio.")]
    public string NombreCompleto { get; set; } = string.Empty;

    public string? Email { get; set; }

    [Required(ErrorMessage = "La direccion es obligatoria.")]
    public string Direccion { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}

