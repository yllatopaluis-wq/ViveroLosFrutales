using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class CompraService(
    ICompraRepository repository,
    IProveedorRepository proveedorRepository,
    IProductoRepository productoRepository,
    IMovimientoCajaRepository movimientoCajaRepository,
    IPagoProveedorRepository pagoProveedorRepository,
    IPagoProveedorAplicacionRepository pagoProveedorAplicacionRepository,
    IOrdenCompraRepository ordenCompraRepository,
    DevolucionService devolucionService,
    CuentaFinancieraService cuentaFinancieraService,
    IFormularioConfiguracionService formularioConfiguracionService,
    IEmpresaContext empresaContext)
{
    private static readonly HashSet<int> DiasCreditoPermitidos = new() { 7, 15, 30, 60, 90 };

    public Task<PagedResult<CompraListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<IReadOnlyList<CompraListDto>> CuentasPorPagarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarCuentasPorPagarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<PagedResult<PagoProveedorTesoreriaListDto>> BuscarPagosProveedorAsync(SearchRequest request, CancellationToken cancellationToken) =>
        pagoProveedorRepository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public async Task<CompraFormDataDto> NuevoAsync(CancellationToken cancellationToken) => new()
    {
        Proveedores = await proveedorRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken),
        Productos = await productoRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken),
        CuentasFinancieras = await cuentaFinancieraService.ListarActivasAsync(cancellationToken),
        Compra = new CompraEditDto { Fecha = PeruDateTime.Today, FechaVencimiento = PeruDateTime.Today, TipoDocumento = TipoDocumentoCompra.PENDIENTE_COMPROBANTE, FormaPago = FormaPagoCompra.CONTADO, EstadoEntrega = EstadoEntregaCompra.PENDIENTE },
        FormularioConfiguracion = await formularioConfiguracionService.ObtenerConfiguracionAsync(FormularioConfiguracionService.TipoCompra, empresaContext.EmpresaId, null, cancellationToken)
    };


    public async Task<CompraFormDataDto> ObtenerParaEditarAsync(int id, CancellationToken cancellationToken)
    {
        var compra = await ObtenerEntidadAsync(id, cancellationToken);
        if (compra.EstadoDocumento == EstadoDocumentoCompra.ANULADO || compra.Estado == EstadoRegistro.Anulado)
        {
            throw new InvalidOperationException("No se puede editar una compra anulada.");
        }

        return new CompraFormDataDto
        {
            Proveedores = await proveedorRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken),
            Productos = await productoRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken),
            CuentasFinancieras = await cuentaFinancieraService.ListarActivasAsync(cancellationToken),
            FormularioConfiguracion = await formularioConfiguracionService.ObtenerConfiguracionAsync(FormularioConfiguracionService.TipoCompra, empresaContext.EmpresaId, null, cancellationToken),
            Compra = ToEditDto(compra)
        };
    }
    public async Task<CompraCamposEditablesDto> ObtenerCamposEditablesAsync(int id, CancellationToken cancellationToken)
    {
        var compra = await ObtenerEntidadAsync(id, cancellationToken);
        if (compra.EstadoDocumento == EstadoDocumentoCompra.ANULADO || compra.Estado == EstadoRegistro.Anulado)
        {
            throw new InvalidOperationException("No se puede editar una compra anulada.");
        }

        return new CompraCamposEditablesDto
        {
            CompraId = compra.CompraId,
            Proveedor = compra.Proveedor?.RazonSocial ?? string.Empty,
            Fecha = compra.Fecha,
            Total = compra.Total,
            TipoDocumento = compra.TipoDocumento,
            Serie = compra.Serie,
            Numero = compra.Numero,
            FormaPago = compra.FormaPago,
            DiasCredito = compra.DiasCredito,
            FechaVencimiento = compra.FechaVencimiento,
            EstadoEntrega = compra.EstadoEntrega,
            Detalle = ToDetalleDto(compra)
        };
    }

    public async Task ActualizarCamposEditablesAsync(CompraCamposEditablesDto dto, CancellationToken cancellationToken)
    {
        dto.Serie = dto.Serie?.Trim().ToUpperInvariant() ?? string.Empty;
        dto.Numero = dto.Numero?.Trim() ?? string.Empty;
        AplicarReglaFormaPago(dto);

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var compra = await ObtenerEntidadAsync(dto.CompraId, cancellationToken);
            if (compra.EstadoDocumento == EstadoDocumentoCompra.ANULADO || compra.Estado == EstadoRegistro.Anulado)
            {
                throw new InvalidOperationException("No se puede editar una compra anulada.");
            }

            if (DocumentoRequiereSerieNumero(dto.TipoDocumento))
            {
                if (string.IsNullOrWhiteSpace(dto.Serie) || string.IsNullOrWhiteSpace(dto.Numero))
                {
                    throw new InvalidOperationException("Serie y correlativo son obligatorios para el tipo de documento seleccionado.");
                }
            }
            else
            {
                dto.Serie = string.Empty;
                dto.Numero = string.Empty;
            }

            if (await repository.ExisteDocumentoAsync(compra.EmpresaId, compra.ProveedorId, dto.TipoDocumento, dto.Serie, dto.Numero, compra.CompraId, cancellationToken))
            {
                throw new InvalidOperationException("Ya existe una compra registrada con el mismo documento para este proveedor.");
            }

            compra.TipoDocumento = dto.TipoDocumento;
            compra.Serie = dto.Serie;
            compra.Numero = dto.Numero;
            compra.FormaPago = dto.FormaPago;
            compra.DiasCredito = dto.DiasCredito;
            compra.FechaVencimiento = dto.FechaVencimiento;
            compra.EstadoEntrega = dto.EstadoEntrega;
            compra.Documento = Documento(compra);
            await repository.GuardarAsync(compra, cancellationToken);
        }, cancellationToken);
    }
    public async Task<CompraDetalleViewDto> ObtenerDetalleAsync(int id, CancellationToken cancellationToken)
    {
        var compra = await ObtenerEntidadAsync(id, cancellationToken);
        RecalcularEstadoPagoCompra(compra);
        return ToDetalleDto(compra);
    }

    public async Task GuardarAsync(CompraEditDto dto, CancellationToken cancellationToken)
    {
        dto.Serie = dto.Serie?.Trim() ?? string.Empty;
        dto.Numero = dto.Numero?.Trim() ?? string.Empty;
        dto.Documento = dto.Documento?.Trim() ?? string.Empty;
        dto.MedioPago = string.IsNullOrWhiteSpace(dto.MedioPago) ? "EFECTIVO" : dto.MedioPago.Trim();
        dto.Moneda = string.IsNullOrWhiteSpace(dto.Moneda) ? "Soles" : dto.Moneda.Trim();
        dto.TipoCambio = dto.TipoCambio <= 0 ? 1 : dto.TipoCambio;
        AplicarReglaFormaPago(dto);
        dto.Observacion = dto.Observacion?.Trim() ?? string.Empty;
        dto.Detalles ??= new List<CompraDetalleEditDto>();

        var detalles = dto.Detalles.Where(x => x.ProductoId > 0 || x.Cantidad > 0 || x.CostoUnitario > 0 || x.CantidadRecibida > 0).ToList();
        if (dto.MontoPagoInicial < 0) throw new InvalidOperationException("El pago inicial no puede ser negativo.");
        if (dto.ProveedorId <= 0) throw new InvalidOperationException("Seleccione un proveedor.");
        if (detalles.Count == 0) throw new InvalidOperationException("Ingrese al menos un producto.");
        if (DocumentoRequiereSerieNumero(dto.TipoDocumento))
        {
            if (string.IsNullOrWhiteSpace(dto.Serie) || string.IsNullOrWhiteSpace(dto.Numero))
            {
                throw new InvalidOperationException("Serie y numero son obligatorios para el tipo de documento seleccionado.");
            }
        }
        else
        {
            dto.Serie = string.Empty;
            dto.Numero = string.Empty;
        }

        if (await repository.ExisteDocumentoAsync(empresaContext.EmpresaId, dto.ProveedorId, dto.TipoDocumento, dto.Serie, dto.Numero, dto.CompraId == 0 ? null : dto.CompraId, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe una compra registrada con el mismo documento para este proveedor.");
        }

        var productos = await productoRepository.BuscarPorIdsAsync(empresaContext.EmpresaId, detalles.Select(x => x.ProductoId).Distinct().ToArray(), cancellationToken);
        var productosPorId = productos.ToDictionary(x => x.ProductoId);
        var usaEstadoEntregaParaInventario = await UsaEstadoEntregaParaInventarioAsync(cancellationToken);
        var estadoEntregaInventario = usaEstadoEntregaParaInventario ? dto.EstadoEntrega : EstadoEntregaCompra.RECIBIDO;

        var compra = new Compra
        {
            EmpresaId = empresaContext.EmpresaId,
            ProveedorId = dto.ProveedorId,
            OrdenCompraId = dto.OrdenCompraId,
            TipoDocumento = dto.TipoDocumento,
            Serie = dto.Serie.ToUpperInvariant(),
            Numero = dto.Numero,
            Fecha = dto.Fecha.Date,
            FechaVencimiento = dto.FechaVencimiento!.Value.Date,
            Moneda = dto.Moneda,
            TipoCambio = dto.TipoCambio,
            DiasCredito = dto.DiasCredito,
            FormaPago = dto.FormaPago,
            Observacion = dto.Observacion,
            UsuarioRegistro = empresaContext.UsuarioNombre
        };
        compra.Documento = Documento(compra);

        foreach (var detalle in CrearDetallesCompra(detalles, estadoEntregaInventario, productosPorId))
        {
            compra.Detalles.Add(detalle);
        }

        compra.SubTotal = compra.Detalles.Sum(x => x.Importe);
        compra.Igv = compra.Detalles.Sum(x => x.Igv);
        compra.Total = compra.Detalles.Sum(x => x.TotalLinea);
        if (compra.Total <= 0) throw new InvalidOperationException("El total debe ser mayor a cero.");
        RecalcularEstadoEntregaCompra(compra);

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            OrdenCompra? orden = null;
            if (compra.OrdenCompraId.HasValue)
            {
                orden = await ordenCompraRepository.ObtenerAsync(empresaContext.EmpresaId, compra.OrdenCompraId.Value, cancellationToken)
                    ?? throw new InvalidOperationException("Orden de compra no encontrada.");
                ValidarCompraContraOrden(compra, orden);
            }

            RecalcularEstadoPagoCompra(compra);
            await repository.GuardarAsync(compra, cancellationToken);
            await repository.AumentarStockAsync(compra, cancellationToken);

            if (orden is not null)
            {
                await AplicarPagosAdelantadosAsync(compra, cancellationToken);
            }

            RecalcularEstadoPagoCompra(compra);
            if (dto.MontoPagoInicial > 0)
            {
                await CrearPagoProveedorAsync(compra, dto.MontoPagoInicial, compra.Fecha, dto.MedioPago, dto.CuentaFinancieraId, "Pago inicial de compra", cancellationToken);
            }

            RecalcularEstadoPagoCompra(compra);
            await repository.GuardarAsync(compra, cancellationToken);
            if (orden is not null)
            {
                if (!orden.Compras.Any(x => x.CompraId == compra.CompraId))
                {
                    orden.Compras.Add(compra);
                }

                foreach (var pago in compra.Pagos.Where(x => x.OrdenCompraId == orden.OrdenCompraId && !orden.Pagos.Any(p => p.PagoProveedorId == x.PagoProveedorId)))
                {
                    orden.Pagos.Add(pago);
                }

                OrdenCompraService.RecalcularEstados(orden);
                await ordenCompraRepository.GuardarAsync(orden, cancellationToken);
            }
        }, cancellationToken);
    }


    public async Task ActualizarAsync(CompraEditDto dto, CancellationToken cancellationToken)
    {
        if (dto.CompraId <= 0) throw new InvalidOperationException("Compra no encontrada.");
        dto.Serie = dto.Serie?.Trim() ?? string.Empty;
        dto.Numero = dto.Numero?.Trim() ?? string.Empty;
        dto.Documento = dto.Documento?.Trim() ?? string.Empty;
        dto.Moneda = string.IsNullOrWhiteSpace(dto.Moneda) ? "Soles" : dto.Moneda.Trim();
        dto.TipoCambio = dto.TipoCambio <= 0 ? 1 : dto.TipoCambio;
        dto.Observacion = dto.Observacion?.Trim() ?? string.Empty;
        dto.Detalles ??= new List<CompraDetalleEditDto>();
        AplicarReglaFormaPago(dto);

        var detalles = dto.Detalles.Where(x => x.ProductoId > 0 || x.Cantidad > 0 || x.CostoUnitario > 0 || x.CantidadRecibida > 0).ToList();
        if (dto.ProveedorId <= 0) throw new InvalidOperationException("Seleccione un proveedor.");
        if (detalles.Count == 0) throw new InvalidOperationException("Ingrese al menos un producto.");
        if (DocumentoRequiereSerieNumero(dto.TipoDocumento))
        {
            if (string.IsNullOrWhiteSpace(dto.Serie) || string.IsNullOrWhiteSpace(dto.Numero))
            {
                throw new InvalidOperationException("Serie y numero son obligatorios para el tipo de documento seleccionado.");
            }
        }
        else
        {
            dto.Serie = string.Empty;
            dto.Numero = string.Empty;
        }

        var proveedor = await proveedorRepository.ObtenerAsync(empresaContext.EmpresaId, dto.ProveedorId, cancellationToken)
            ?? throw new InvalidOperationException("Proveedor no encontrado.");
        if (proveedor.Estado != EstadoRegistro.Activo) throw new InvalidOperationException("Seleccione un proveedor activo.");

        if (await repository.ExisteDocumentoAsync(empresaContext.EmpresaId, dto.ProveedorId, dto.TipoDocumento, dto.Serie, dto.Numero, dto.CompraId, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe una compra registrada con el mismo documento para este proveedor.");
        }

        var productos = await productoRepository.BuscarPorIdsAsync(empresaContext.EmpresaId, detalles.Select(x => x.ProductoId).Distinct().ToArray(), cancellationToken);
        var productosPorId = productos.ToDictionary(x => x.ProductoId);
        var usaEstadoEntregaParaInventario = await UsaEstadoEntregaParaInventarioAsync(cancellationToken);
        var estadoEntregaInventario = usaEstadoEntregaParaInventario ? dto.EstadoEntrega : EstadoEntregaCompra.RECIBIDO;
        var nuevosDetalles = CrearDetallesCompra(detalles, estadoEntregaInventario, productosPorId);
        var nuevoSubTotal = nuevosDetalles.Sum(x => x.Importe);
        var nuevoIgv = nuevosDetalles.Sum(x => x.Igv);
        var nuevoTotal = nuevosDetalles.Sum(x => x.TotalLinea);
        if (nuevoTotal <= 0) throw new InvalidOperationException("El total debe ser mayor a cero.");

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var compra = await ObtenerEntidadAsync(dto.CompraId, cancellationToken);
            if (compra.EstadoDocumento == EstadoDocumentoCompra.ANULADO || compra.Estado == EstadoRegistro.Anulado)
            {
                throw new InvalidOperationException("No se puede editar una compra anulada.");
            }

            var tienePagosActivos = compra.Pagos.Any(x => x.EstadoPago == PagoProveedorEstado.ACTIVO);
            if (tienePagosActivos && dto.ProveedorId != compra.ProveedorId)
            {
                throw new InvalidOperationException("No es posible cambiar el proveedor de una compra con pagos asociados. Primero debe anular los pagos y luego modificar el proveedor.");
            }

            var totalAplicado = TotalAplicadoActivo(compra);
            if (nuevoTotal < totalAplicado)
            {
                throw new InvalidOperationException($"No es posible reducir el total de la compra a un importe menor que los pagos ya aplicados (S/ {totalAplicado:N2}). Primero debe anular o ajustar las aplicaciones de pago y luego modificar la compra.");
            }

            OrdenCompra? orden = null;
            compra.ProveedorId = dto.ProveedorId;
            compra.OrdenCompraId = dto.OrdenCompraId;
            compra.TipoDocumento = dto.TipoDocumento;
            compra.Serie = dto.Serie.ToUpperInvariant();
            compra.Numero = dto.Numero;
            compra.Fecha = dto.Fecha.Date;
            compra.FechaVencimiento = dto.FechaVencimiento!.Value.Date;
            compra.Moneda = dto.Moneda;
            compra.TipoCambio = dto.TipoCambio;
            compra.DiasCredito = dto.DiasCredito;
            compra.FormaPago = dto.FormaPago;
            compra.Observacion = dto.Observacion;
            compra.EstadoEntrega = dto.EstadoEntrega;
            compra.Documento = Documento(compra);

            if (compra.OrdenCompraId.HasValue)
            {
                orden = await ordenCompraRepository.ObtenerAsync(empresaContext.EmpresaId, compra.OrdenCompraId.Value, cancellationToken)
                    ?? throw new InvalidOperationException("Orden de compra no encontrada.");
            }

            compra.SubTotal = nuevoSubTotal;
            compra.Igv = nuevoIgv;
            compra.Total = nuevoTotal;
            if (orden is not null) ValidarCompraContraOrden(compra, orden);

            await repository.RevertirStockAsync(compra, cancellationToken);
            await repository.EliminarDetallesAsync(compra, cancellationToken);
            foreach (var detalle in nuevosDetalles)
            {
                compra.Detalles.Add(detalle);
            }

            RecalcularEstadoEntregaCompra(compra);
            RecalcularEstadoPagoCompra(compra);
            await repository.GuardarAsync(compra, cancellationToken);
            await repository.AumentarStockAsync(compra, cancellationToken);
            await repository.GuardarAsync(compra, cancellationToken);

            if (orden is not null)
            {
                OrdenCompraService.RecalcularEstados(orden);
                await ordenCompraRepository.GuardarAsync(orden, cancellationToken);
            }
        }, cancellationToken);
    }
    public async Task<RegistrarPagoProveedorDto> ObtenerFormularioPagoAsync(int compraId, CancellationToken cancellationToken)
    {
        var compra = await ObtenerEntidadAsync(compraId, cancellationToken);
        ValidarPuedePagar(compra);

        return new RegistrarPagoProveedorDto
        {
            CompraId = compra.CompraId,
            Proveedor = compra.Proveedor?.RazonSocial ?? string.Empty,
            DocumentoCompra = Documento(compra),
            FechaCompra = compra.Fecha,
            CondicionPago = CondicionPago(compra),
            EstadoPago = compra.EstadoPago,
            TotalCompra = compra.Total,
            TotalPagado = compra.TotalPagado,
            SaldoPendiente = compra.SaldoPendiente,
            MontoPago = compra.SaldoPendiente,
            FechaPago = PeruDateTime.Today,
            CuentasFinancieras = await cuentaFinancieraService.ListarActivasAsync(cancellationToken)
        };
    }

    public async Task RegistrarPagoProveedorAsync(RegistrarPagoProveedorDto dto, CancellationToken cancellationToken)
    {
        if (dto.MontoPago <= 0) throw new InvalidOperationException("El monto del pago debe ser mayor a cero.");
        dto.MedioPago = dto.MedioPago?.Trim() ?? string.Empty;
        dto.Observacion = dto.Observacion?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(dto.MedioPago)) throw new InvalidOperationException("Ingrese el medio de pago.");

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var compra = await ObtenerEntidadAsync(dto.CompraId, cancellationToken);
            ValidarPuedePagar(compra);
            await CrearPagoProveedorAsync(compra, dto.MontoPago, dto.FechaPago, dto.MedioPago, dto.CuentaFinancieraId, dto.Observacion, cancellationToken);
            RecalcularEstadoPagoCompra(compra);
            await repository.GuardarAsync(compra, cancellationToken);
        }, cancellationToken);
    }

    public async Task AnularPagoProveedorAsync(int pagoProveedorId, string motivo, CancellationToken cancellationToken)
    {
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo)) throw new InvalidOperationException("Ingrese el motivo de anulacion del pago.");

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var pago = await pagoProveedorRepository.ObtenerAsync(empresaContext.EmpresaId, pagoProveedorId, cancellationToken)
                ?? throw new InvalidOperationException("Pago proveedor no encontrado.");
            if (pago.EstadoPago == PagoProveedorEstado.ANULADO) return;

            pago.EstadoPago = PagoProveedorEstado.ANULADO;
            pago.FechaAnulacion = DateTime.UtcNow;
            pago.MotivoAnulacion = motivo;
            pago.UsuarioAnulacion = empresaContext.UsuarioNombre;
            if (pago.Aplicaciones.Any(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO)) throw new InvalidOperationException("No se puede anular un pago con aplicaciones activas.");
            await pagoProveedorRepository.GuardarAsync(pago, cancellationToken);

            var movimiento = await movimientoCajaRepository.ObtenerPorOrigenAsync(empresaContext.EmpresaId, OrigenMovimientoCaja.PAGO_PROVEEDOR, pago.PagoProveedorId, cancellationToken);
            if (movimiento is not null)
            {
                movimiento.Estado = EstadoRegistro.Anulado;
                await movimientoCajaRepository.GuardarAsync(movimiento, cancellationToken);
            }
            if (pago.CompraId.HasValue)
            {
                var compra = await ObtenerEntidadAsync(pago.CompraId.Value, cancellationToken);
                RecalcularEstadoPagoCompra(compra);
                await repository.GuardarAsync(compra, cancellationToken);
            }
        }, cancellationToken);
    }

    public async Task RevertirAplicacionesPagoCompraAsync(int compraId, string motivo, CancellationToken cancellationToken)
    {
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo)) throw new InvalidOperationException("Ingrese el motivo para revertir las aplicaciones de pago.");

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var compra = await ObtenerEntidadAsync(compraId, cancellationToken);
            if (compra.EstadoDocumento == EstadoDocumentoCompra.ANULADO || compra.Estado == EstadoRegistro.Anulado)
            {
                throw new InvalidOperationException("La compra ya esta anulada.");
            }

            RevertirAplicacionesActivas(compra, motivo, empresaContext.UsuarioNombre, DateTime.UtcNow);
            RecalcularEstadoPagoCompra(compra);
            await repository.GuardarAsync(compra, cancellationToken);
        }, cancellationToken);
    }

    public async Task<AnularNotaPedidoResultadoDto> AnularCompraAsync(int compraId, string motivo, CancellationToken cancellationToken)
    {
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo)) throw new InvalidOperationException("Ingrese el motivo de anulacion de la compra.");

        AnularNotaPedidoResultadoDto resultado = new(false, 0, "Compra anulada.");
        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            var compra = await ObtenerEntidadAsync(compraId, cancellationToken);
            ValidarPuedeAnularCompra(compra, tieneMovimientosInventarioActivos: false);

            var montoAplicadoActivo = compra.PagoAplicaciones
                .Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO)
                .Sum(x => x.MontoAplicado);

            await repository.RevertirStockAsync(compra, cancellationToken);
            RevertirAplicacionesActivas(compra, motivo, empresaContext.UsuarioNombre, DateTime.UtcNow);
            RecalcularEstadoPagoCompra(compra);

            var devolucion = await devolucionService.CrearDevolucionPorAnulacionCompraAsync(compra, montoAplicadoActivo, motivo, cancellationToken);

            compra.Estado = EstadoRegistro.Anulado;
            compra.EstadoDocumento = EstadoDocumentoCompra.ANULADO;
            compra.FechaAnulacion = DateTime.UtcNow;
            compra.MotivoAnulacion = motivo;
            compra.UsuarioAnulacion = empresaContext.UsuarioNombre;
            await repository.GuardarAsync(compra, cancellationToken);

            if (devolucion is not null)
            {
                resultado = new(
                    true,
                    devolucion.MontoOriginal,
                    $"Compra anulada. Se revirtio stock, se anularon aplicaciones y se genero una solicitud de devolucion por S/ {devolucion.MontoOriginal:N2}.");
            }
        }, cancellationToken);

        return resultado;
    }
    private async Task CrearPagoProveedorAsync(Compra compra, decimal monto, DateTime fechaPago, string medioPago, int? cuentaFinancieraId, string observacion, CancellationToken cancellationToken)
    {
        medioPago = string.IsNullOrWhiteSpace(medioPago) ? "EFECTIVO" : medioPago.Trim();
        observacion = observacion?.Trim() ?? string.Empty;
        var cuentaId = await cuentaFinancieraService.ResolverCuentaIdAsync(cuentaFinancieraId, cancellationToken);
        var pago = new PagoProveedor
        {
            EmpresaId = empresaContext.EmpresaId,
            ProveedorId = compra.ProveedorId,
            CompraId = compra.CompraId,
            OrdenCompraId = compra.OrdenCompraId,
            FechaPago = fechaPago.Date,
            Monto = decimal.Round(monto, 2),
            MedioPago = medioPago.ToUpperInvariant(),
            CuentaFinancieraId = cuentaId,
            Observacion = observacion,
            UsuarioRegistro = empresaContext.UsuarioNombre
        };

        compra.Pagos.Add(pago);
        await pagoProveedorRepository.GuardarAsync(pago, cancellationToken);
        var montoAplicado = Math.Min(decimal.Round(monto, 2), SaldoPendientePorAplicaciones(compra));
        if (montoAplicado > 0)
        {
            var aplicacion = new PagoProveedorAplicacion
            {
                EmpresaId = empresaContext.EmpresaId,
                PagoProveedorId = pago.PagoProveedorId,
                CompraId = compra.CompraId,
                MontoAplicado = montoAplicado,
                FechaAplicacion = DateTime.UtcNow,
                UsuarioRegistro = empresaContext.UsuarioNombre
            };
            compra.PagoAplicaciones.Add(aplicacion);
            pago.Aplicaciones.Add(aplicacion);
            await pagoProveedorAplicacionRepository.GuardarAsync(aplicacion, cancellationToken);
        }
        await movimientoCajaRepository.GuardarAsync(new MovimientoCaja
        {
            EmpresaId = empresaContext.EmpresaId,
            ProveedorId = compra.ProveedorId,
            CuentaFinancieraId = cuentaId,
            TipoMovimiento = TipoMovimientoCaja.EGRESO,
            Origen = OrigenMovimientoCaja.PAGO_PROVEEDOR,
            OrigenId = pago.PagoProveedorId,
            Fecha = pago.FechaPago,
            Monto = pago.Monto,
            MedioPago = pago.MedioPago,
            Descripcion = string.IsNullOrWhiteSpace(observacion) ? $"Pago proveedor {Documento(compra)}" : observacion,
            UsuarioRegistro = empresaContext.UsuarioNombre
        }, cancellationToken);
    }

    private async Task AplicarPagosAdelantadosAsync(Compra compra, CancellationToken cancellationToken)
    {
        if (!compra.OrdenCompraId.HasValue || compra.EstadoDocumento != EstadoDocumentoCompra.ACTIVO) return;
        var pagos = await pagoProveedorRepository.ListarPorOrdenCompraAsync(empresaContext.EmpresaId, compra.OrdenCompraId.Value, cancellationToken);
        foreach (var pago in pagos.Where(x => x.EstadoPago == PagoProveedorEstado.ACTIVO).OrderBy(x => x.FechaPago).ThenBy(x => x.PagoProveedorId))
        {
            if (pago.EmpresaId != compra.EmpresaId) continue;
            if (pago.ProveedorId != compra.ProveedorId) continue;
            if (pago.OrdenCompraId != compra.OrdenCompraId) continue;

            var saldoCompra = SaldoPendientePorAplicaciones(compra);
            if (saldoCompra <= 0) break;
            var saldoPago = SaldoDisponiblePago(pago);
            if (saldoPago <= 0) continue;
            var monto = Math.Min(saldoPago, saldoCompra);
            await CrearAplicacionPagoProveedorAsync(compra, pago, monto, cancellationToken);
        }
    }

    private async Task CrearAplicacionPagoProveedorAsync(Compra compra, PagoProveedor pago, decimal monto, CancellationToken cancellationToken)
    {
        monto = decimal.Round(monto, 2);
        if (monto <= 0) return;
        if (pago.EmpresaId != compra.EmpresaId) throw new InvalidOperationException("El pago pertenece a otra empresa.");
        if (pago.ProveedorId != compra.ProveedorId) throw new InvalidOperationException("El pago y la compra pertenecen a proveedores diferentes.");
        if (pago.OrdenCompraId != compra.OrdenCompraId) throw new InvalidOperationException("El pago pertenece a otra orden de compra.");
        if (pago.EstadoPago != PagoProveedorEstado.ACTIVO) throw new InvalidOperationException("El pago no esta activo.");
        if (compra.EstadoDocumento != EstadoDocumentoCompra.ACTIVO) throw new InvalidOperationException("La compra no esta activa.");

        var saldoPago = SaldoDisponiblePago(pago);
        var saldoCompra = SaldoPendientePorAplicaciones(compra);
        if (saldoPago <= 0 || saldoCompra <= 0) return;
        if (monto > saldoPago) throw new InvalidOperationException("El monto a aplicar supera el saldo disponible del pago.");
        if (monto > saldoCompra) throw new InvalidOperationException("El monto a aplicar supera el saldo pendiente de la compra.");

        var aplicacion = new PagoProveedorAplicacion
        {
            EmpresaId = compra.EmpresaId,
            PagoProveedorId = pago.PagoProveedorId,
            CompraId = compra.CompraId,
            MontoAplicado = monto,
            FechaAplicacion = DateTime.UtcNow,
            UsuarioRegistro = empresaContext.UsuarioNombre
        };
        compra.PagoAplicaciones.Add(aplicacion);
        pago.Aplicaciones.Add(aplicacion);
        await pagoProveedorAplicacionRepository.GuardarAsync(aplicacion, cancellationToken);
    }

    private async Task<bool> UsaEstadoEntregaParaInventarioAsync(CancellationToken cancellationToken)
    {
        var configuracion = await formularioConfiguracionService.ObtenerConfiguracionAsync(FormularioConfiguracionService.TipoCompra, empresaContext.EmpresaId, null, cancellationToken);
        return configuracion.Campos.Any(x =>
            string.Equals(x.Bloque, "GENERAL", StringComparison.OrdinalIgnoreCase)
            && string.Equals(x.Campo, "EstadoEntrega", StringComparison.OrdinalIgnoreCase)
            && x.Visible);
    }
    private static List<CompraDetalle> CrearDetallesCompra(IReadOnlyCollection<CompraDetalleEditDto> detalles, EstadoEntregaCompra estadoEntrega, IReadOnlyDictionary<int, ProductoListDto> productosPorId)
    {
        var resultado = new List<CompraDetalle>();
        foreach (var item in detalles)
        {
            if (!productosPorId.TryGetValue(item.ProductoId, out var producto)) throw new InvalidOperationException("Seleccione un producto valido.");
            if (item.Cantidad <= 0) throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
            if (item.CostoUnitario < 0) throw new InvalidOperationException("El costo unitario no puede ser negativo.");
            var cantidadRecibida = estadoEntrega switch
            {
                EstadoEntregaCompra.RECIBIDO => item.Cantidad,
                EstadoEntregaCompra.PENDIENTE => 0,
                _ => item.CantidadRecibida
            };
            if (cantidadRecibida < 0) throw new InvalidOperationException("La cantidad recibida no puede ser negativa.");
            if (cantidadRecibida > item.Cantidad) throw new InvalidOperationException("La cantidad recibida no puede superar la cantidad comprada.");

            var totalLinea = decimal.Round(item.Cantidad * item.CostoUnitario, 2);
            var importe = producto.AfectoIgv ? decimal.Round(totalLinea / 1.18m, 2) : totalLinea;
            var igv = producto.AfectoIgv ? decimal.Round(totalLinea - importe, 2) : 0;
            resultado.Add(new CompraDetalle
            {
                ProductoId = item.ProductoId,
                UnidadMedida = string.IsNullOrWhiteSpace(item.UnidadMedida) ? producto.UnidadMedida : item.UnidadMedida.Trim(),
                Cantidad = item.Cantidad,
                CantidadRecibida = cantidadRecibida,
                CostoUnitario = item.CostoUnitario,
                Importe = importe,
                Igv = igv,
                TotalLinea = totalLinea
            });
        }

        return resultado;
    }

    private static decimal TotalAplicadoActivo(Compra compra) =>
        decimal.Round(compra.PagoAplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado), 2);

    private static CompraEditDto ToEditDto(Compra compra) => new()
    {
        CompraId = compra.CompraId,
        ProveedorId = compra.ProveedorId,
        OrdenCompraId = compra.OrdenCompraId,
        TipoDocumento = compra.TipoDocumento,
        Serie = compra.Serie,
        Numero = compra.Numero,
        Documento = compra.Documento,
        Fecha = compra.Fecha,
        FechaVencimiento = compra.FechaVencimiento,
        Moneda = compra.Moneda,
        TipoCambio = compra.TipoCambio,
        DiasCredito = compra.DiasCredito,
        FormaPago = compra.FormaPago,
        EstadoEntrega = compra.EstadoEntrega,
        Observacion = compra.Observacion,
        Detalles = compra.Detalles.Select(x => new CompraDetalleEditDto
        {
            ProductoId = x.ProductoId,
            UnidadMedida = x.UnidadMedida,
            Cantidad = x.Cantidad,
            CantidadRecibida = x.CantidadRecibida,
            CostoUnitario = x.CostoUnitario
        }).DefaultIfEmpty(new CompraDetalleEditDto()).ToList()
    };
    private async Task<Compra> ObtenerEntidadAsync(int id, CancellationToken cancellationToken) =>
        await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Compra no encontrada.");

    private static void ValidarPuedePagar(Compra compra)
    {
        if (compra.EstadoDocumento == EstadoDocumentoCompra.ANULADO) throw new InvalidOperationException("No se puede pagar una compra anulada.");
        if (compra.EstadoPago == EstadoPagoCompra.PAGADO || decimal.Round(compra.SaldoPendiente, 2) <= 0) throw new InvalidOperationException("La compra ya esta pagada.");
    }

    private static bool DocumentoRequiereSerieNumero(TipoDocumentoCompra tipo) =>
        tipo is TipoDocumentoCompra.FACTURA or TipoDocumentoCompra.BOLETA or TipoDocumentoCompra.LIQUIDACION_COMPRA or TipoDocumentoCompra.RECIBO or TipoDocumentoCompra.NOTA_VENTA;

    public static void ValidarPuedeAnularCompra(Compra compra, bool tieneMovimientosInventarioActivos)
    {
        if (compra.EstadoDocumento == EstadoDocumentoCompra.ANULADO || compra.Estado == EstadoRegistro.Anulado)
        {
            throw new InvalidOperationException("La compra ya esta anulada.");
        }
    }
    public static void RevertirAplicacionesActivas(Compra compra, string motivo, string usuario, DateTime fechaAnulacion)
    {
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo)) throw new InvalidOperationException("Ingrese el motivo para revertir las aplicaciones de pago.");

        foreach (var aplicacion in compra.PagoAplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO))
        {
            aplicacion.Estado = EstadoPagoProveedorAplicacion.ANULADO;
            aplicacion.FechaAnulacion = fechaAnulacion;
            aplicacion.UsuarioAnulacion = usuario;
            aplicacion.MotivoAnulacion = motivo;
        }
    }

    public static void RecalcularEstadoPago(Compra compra) => RecalcularEstadoPagoCompra(compra);
    private static void RecalcularEstadoPagoCompra(Compra compra)
    {
        compra.TotalPagado = decimal.Round(compra.PagoAplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado), 2);
        compra.SaldoPendiente = SaldoPendientePorAplicaciones(compra);
        compra.EstadoPago = compra.TotalPagado <= 0
            ? EstadoPagoCompra.PENDIENTE
            : compra.TotalPagado >= compra.Total
                ? EstadoPagoCompra.PAGADO
                : EstadoPagoCompra.PARCIAL;
    }

    private static void RecalcularEstadoEntregaCompra(Compra compra)
    {
        var totalComprado = compra.Detalles.Sum(x => x.Cantidad);
        var totalRecibido = compra.Detalles.Sum(x => x.CantidadRecibida);
        compra.EstadoEntrega = totalRecibido <= 0
            ? EstadoEntregaCompra.PENDIENTE
            : totalRecibido >= totalComprado
                ? EstadoEntregaCompra.RECIBIDO
                : EstadoEntregaCompra.PARCIAL;
    }

    private static decimal SaldoPendientePorAplicaciones(Compra compra) =>
        Math.Max(decimal.Round(compra.Total - compra.PagoAplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado), 2), 0);

    private static string CondicionPago(Compra compra)
    {
        if (compra.FormaPago == FormaPagoCompra.CREDITO)
        {
            return compra.DiasCredito > 0 ? $"Credito {compra.DiasCredito} dias" : "Credito";
        }

        return "Contado";
    }
    private static decimal SaldoDisponiblePago(PagoProveedor pago) =>
        Math.Max(decimal.Round(pago.Monto - pago.Aplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado), 2), 0);

    private static void ValidarCompraContraOrden(Compra compra, OrdenCompra orden)
    {
        if (orden.EmpresaId != compra.EmpresaId) throw new InvalidOperationException("La orden pertenece a otra empresa.");
        if (orden.ProveedorId != compra.ProveedorId) throw new InvalidOperationException("La compra debe usar el mismo proveedor de la orden de compra.");
        if (orden.EstadoDocumento != EstadoDocumentoOrdenCompra.ACTIVO) throw new InvalidOperationException("La orden de compra no esta activa.");

        var totalComprasActivas = decimal.Round(orden.Compras
            .Where(x => x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO && x.CompraId != compra.CompraId)
            .Sum(x => x.Total), 2);
        if (totalComprasActivas + compra.Total > orden.Total)
        {
            throw new InvalidOperationException("El total de compras activas supera el total permitido por la orden de compra.");
        }
    }


    private static void AplicarReglaFormaPago(CompraEditDto dto)
    {
        if (dto.FormaPago == FormaPagoCompra.CREDITO)
        {
            if (dto.DiasCredito <= 0)
            {
                throw new InvalidOperationException("Seleccione los dias de credito.");
            }

            if (!DiasCreditoPermitidos.Contains(dto.DiasCredito))
            {
                throw new InvalidOperationException("Dias de credito debe ser 7, 15, 30, 60 o 90 dias.");
            }

            dto.FechaVencimiento = dto.Fecha.Date.AddDays(dto.DiasCredito);
            return;
        }

        dto.DiasCredito = 0;
        dto.FechaVencimiento = dto.Fecha.Date;
    }

    private static void AplicarReglaFormaPago(CompraCamposEditablesDto dto)
    {
        if (dto.FormaPago == FormaPagoCompra.CREDITO)
        {
            if (dto.DiasCredito <= 0)
            {
                throw new InvalidOperationException("Seleccione los dias de credito.");
            }

            if (!DiasCreditoPermitidos.Contains(dto.DiasCredito))
            {
                throw new InvalidOperationException("Dias de credito debe ser 7, 15, 30, 60 o 90 dias.");
            }

            dto.FechaVencimiento = dto.Fecha.Date.AddDays(dto.DiasCredito);
            return;
        }

        dto.DiasCredito = 0;
        dto.FechaVencimiento = dto.Fecha.Date;
    }
    private static string Documento(Compra compra) =>
        string.IsNullOrWhiteSpace(compra.Serie) || string.IsNullOrWhiteSpace(compra.Numero)
            ? (string.IsNullOrWhiteSpace(compra.Documento) ? TipoDocumentoEtiqueta(compra.TipoDocumento) : compra.Documento)
            : $"{compra.Serie}-{compra.Numero}";

    private static string TipoDocumentoEtiqueta(TipoDocumentoCompra tipo) => tipo switch
    {
        TipoDocumentoCompra.FACTURA => "FACTURA",
        TipoDocumentoCompra.BOLETA => "BOLETA",
        TipoDocumentoCompra.LIQUIDACION_COMPRA => "LIQUIDACION COMPRA",
        TipoDocumentoCompra.RECIBO => "RECIBO",
        TipoDocumentoCompra.NOTA_VENTA => "NOTA VENTA",
        TipoDocumentoCompra.PENDIENTE_COMPROBANTE => "PENDIENTE COMPROBANTE",
        TipoDocumentoCompra.SIN_DOCUMENTO => "SIN DOCUMENTO",
        _ => tipo.ToString()
    };

    private static CompraDetalleViewDto ToDetalleDto(Compra compra) => new()
    {
        CompraId = compra.CompraId,
        Fecha = compra.Fecha,
        Proveedor = compra.Proveedor?.RazonSocial ?? string.Empty,
        TipoDocumento = compra.TipoDocumento,
        Serie = compra.Serie,
        Numero = compra.Numero,
        Documento = Documento(compra),
        FormaPago = compra.FormaPago,
        FechaVencimiento = compra.FechaVencimiento,
        Moneda = compra.Moneda,
        TipoCambio = compra.TipoCambio,
        DiasCredito = compra.DiasCredito,
        Observacion = compra.Observacion,
        SubTotal = compra.SubTotal,
        Igv = compra.Igv,
        Total = compra.Total,
        TotalPagado = compra.TotalPagado,
        SaldoPendiente = compra.SaldoPendiente,
        EstadoPago = compra.EstadoPago,
        EstadoEntrega = compra.EstadoEntrega,
        EstadoDocumento = compra.EstadoDocumento,
        TieneAplicacionesPagoActivas = compra.PagoAplicaciones.Any(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO),
        Detalles = compra.Detalles.Select(x => new CompraDetalleDto(
            x.Producto?.Nombre ?? string.Empty,
            x.UnidadMedida,
            x.Cantidad,
            x.CantidadRecibida,
            x.CantidadPendiente,
            x.CostoUnitario,
            x.Importe,
            x.Igv,
            x.TotalLinea)).ToList(),
        Pagos = compra.Pagos.OrderByDescending(x => x.FechaPago).Select(x => new PagoProveedorListDto(
            x.PagoProveedorId,
            x.FechaPago,
            compra.Proveedor?.RazonSocial ?? string.Empty,
            Documento(compra),
            x.Monto,
            x.MedioPago,
            x.CuentaFinanciera?.Nombre ?? string.Empty,
            x.EstadoPago,
            x.Observacion,
            x.EstadoPago == PagoProveedorEstado.ACTIVO && compra.EstadoDocumento == EstadoDocumentoCompra.ACTIVO)).ToList()
    };
}





