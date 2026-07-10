using System.ComponentModel.DataAnnotations;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record ProveedorListDto(int ProveedorId, TipoDocumentoCliente TipoDocumento, string NumeroDocumento, string RazonSocial, string NombreComercial, string Telefono, EstadoRegistro Estado);

public class ProveedorEditDto
{
    public int ProveedorId { get; set; }
    public TipoDocumentoCliente TipoDocumento { get; set; } = TipoDocumentoCliente.RUC;

    [Required(ErrorMessage = "El numero de documento es obligatorio.")]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "La razon social es obligatoria.")]
    public string RazonSocial { get; set; } = string.Empty;

    public string? NombreComercial { get; set; }

    [Required(ErrorMessage = "La direccion es obligatoria.")]
    public string Direccion { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;
}
