namespace ViveroLosFrutales.Application.DTOs;

public record FormularioConfiguracionDto(
    string TipoDocumento,
    string Nombre,
    int Version,
    IReadOnlyList<FormularioBloqueDto> Bloques,
    IReadOnlyList<FormularioCampoDto> Campos,
    FormularioProductoComportamientoEditDto ProductoComportamiento)
{
    public bool BloqueVisible(string bloque) => Bloques.FirstOrDefault(x => x.Bloque == bloque)?.Visible ?? true;
    public bool CampoVisible(string campo) => Campos.FirstOrDefault(x => x.Campo == campo)?.Visible ?? true;
}

public record FormularioBloqueDto(string Bloque, string Titulo, bool Visible, int Orden, bool Colapsado);
public record FormularioCampoDto(string Bloque, string Campo, string Etiqueta, bool Visible, bool Obligatorio, bool SoloLectura, int Orden, string Ancho, string? ValorDefecto);
public record FormularioCampoCatalogoDto(
    string TipoDocumento,
    string Bloque,
    string Campo,
    string EtiquetaDefault,
    int OrdenDefault,
    bool VisibleDefault,
    bool ObligatorioDefault,
    bool SoloLecturaDefault,
    int AnchoDefault,
    bool Ocultable,
    bool PuedeSerObligatorio,
    bool PuedeSerSoloLectura,
    bool EsCampoSistema,
    string TipoVisual);

public record CondicionComercialPlantillaDto(
    int? CondicionComercialPlantillaId,
    string TipoDocumento,
    string Nombre,
    IReadOnlyList<CondicionComercialItemDto> Items);

public record CondicionComercialItemDto(string Etiqueta, string Texto, int Orden, bool Visible);
public record CotizacionCondicionSnapshotDto(string Etiqueta, string Texto, int Orden);

public record PlantillaDocumentoDto(
    string TipoDocumento,
    string Nombre,
    int Version,
    IReadOnlyList<PlantillaDocumentoBloqueDto> Bloques)
{
    public bool BloqueVisible(string bloque) => Bloques.FirstOrDefault(x => x.Bloque == bloque)?.Visible ?? true;
}

public record PlantillaDocumentoBloqueDto(string Bloque, string Titulo, bool Visible, int Orden);

public class DocumentoConfiguracionEditDto
{
    public string TipoDocumento { get; set; } = "COTIZACION";
    public string Nombre { get; set; } = "Cotizacion";
    public List<FormularioBloqueEditDto> Bloques { get; set; } = new();
    public List<FormularioCampoEditDto> Campos { get; set; } = new();
    public List<CondicionComercialEditDto> Condiciones { get; set; } = new();
    public FormularioProductoComportamientoEditDto ProductoComportamiento { get; set; } = new();
}

public class FormularioBloqueEditDto
{
    public string Bloque { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;
    public int Orden { get; set; }
    public bool Colapsado { get; set; }
}

public class FormularioCampoEditDto
{
    public string Bloque { get; set; } = string.Empty;
    public string Campo { get; set; } = string.Empty;
    public string Etiqueta { get; set; } = string.Empty;
    public bool Visible { get; set; } = true;
    public bool Obligatorio { get; set; }
    public bool SoloLectura { get; set; }
    public int Orden { get; set; }
    public string Ancho { get; set; } = string.Empty;
    public string? ValorDefecto { get; set; }
    public bool Ocultable { get; set; } = true;
    public bool PuedeSerObligatorio { get; set; } = true;
    public bool PuedeSerSoloLectura { get; set; } = true;
    public bool EsCampoSistema { get; set; }
    public string TipoVisual { get; set; } = "Texto";
}

public class FormularioProductoComportamientoEditDto
{
    public bool UnirProductosDuplicados { get; set; } = true;
    public decimal CantidadInicial { get; set; } = 1m;
    public bool PermitirEditarPrecio { get; set; } = true;
    public bool PermitirDescuento { get; set; } = true;
    public bool MostrarStock { get; set; } = true;
    public bool BloquearSinStock { get; set; }
}

public class CondicionComercialEditDto
{
    public string Etiqueta { get; set; } = string.Empty;
    public string Texto { get; set; } = string.Empty;
    public int Orden { get; set; }
    public bool Visible { get; set; } = true;
}










