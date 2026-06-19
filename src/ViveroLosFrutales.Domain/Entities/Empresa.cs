using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Domain.Entities;

public class Empresa
{
    public int EmpresaId { get; set; }
    public string RUC { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MonedaPredeterminada { get; set; } = "PEN";
    public string UrlNubefact { get; set; } = string.Empty;
    public string TokenNubefact { get; set; } = string.Empty;
    public string LogoPath { get; set; } = string.Empty;
    public byte[]? LogoContenido { get; set; }
    public string LogoContentType { get; set; } = string.Empty;
    public string LogoNombre { get; set; } = string.Empty;
    public string SerieBoleta { get; set; } = "B001";
    public string SerieFactura { get; set; } = "F001";
    public string SerieNotaCredito { get; set; } = "NC001";
    public string SerieNotaCreditoFactura { get; set; } = "F101";
    public string SerieNotaCreditoBoleta { get; set; } = "B101";
    public string SerieNotaPedido { get; set; } = "NP001";
    public string SerieCotizacion { get; set; } = "C001";
    public EstadoRegistro Estado { get; set; } = EstadoRegistro.Activo;

    public ICollection<UsuarioEmpresa> UsuarioEmpresas { get; set; } = new List<UsuarioEmpresa>();
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    public ICollection<Comprobante> Comprobantes { get; set; } = new List<Comprobante>();
    public ICollection<Proveedor> Proveedores { get; set; } = new List<Proveedor>();
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    public ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
    public ICollection<Ingreso> Ingresos { get; set; } = new List<Ingreso>();
}
