using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record ConfiguracionEmpresaListDto(int ConfiguracionEmpresaId, string Clave, string Valor, string Descripcion);

public class ConfiguracionEmpresaEditDto
{
    public int ConfiguracionEmpresaId { get; set; }
    public string Clave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class CorrelativoEmpresaDto
{
    public TipoComprobante TipoComprobante { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public int SiguienteCorrelativo { get; set; }
    public string Clave { get; set; } = string.Empty;
}

public class CorrelativosEmpresaDto
{
    public List<CorrelativoEmpresaDto> Items { get; set; } = new();
}
