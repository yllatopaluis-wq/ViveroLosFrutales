using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class OrdenCompraService(
    IOrdenCompraRepository repository,
    IProveedorRepository proveedorRepository,
    IProductoRepository productoRepository,
    IPagoProveedorRepository pagoProveedorRepository,
    IMovimientoCajaRepository movimientoCajaRepository,
    CuentaFinancieraService cuentaFinancieraService,
    DevolucionService devolucionService,
    IFormularioConfiguracionService formularioConfiguracionService,
    IEmpresaContext empresaContext)
{
    private const string SerieDefault = "OC001";

    public Task<PagedResult<OrdenCompraListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public async Task<OrdenCompraFormDataDto> NuevoAsync(CancellationToken cancellationToken) => new()
    {
        Proveedores = await proveedorRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken),
        Productos = await productoRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken),
        OrdenCompra = new OrdenCompraEditDto
        {
            Serie = SerieDefault,
            Correlativo = await repository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, SerieDefault, cancellationToken),
            Fecha = PeruDateTime.Today
        },
        FormularioConfiguracion = await formularioConfiguracionService.ObtenerConfiguracionAsync(FormularioConfiguracionService.TipoOrdenCompra, empresaContext.EmpresaId, null, cancellationToken)
    };

    public async Task GuardarAsync(OrdenCompraEditDto dto, CancellationToken cancellationToken)
    {
        dto.Serie = string.IsNullOrWhiteSpace(dto.Serie) ? SerieDefault : dto.Serie.Trim().ToUpperInvariant();
        dto.Observacion = dto.Observacion?.Trim() ?? string.Empty;
        dto.LugarEntrega = dto.LugarEntrega?.Trim() ?? string.Empty;
        dto.CondicionEntrega = dto.CondicionEntrega?.Trim() ?? string.Empty;
        dto.CondicionPago = dto.CondicionPago?.Trim() ?? string.Empty;
        dto.Garantia = dto.Garantia?.Trim() ?? string.Empty;
        var detalles = (dto.Detalles ?? new List<OrdenCompraDetalleEditDto>()).Where(x => x.ProductoId > 0 || x.Cantidad > 0 || x.CostoUnitario > 0).ToList();
        if (dto.ProveedorId <= 0) throw new InvalidOperationException("Seleccione un proveedor.");
        if (detalles.Count == 0) throw new InvalidOperationException("Ingrese al menos un producto.");

        var proveedor = await proveedorRepository.ObtenerAsync(empresaContext.EmpresaId, dto.ProveedorId, cancellationToken)
            ?? throw new InvalidOperationException("Proveedor no encontrado.");
        if (proveedor.Estado != EstadoRegistro.Activo) throw new InvalidOperationException("Seleccione un proveedor activo.");

        var productos = await productoRepository.BuscarPorIdsAsync(empresaContext.EmpresaId, detalles.Select(x => x.ProductoId).Distinct().ToArray(), cancellationToken);
        var productosPorId = productos.ToDictionary(x => x.ProductoId);

        var orden = new OrdenCompra
        {
            EmpresaId = empresaContext.EmpresaId,
            ProveedorId = proveedor.ProveedorId,
            Serie = dto.Serie,
            Correlativo = dto.Correlativo > 0 ? dto.Correlativo : await repository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, dto.Serie, cancellationToken),
            Fecha = dto.Fecha.Date,
            Moneda = string.IsNullOrWhiteSpace(dto.Moneda) ? "Soles" : dto.Moneda.Trim(),
            FechaEntregaEsperada = dto.FechaEntregaEsperada?.Date,
            LugarEntrega = dto.LugarEntrega,
            FormaPago = dto.FormaPago,
            CondicionPago = dto.CondicionPago,
            PorcentajeAdelanto = dto.PorcentajeAdelanto < 0 ? 0 : dto.PorcentajeAdelanto,
            PlazoDias = dto.PlazoDias < 0 ? 0 : dto.PlazoDias,
            CondicionEntrega = dto.CondicionEntrega,
            Garantia = dto.Garantia,
            Observacion = dto.Observacion,
            ProveedorTipoDocumento = proveedor.TipoDocumento.ToString(),
            ProveedorNumeroDocumento = proveedor.NumeroDocumento,
            ProveedorRazonSocial = proveedor.RazonSocial,
            ProveedorNombreComercial = proveedor.NombreComercial,
            ProveedorDireccion = proveedor.Direccion,
            ProveedorTelefono = proveedor.Telefono,
            ProveedorEmail = proveedor.Email,
            UsuarioRegistro = empresaContext.UsuarioNombre
        };

        var ordenLinea = 10;
        foreach (var item in detalles)
        {
            if (!productosPorId.TryGetValue(item.ProductoId, out var producto)) throw new InvalidOperationException("Seleccione un producto valido.");
            if (item.Cantidad <= 0) throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
            if (item.CostoUnitario < 0) throw new InvalidOperationException("El costo unitario no puede ser negativo.");
            var totalLinea = decimal.Round(item.Cantidad * item.CostoUnitario, 2);
            var subtotal = producto.AfectoIgv ? decimal.Round(totalLinea / 1.18m, 2) : totalLinea;
            var igv = producto.AfectoIgv ? decimal.Round(totalLinea - subtotal, 2) : 0;
            orden.Detalles.Add(new OrdenCompraDetalle
            {
                ProductoId = item.ProductoId,
                UnidadMedida = string.IsNullOrWhiteSpace(item.UnidadMedida) ? producto.UnidadMedida : item.UnidadMedida.Trim(),
                Cantidad = item.Cantidad,
                CostoUnitario = item.CostoUnitario,
                Subtotal = subtotal,
                Igv = igv,
                Total = totalLinea,
                Orden = ordenLinea
            });
            ordenLinea += 10;
        }

        RecalcularImportes(orden);
        await repository.GuardarAsync(orden, cancellationToken);
    }

    public async Task<OrdenCompraDetalleViewDto> ObtenerDetalleAsync(int id, CancellationToken cancellationToken)
    {
        var orden = await ObtenerEntidadAsync(id, cancellationToken);
        RecalcularEstados(orden);
        await repository.GuardarAsync(orden, cancellationToken);
        return ToDetalleDto(orden);
    }

    public async Task<RegistrarPagoOrdenCompraDto> ObtenerFormularioPagoAsync(int id, CancellationToken cancellationToken)
    {
        var orden = await ObtenerEntidadAsync(id, cancellationToken);
        if (orden.EstadoDocumento != EstadoDocumentoOrdenCompra.ACTIVO) throw new InvalidOperationException("La orden no permite registrar pagos.");
        RecalcularEstados(orden);
        return new RegistrarPagoOrdenCompraDto
        {
            OrdenCompraId = orden.OrdenCompraId,
            OrdenCompra = Documento(orden),
            Proveedor = ProveedorNombre(orden),
            TotalOrden = orden.Total,
            TotalPagado = orden.TotalPagado,
            TotalAplicado = orden.TotalAplicado,
            SaldoDisponible = orden.SaldoDisponible,
            FechaPago = PeruDateTime.Today,
            CuentasFinancieras = await cuentaFinancieraService.ListarActivasAsync(cancellationToken)
        };
    }

    public async Task RegistrarPagoAsync(RegistrarPagoOrdenCompraDto dto, CancellationToken cancellationToken)
    {
        if (dto.MontoPago <= 0) throw new InvalidOperationException("El monto del pago debe ser mayor a cero.");
        dto.MedioPago = string.IsNullOrWhiteSpace(dto.MedioPago) ? "EFECTIVO" : dto.MedioPago.Trim();

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var orden = await ObtenerEntidadAsync(dto.OrdenCompraId, cancellationToken);
            if (orden.EstadoDocumento != EstadoDocumentoOrdenCompra.ACTIVO) throw new InvalidOperationException("La orden no permite registrar pagos.");
            var cuentaId = await cuentaFinancieraService.ResolverCuentaIdAsync(dto.CuentaFinancieraId, cancellationToken);
            var pago = new PagoProveedor
            {
                EmpresaId = empresaContext.EmpresaId,
                ProveedorId = orden.ProveedorId,
                OrdenCompraId = orden.OrdenCompraId,
                FechaPago = dto.FechaPago.Date,
                Monto = decimal.Round(dto.MontoPago, 2),
                MedioPago = dto.MedioPago.ToUpperInvariant(),
                CuentaFinancieraId = cuentaId,
                Observacion = string.IsNullOrWhiteSpace(dto.Observacion) ? $"Pago adelantado {Documento(orden)}" : dto.Observacion.Trim(),
                UsuarioRegistro = empresaContext.UsuarioNombre
            };
            await pagoProveedorRepository.GuardarAsync(pago, cancellationToken);
            await movimientoCajaRepository.GuardarAsync(new MovimientoCaja
            {
                EmpresaId = empresaContext.EmpresaId,
                ProveedorId = orden.ProveedorId,
                CuentaFinancieraId = cuentaId,
                TipoMovimiento = TipoMovimientoCaja.EGRESO,
                Origen = OrigenMovimientoCaja.PAGO_PROVEEDOR,
                OrigenId = pago.PagoProveedorId,
                Fecha = pago.FechaPago,
                Monto = pago.Monto,
                MedioPago = pago.MedioPago,
                Descripcion = pago.Observacion,
                UsuarioRegistro = empresaContext.UsuarioNombre
            }, cancellationToken);

            orden.Pagos.Add(pago);
            RecalcularEstados(orden);
            await repository.GuardarAsync(orden, cancellationToken);
        }, cancellationToken);
    }

    public async Task<CompraFormDataDto> PrepararCompraAsync(int ordenCompraId, CancellationToken cancellationToken)
    {
        var orden = await ObtenerEntidadAsync(ordenCompraId, cancellationToken);
        var data = new CompraFormDataDto
        {
            Proveedores = await proveedorRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken),
            Productos = await productoRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken),
            CuentasFinancieras = await cuentaFinancieraService.ListarActivasAsync(cancellationToken),
            FormularioConfiguracion = await formularioConfiguracionService.ObtenerConfiguracionAsync(FormularioConfiguracionService.TipoCompra, empresaContext.EmpresaId, null, cancellationToken),
            Compra = new CompraEditDto
            {
                OrdenCompraId = orden.OrdenCompraId,
                ProveedorId = orden.ProveedorId,
                Fecha = PeruDateTime.Today,
                FormaPago = orden.FormaPago,
                Observacion = orden.Observacion,
                Detalles = orden.Detalles.Select(x => new CompraDetalleEditDto
                {
                    ProductoId = x.ProductoId,
                    UnidadMedida = x.UnidadMedida,
                    Cantidad = Math.Max(x.Cantidad - x.CantidadFacturada, 0),
                    CostoUnitario = x.CostoUnitario
                }).Where(x => x.Cantidad > 0).DefaultIfEmpty(new CompraDetalleEditDto()).ToList()
            }
        };
        return data;
    }

    public async Task CerrarAsync(int id, bool solicitarDevolucion, string motivo, CancellationToken cancellationToken)
    {
        motivo = string.IsNullOrWhiteSpace(motivo) ? "Cierre de orden de compra" : motivo.Trim();
        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var orden = await ObtenerEntidadAsync(id, cancellationToken);
            RecalcularEstados(orden);
            if (orden.EstadoDocumento != EstadoDocumentoOrdenCompra.ACTIVO) throw new InvalidOperationException("La orden no esta activa.");
            if (solicitarDevolucion && orden.SaldoDisponible > 0)
            {
                await devolucionService.CrearDevolucionPorSaldoFavorOrdenCompraAsync(orden, orden.SaldoDisponible, OrigenDevolucion.CIERRE_ORDEN_CON_SALDO, motivo, cancellationToken);
            }
            orden.EstadoDocumento = EstadoDocumentoOrdenCompra.CERRADO;
            await repository.GuardarAsync(orden, cancellationToken);
        }, cancellationToken);
    }

    public async Task AnularAsync(int id, string motivo, CancellationToken cancellationToken)
    {
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo)) throw new InvalidOperationException("Ingrese el motivo de anulacion.");
        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var orden = await ObtenerEntidadAsync(id, cancellationToken);
            RecalcularEstados(orden);
            if (orden.Compras.Any(x => x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO)) throw new InvalidOperationException("No se puede anular una orden con compras activas.");
            if (orden.Pagos.SelectMany(x => x.Aplicaciones).Any(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO)) throw new InvalidOperationException("No se puede anular una orden con aplicaciones activas.");
            orden.EstadoDocumento = EstadoDocumentoOrdenCompra.ANULADO;
            orden.FechaAnulacion = DateTime.UtcNow;
            orden.MotivoAnulacion = motivo;
            orden.UsuarioAnulacion = empresaContext.UsuarioNombre;
            if (orden.SaldoDisponible > 0)
            {
                await devolucionService.CrearDevolucionPorSaldoFavorOrdenCompraAsync(orden, orden.SaldoDisponible, OrigenDevolucion.ANULACION_ORDEN_COMPRA, motivo, cancellationToken);
            }
            await repository.GuardarAsync(orden, cancellationToken);
        }, cancellationToken);
    }

    private async Task<OrdenCompra> ObtenerEntidadAsync(int id, CancellationToken cancellationToken) =>
        await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Orden de compra no encontrada.");

    internal static void RecalcularImportes(OrdenCompra orden)
    {
        orden.SubTotal = decimal.Round(orden.Detalles.Sum(x => x.Subtotal), 2);
        orden.Igv = decimal.Round(orden.Detalles.Sum(x => x.Igv), 2);
        orden.Total = decimal.Round(orden.Detalles.Sum(x => x.Total), 2);
        orden.PendienteFacturar = orden.Total;
    }

    internal static void RecalcularEstados(OrdenCompra orden)
    {
        orden.TotalFacturado = decimal.Round(orden.Compras.Where(x => x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO).Sum(x => x.Total), 2);
        orden.TotalPagado = decimal.Round(orden.Pagos.Where(x => x.EstadoPago == PagoProveedorEstado.ACTIVO).Sum(x => x.Monto), 2);
        orden.TotalAplicado = decimal.Round(orden.Pagos.SelectMany(x => x.Aplicaciones).Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado), 2);
        orden.TotalDevuelto = decimal.Round(orden.Devoluciones.Where(x => x.EstadoDevolucion != EstadoDevolucion.ANULADO).Sum(x => x.MontoDevuelto), 2);
        var reservado = orden.Devoluciones.Where(x => x.EstadoDevolucion is EstadoDevolucion.PENDIENTE or EstadoDevolucion.PARCIAL).Sum(x => x.MontoPendiente);
        orden.SaldoDisponible = Math.Max(decimal.Round(orden.TotalPagado - orden.TotalAplicado - orden.TotalDevuelto - reservado, 2), 0);
        orden.PendienteFacturar = Math.Max(decimal.Round(orden.Total - orden.TotalFacturado, 2), 0);
        orden.EstadoFacturacion = orden.TotalFacturado <= 0 ? EstadoFacturacionOrdenCompra.NO_FACTURADO
            : orden.TotalFacturado < orden.Total ? EstadoFacturacionOrdenCompra.FACTURADO_PARCIAL
            : orden.TotalFacturado == orden.Total ? EstadoFacturacionOrdenCompra.FACTURADO_TOTAL
            : EstadoFacturacionOrdenCompra.FACTURADO_CON_DIFERENCIA;
        orden.EstadoRecepcion = orden.TotalFacturado <= 0 ? EstadoRecepcionOrdenCompra.NO_RECIBIDO
            : orden.TotalFacturado < orden.Total ? EstadoRecepcionOrdenCompra.RECIBIDO_PARCIAL
            : EstadoRecepcionOrdenCompra.RECIBIDO_TOTAL;
        orden.EstadoFinanciero = orden.TotalPagado <= 0 ? EstadoFinancieroOrdenCompra.SIN_PAGOS
            : orden.SaldoDisponible > 0 ? EstadoFinancieroOrdenCompra.SALDO_A_FAVOR
            : orden.TotalAplicado >= orden.TotalFacturado && orden.TotalFacturado > 0 ? EstadoFinancieroOrdenCompra.PAGADO
            : EstadoFinancieroOrdenCompra.PAGO_PARCIAL;
    }

    private static string Documento(OrdenCompra orden) => $"{orden.Serie}-{orden.Correlativo:000000}";
    private static string ProveedorNombre(OrdenCompra orden) => string.IsNullOrWhiteSpace(orden.ProveedorRazonSocial) ? orden.Proveedor?.RazonSocial ?? string.Empty : orden.ProveedorRazonSocial;

    private static OrdenCompraDetalleViewDto ToDetalleDto(OrdenCompra orden) => new()
    {
        OrdenCompraId = orden.OrdenCompraId,
        Fecha = orden.Fecha,
        Documento = Documento(orden),
        Proveedor = ProveedorNombre(orden),
        ProveedorDocumento = string.IsNullOrWhiteSpace(orden.ProveedorNumeroDocumento) ? orden.Proveedor?.NumeroDocumento ?? string.Empty : orden.ProveedorNumeroDocumento,
        ProveedorDireccion = string.IsNullOrWhiteSpace(orden.ProveedorDireccion) ? orden.Proveedor?.Direccion ?? string.Empty : orden.ProveedorDireccion,
        Moneda = orden.Moneda,
        FechaEntregaEsperada = orden.FechaEntregaEsperada,
        LugarEntrega = orden.LugarEntrega,
        FormaPago = orden.FormaPago,
        CondicionPago = orden.CondicionPago,
        Garantia = orden.Garantia,
        Observacion = orden.Observacion,
        SubTotal = orden.SubTotal,
        Igv = orden.Igv,
        Total = orden.Total,
        TotalFacturado = orden.TotalFacturado,
        TotalPagado = orden.TotalPagado,
        TotalAplicado = orden.TotalAplicado,
        TotalDevuelto = orden.TotalDevuelto,
        SaldoDisponible = orden.SaldoDisponible,
        PendienteFacturar = orden.PendienteFacturar,
        EstadoDocumento = orden.EstadoDocumento,
        EstadoFacturacion = orden.EstadoFacturacion,
        EstadoRecepcion = orden.EstadoRecepcion,
        EstadoFinanciero = orden.EstadoFinanciero,
        Detalles = orden.Detalles.OrderBy(x => x.Orden).Select(x => new OrdenCompraDetalleDto(x.Producto?.Nombre ?? string.Empty, x.UnidadMedida, x.Cantidad, x.CantidadFacturada, x.CantidadRecibida, x.CostoUnitario, x.Subtotal, x.Igv, x.Total)).ToList(),
        Compras = orden.Compras.OrderByDescending(x => x.Fecha).Select(x => new CompraListDto(x.CompraId, x.Fecha, ProveedorNombre(orden), string.IsNullOrWhiteSpace(x.Documento) ? $"{x.Serie}-{x.Numero}" : x.Documento, x.SubTotal, x.Igv, x.Total, x.TotalPagado, x.SaldoPendiente, x.EstadoPago, x.EstadoEntrega, x.EstadoDocumento, x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO && x.SaldoPendiente > 0, x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO)).ToList(),
        Pagos = orden.Pagos.OrderBy(x => x.FechaPago).Select(x => new PagoProveedorOrdenDto(x.PagoProveedorId, x.FechaPago, x.MedioPago, x.CuentaFinanciera?.Nombre ?? "Caja principal", x.Monto, x.Aplicaciones.Where(a => a.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(a => a.MontoAplicado), 0, 0, Math.Max(x.Monto - x.Aplicaciones.Where(a => a.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(a => a.MontoAplicado), 0), x.EstadoPago, x.Observacion)).ToList(),
        Aplicaciones = orden.Pagos.SelectMany(x => x.Aplicaciones).OrderByDescending(x => x.FechaAplicacion).Select(x => new PagoProveedorAplicacionListDto(x.PagoProveedorAplicacionId, x.FechaAplicacion, $"Pago {x.PagoProveedorId}", x.Compra?.Documento ?? string.Empty, x.MontoAplicado, x.Estado)).ToList(),
        Devoluciones = orden.Devoluciones.OrderByDescending(x => x.FechaGeneracion).Select(x => new DevolucionListDto(x.DevolucionId, x.FechaGeneracion, x.TipoTercero, ProveedorNombre(orden), x.Origen, x.Origen.ToString(), Documento(orden), x.MontoOriginal, x.MontoDevuelto, x.MontoPendiente, x.EstadoDevolucion, true, false)).ToList()
    };
}


