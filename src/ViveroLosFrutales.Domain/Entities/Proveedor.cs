using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class Proveedor : EmpresaEntity
{
    public int ProveedorId { get; set; }
    public TipoDocumentoCliente TipoDocumento { get; set; } = TipoDocumentoCliente.RUC;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Empresa? Empresa { get; set; }
}
