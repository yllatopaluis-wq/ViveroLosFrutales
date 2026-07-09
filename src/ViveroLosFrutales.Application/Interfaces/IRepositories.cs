using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Interfaces;

public interface IEmpresaRepository
{
    Task<PagedResult<EmpresaListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken);
    Task<Empresa?> ObtenerAsync(int id, CancellationToken cancellationToken);
    Task<EmpresaMarcaDto?> ObtenerMarcaActivaAsync(int empresaId, string usuarioId, CancellationToken cancellationToken);
    Task<Empresa?> ObtenerLogoActivaAsync(int empresaId, string usuarioId, CancellationToken cancellationToken);
    Task GuardarAsync(Empresa empresa, CancellationToken cancellationToken);
}

public interface IProductoRepository
{
    Task<PagedResult<ProductoListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductoListDto>> ListarActivosAsync(int empresaId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductoListDto>> BuscarActivosAsync(int empresaId, string? search, int take, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductoListDto>> BuscarPorIdsAsync(int empresaId, IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
    Task<Producto?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task GuardarAsync(Producto producto, CancellationToken cancellationToken);
}

public interface ICategoriaRepository
{
    Task<PagedResult<CategoriaListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoriaListDto>> ListarActivasAsync(int empresaId, CancellationToken cancellationToken);
    Task<Categoria?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task GuardarAsync(Categoria categoria, CancellationToken cancellationToken);
}

public interface IClienteRepository
{
    Task<PagedResult<ClienteListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<ClienteListDto>> ListarActivosAsync(int empresaId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ClienteListDto>> BuscarActivosAsync(int empresaId, string? search, int take, CancellationToken cancellationToken);
    Task<Cliente?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<bool> ExisteDocumentoAsync(int empresaId, TipoDocumentoCliente tipoDocumento, string numeroDocumento, int? excluirClienteId, CancellationToken cancellationToken);
    Task GuardarAsync(Cliente cliente, CancellationToken cancellationToken);
}

public interface IComprobanteRepository
{
    Task<PagedResult<ComprobanteListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<PagedResult<ComprobanteListDto>> BuscarPorTipoAsync(int empresaId, TipoComprobante tipo, SearchRequest request, CancellationToken cancellationToken);
    Task<PagedResult<NotaCreditoOrigenDto>> BuscarOrigenesNotaCreditoAsync(int empresaId, NotaCreditoOrigenSearchRequest request, CancellationToken cancellationToken);
    Task<bool> TieneNotaCreditoActivaAsync(int empresaId, int comprobanteReferenciaId, CancellationToken cancellationToken);
    Task<decimal> TotalNotasCreditoActivasAsync(int empresaId, int comprobanteReferenciaId, CancellationToken cancellationToken);
    Task<Comprobante?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<int> SiguienteCorrelativoAsync(int empresaId, TipoComprobante tipo, string serie, CancellationToken cancellationToken);
    Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken);
    Task GuardarAsync(Comprobante comprobante, CancellationToken cancellationToken);
}

public interface IMotivoNotaCreditoRepository
{
    Task<IReadOnlyList<MotivoNotaCreditoOptionDto>> ListarActivosAsync(CancellationToken cancellationToken);
    Task<MotivoNotaCredito?> ObtenerAsync(int id, CancellationToken cancellationToken);
}

public interface ICotizacionRepository
{
    Task<PagedResult<CotizacionListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<Cotizacion?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<int> SiguienteCorrelativoAsync(int empresaId, string serie, CancellationToken cancellationToken);
    Task<bool> TieneDocumentosRelacionadosAsync(int empresaId, int cotizacionId, CancellationToken cancellationToken);
    Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken);
    Task GuardarAsync(Cotizacion cotizacion, CancellationToken cancellationToken);
}

public interface INubefactOperacionRepository
{
    Task<PagedResult<NubefactOperacionDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<NubefactOperacionDto?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<NubefactOperacionDto>> ListarPorComprobanteAsync(int empresaId, int comprobanteId, CancellationToken cancellationToken);
}

public interface IProveedorRepository
{
    Task<PagedResult<ProveedorListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<Proveedor?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProveedorListDto>> ListarActivosAsync(int empresaId, CancellationToken cancellationToken);
    Task GuardarAsync(Proveedor proveedor, CancellationToken cancellationToken);
}

public interface ICompraRepository
{
    Task<PagedResult<CompraListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<CompraListDto>> BuscarCuentasPorPagarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<Compra?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<bool> ExisteDocumentoAsync(int empresaId, int proveedorId, TipoDocumentoCompra tipoDocumento, string serie, string numero, int? excluirCompraId, CancellationToken cancellationToken);
    Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken);
    Task AumentarStockAsync(Compra compra, CancellationToken cancellationToken);
    Task RevertirStockAsync(Compra compra, CancellationToken cancellationToken);
    Task GuardarAsync(Compra compra, CancellationToken cancellationToken);
}

public interface IPagoProveedorRepository
{
    Task<PagedResult<PagoProveedorTesoreriaListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<PagoProveedor?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task GuardarAsync(PagoProveedor pago, CancellationToken cancellationToken);
}

public interface IGastoRepository
{
    Task<PagedResult<GastoListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoriaGastoOptionDto>> ListarCategoriasAsync(int empresaId, CancellationToken cancellationToken);
    Task<CategoriaGasto?> ObtenerCategoriaAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<Gasto?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken);
    Task GuardarAsync(Gasto gasto, CancellationToken cancellationToken);
}

public interface IIngresoRepository
{
    Task<PagedResult<IngresoListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoriaIngresoOptionDto>> ListarCategoriasAsync(int empresaId, CancellationToken cancellationToken);
    Task<CategoriaIngreso?> ObtenerCategoriaAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<Ingreso?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken);
    Task GuardarAsync(Ingreso ingreso, CancellationToken cancellationToken);
}

public interface ICuentaFinancieraRepository
{
    Task<CuentaFinanciera> EnsureCuentaPrincipalAsync(int empresaId, CancellationToken cancellationToken);
    Task<PagedResult<CuentaFinancieraListDto>> BuscarAsync(int empresaId, SearchRequest request, TipoCuentaFinanciera? tipo, bool? activo, CancellationToken cancellationToken);
    Task<IReadOnlyList<CuentaFinancieraOptionDto>> ListarActivasAsync(int empresaId, CancellationToken cancellationToken);
    Task<CuentaFinanciera?> ObtenerAsync(int empresaId, int cuentaFinancieraId, CancellationToken cancellationToken);
    Task<CuentaFinanciera?> ObtenerActivaAsync(int empresaId, int cuentaFinancieraId, CancellationToken cancellationToken);
    Task<bool> TieneMovimientosAsync(int empresaId, int cuentaFinancieraId, CancellationToken cancellationToken);
    Task GuardarAsync(CuentaFinanciera cuenta, CancellationToken cancellationToken);
    Task<CajaBancosDto> ObtenerCajaBancosAsync(int empresaId, DateTime? fechaDesde, DateTime? fechaHasta, TipoCuentaFinanciera? tipo, string? search, CancellationToken cancellationToken);
}

public interface ITransferenciaFinancieraRepository
{
    Task<PagedResult<TransferenciaListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<TransferenciaFinanciera?> ObtenerAsync(int empresaId, int transferenciaFinancieraId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CuentaFinancieraOptionDto>> ListarCuentasActivasAsync(int empresaId, CancellationToken cancellationToken);
    Task<CuentaFinanciera?> ObtenerCuentaActivaAsync(int empresaId, int cuentaFinancieraId, CancellationToken cancellationToken);
    Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken);
    Task GuardarAsync(TransferenciaFinanciera transferencia, CancellationToken cancellationToken);
    Task GuardarMovimientoAsync(MovimientoCaja movimiento, CancellationToken cancellationToken);
}

public interface IDashboardRepository
{
    Task<DashboardDto> ObtenerAsync(int empresaId, DateTime fechaDesde, DateTime fechaHasta, CancellationToken cancellationToken);
}

public interface IReporteRepository
{
    Task<ReporteGeneralDto> ObtenerGeneralAsync(int empresaId, int anioDesde, int anioHasta, string indicador, CancellationToken cancellationToken);
    Task<ReporteNotasPedidoDto> ObtenerNotasPedidoAsync(int empresaId, ReporteNotasPedidoRequest request, CancellationToken cancellationToken);
    Task<ReporteComprobantesDto> ObtenerComprobantesAsync(int empresaId, ReporteComprobantesRequest request, CancellationToken cancellationToken);
    Task<ReporteMovimientoCajaDto> ObtenerMovimientoCajaAsync(int empresaId, ReporteMovimientoCajaRequest request, CancellationToken cancellationToken);
}

public interface IErrorAplicacionRepository
{
    Task<PagedResult<ErrorAplicacionListDto>> BuscarAsync(int empresaId, ErrorAplicacionSearchDto request, CancellationToken cancellationToken);
    Task<ErrorAplicacion?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task GuardarAsync(ErrorAplicacion error, CancellationToken cancellationToken);
}

public interface INotaPedidoRepository
{
    Task<PagedResult<NotaPedidoListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<NotaPedido?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<int> SiguienteCorrelativoAsync(int empresaId, string serie, CancellationToken cancellationToken);
    Task GuardarAsync(NotaPedido notaPedido, CancellationToken cancellationToken);
}

public interface ICobroClienteRepository
{
    Task<PagedResult<CobroClienteListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<CobroCliente?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CobroClienteListDto>> ListarPorNotaPedidoAsync(int empresaId, int notaPedidoId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CobroClienteListDto>> ListarPorComprobanteAsync(int empresaId, int comprobanteId, CancellationToken cancellationToken);
    Task<IReadOnlyList<CobroClienteListDto>> ListarPorClienteAsync(int empresaId, int clienteId, CancellationToken cancellationToken);
    Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken);
    Task GuardarAsync(CobroCliente cobro, CancellationToken cancellationToken);
}

public interface IMovimientoCajaRepository
{
    Task<CajaIndexDto> BuscarAsync(int empresaId, SearchRequest request, string? medioPago, TipoMovimientoCaja? tipoMovimiento, CancellationToken cancellationToken);
    Task GuardarAsync(MovimientoCaja movimiento, CancellationToken cancellationToken);
    Task<MovimientoCaja?> ObtenerPorOrigenAsync(int empresaId, OrigenMovimientoCaja origen, int origenId, CancellationToken cancellationToken);
}

public interface IComprobanteCobroAplicadoRepository
{
    Task<IReadOnlyList<ComprobanteCobroAplicado>> ListarPorComprobanteAsync(int empresaId, int comprobanteId, CancellationToken cancellationToken);
    Task GuardarAsync(ComprobanteCobroAplicado aplicacion, CancellationToken cancellationToken);
}

public interface IEstadoCuentaClienteRepository
{
    Task<EstadoCuentaClienteDto?> ObtenerAsync(int empresaId, int clienteId, CancellationToken cancellationToken);
}

public interface IDevolucionRepository
{
    Task<PagedResult<DevolucionListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken);
    Task<DevolucionAlertasDto> ObtenerAlertasAsync(int empresaId, int cantidad, CancellationToken cancellationToken);
    Task<Devolucion?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken);
    Task<bool> ExisteActivaPorNotaPedidoAsync(int empresaId, int notaPedidoId, CancellationToken cancellationToken);
    Task<bool> ExisteActivaPorNotaCreditoAsync(int empresaId, int notaCreditoId, CancellationToken cancellationToken);
    Task<bool> ExisteActivaPorCompraAsync(int empresaId, int compraId, CancellationToken cancellationToken);
    Task<bool> ExisteActivaPorComprobanteAsync(int empresaId, int comprobanteId, CancellationToken cancellationToken);
    Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken);
    Task GuardarAsync(Devolucion devolucion, CancellationToken cancellationToken);
}



