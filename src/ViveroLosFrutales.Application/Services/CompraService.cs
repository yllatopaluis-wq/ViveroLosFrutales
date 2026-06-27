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
    DevolucionService devolucionService,
    IEmpresaContext empresaContext)
{
    public Task<PagedResult<CompraListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<IReadOnlyList<CompraListDto>> CuentasPorPagarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarCuentasPorPagarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public async Task<CompraFormDataDto> NuevoAsync(CancellationToken cancellationToken) => new()
    {
        Proveedores = await proveedorRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken),
        Productos = await productoRepository.ListarActivosAsync(empresaContext.EmpresaId, cancellationToken),
        Compra = new CompraEditDto { Fecha = PeruDateTime.Today, FormaPago = FormaPagoCompra.CREDITO }
    };

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
        dto.Observacion = dto.Observacion?.Trim() ?? string.Empty;
        dto.Detalles ??= new List<CompraDetalleEditDto>();

        var detalles = dto.Detalles.Where(x => x.ProductoId > 0 || x.Cantidad > 0 || x.CostoUnitario > 0).ToList();
        if (dto.ProveedorId <= 0) throw new InvalidOperationException("Seleccione un proveedor.");
        if (detalles.Count == 0) throw new InvalidOperationException("Ingrese al menos un producto.");
        if (DocumentoRequiereSerieNumero(dto.TipoDocumento))
        {
            if (string.IsNullOrWhiteSpace(dto.Serie) || string.IsNullOrWhiteSpace(dto.Numero))
            {
                throw new InvalidOperationException("Serie y número son obligatorios para el tipo de documento seleccionado.");
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

        var compra = new Compra
        {
            EmpresaId = empresaContext.EmpresaId,
            ProveedorId = dto.ProveedorId,
            TipoDocumento = dto.TipoDocumento,
            Serie = dto.Serie.ToUpperInvariant(),
            Numero = dto.Numero,
            Fecha = dto.Fecha.Date,
            FormaPago = dto.FormaPago,
            Observacion = dto.Observacion,
            UsuarioRegistro = empresaContext.UsuarioNombre
        };
        compra.Documento = Documento(compra);

        foreach (var item in detalles)
        {
            if (!productosPorId.TryGetValue(item.ProductoId, out var producto)) throw new InvalidOperationException("Seleccione un producto valido.");
            if (item.Cantidad <= 0) throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
            if (item.CostoUnitario < 0) throw new InvalidOperationException("El costo unitario no puede ser negativo.");

            var importe = decimal.Round(item.Cantidad * item.CostoUnitario, 2);
            var igv = decimal.Round(importe * 0.18m, 2);
            compra.Detalles.Add(new CompraDetalle
            {
                ProductoId = item.ProductoId,
                UnidadMedida = item.UnidadMedida?.Trim() ?? string.Empty,
                Cantidad = item.Cantidad,
                CostoUnitario = item.CostoUnitario,
                Importe = importe,
                Igv = igv,
                TotalLinea = importe + igv
            });
        }

        compra.SubTotal = compra.Detalles.Sum(x => x.Importe);
        compra.Igv = compra.Detalles.Sum(x => x.Igv);
        compra.Total = compra.Detalles.Sum(x => x.TotalLinea);
        if (compra.Total <= 0) throw new InvalidOperationException("El total debe ser mayor a cero.");

        await repository.EjecutarEnTransaccionAsync(async () =>
        {
            if (compra.FormaPago == FormaPagoCompra.CONTADO)
            {
                compra.TotalPagado = compra.Total;
                compra.SaldoPendiente = 0;
                compra.EstadoPago = EstadoPagoCompra.PAGADO;
            }
            else
            {
                compra.TotalPagado = 0;
                compra.SaldoPendiente = compra.Total;
                compra.EstadoPago = EstadoPagoCompra.PENDIENTE;
            }

            await repository.GuardarAsync(compra, cancellationToken);
            await repository.AumentarStockAsync(compra, cancellationToken);

            if (compra.FormaPago == FormaPagoCompra.CONTADO)
            {
                await CrearPagoProveedorAsync(compra, compra.Total, compra.Fecha, dto.MedioPago, "Compra al contado", cancellationToken);
            }

            await repository.GuardarAsync(compra, cancellationToken);
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
            TotalCompra = compra.Total,
            TotalPagado = compra.TotalPagado,
            SaldoPendiente = compra.SaldoPendiente,
            MontoPago = compra.SaldoPendiente,
            FechaPago = PeruDateTime.Today
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
            if (dto.MontoPago > compra.SaldoPendiente)
            {
                throw new InvalidOperationException("El monto del pago no puede superar el saldo pendiente.");
            }

            await CrearPagoProveedorAsync(compra, dto.MontoPago, dto.FechaPago, dto.MedioPago, dto.Observacion, cancellationToken);
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
            await pagoProveedorRepository.GuardarAsync(pago, cancellationToken);

            var movimiento = await movimientoCajaRepository.ObtenerPorOrigenAsync(empresaContext.EmpresaId, OrigenMovimientoCaja.PAGO_PROVEEDOR, pago.PagoProveedorId, cancellationToken);
            if (movimiento is not null)
            {
                movimiento.Estado = EstadoRegistro.Anulado;
                await movimientoCajaRepository.GuardarAsync(movimiento, cancellationToken);
            }

            var compra = await ObtenerEntidadAsync(pago.CompraId, cancellationToken);
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
            if (compra.EstadoDocumento == EstadoDocumentoCompra.ANULADO)
            {
                throw new InvalidOperationException("La compra ya esta anulada.");
            }

            var totalPagadoActivo = compra.Pagos.Where(x => x.EstadoPago == PagoProveedorEstado.ACTIVO).Sum(x => x.Monto);
            await repository.RevertirStockAsync(compra, cancellationToken);

            compra.EstadoDocumento = EstadoDocumentoCompra.ANULADO;
            compra.FechaAnulacion = DateTime.UtcNow;
            compra.MotivoAnulacion = motivo;
            compra.UsuarioAnulacion = empresaContext.UsuarioNombre;
            await repository.GuardarAsync(compra, cancellationToken);

            var devolucion = await devolucionService.CrearDevolucionPorAnulacionCompraAsync(compra, totalPagadoActivo, motivo, cancellationToken);
            if (devolucion is not null)
            {
                resultado = new AnularNotaPedidoResultadoDto(
                    true,
                    devolucion.MontoOriginal,
                    $"Compra anulada. Esta compra tiene pagos registrados por S/ {devolucion.MontoOriginal:N2}. Los egresos historicos permaneceran registrados para conservar la trazabilidad de caja. Se genero una devolucion pendiente del proveedor.");
            }
        }, cancellationToken);

        return resultado;
    }

    private async Task CrearPagoProveedorAsync(Compra compra, decimal monto, DateTime fechaPago, string medioPago, string observacion, CancellationToken cancellationToken)
    {
        medioPago = string.IsNullOrWhiteSpace(medioPago) ? "EFECTIVO" : medioPago.Trim();
        observacion = observacion?.Trim() ?? string.Empty;
        var pago = new PagoProveedor
        {
            EmpresaId = empresaContext.EmpresaId,
            ProveedorId = compra.ProveedorId,
            CompraId = compra.CompraId,
            FechaPago = fechaPago.Date,
            Monto = decimal.Round(monto, 2),
            MedioPago = medioPago.ToUpperInvariant(),
            Observacion = observacion,
            UsuarioRegistro = empresaContext.UsuarioNombre
        };

        compra.Pagos.Add(pago);
        await pagoProveedorRepository.GuardarAsync(pago, cancellationToken);
        await movimientoCajaRepository.GuardarAsync(new MovimientoCaja
        {
            EmpresaId = empresaContext.EmpresaId,
            ProveedorId = compra.ProveedorId,
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

    private async Task<Compra> ObtenerEntidadAsync(int id, CancellationToken cancellationToken) =>
        await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Compra no encontrada.");

    private static void ValidarPuedePagar(Compra compra)
    {
        if (compra.EstadoDocumento == EstadoDocumentoCompra.ANULADO) throw new InvalidOperationException("No se puede pagar una compra anulada.");
        if (compra.EstadoPago == EstadoPagoCompra.PAGADO || compra.SaldoPendiente <= 0) throw new InvalidOperationException("La compra ya esta pagada.");
    }

    private static bool DocumentoRequiereSerieNumero(TipoDocumentoCompra tipo) =>
        tipo is TipoDocumentoCompra.FACTURA or TipoDocumentoCompra.BOLETA or TipoDocumentoCompra.LIQUIDACION_COMPRA;

    private static void RecalcularEstadoPagoCompra(Compra compra)
    {
        compra.TotalPagado = decimal.Round(compra.Pagos.Where(x => x.EstadoPago == PagoProveedorEstado.ACTIVO).Sum(x => x.Monto), 2);
        compra.SaldoPendiente = compra.Total - compra.TotalPagado;
        if (compra.SaldoPendiente < 0) compra.SaldoPendiente = 0;
        compra.EstadoPago = compra.TotalPagado <= 0
            ? EstadoPagoCompra.PENDIENTE
            : compra.TotalPagado >= compra.Total
                ? EstadoPagoCompra.PAGADO
                : EstadoPagoCompra.PARCIAL;
    }

    private static string Documento(Compra compra) =>
        string.IsNullOrWhiteSpace(compra.Serie) || string.IsNullOrWhiteSpace(compra.Numero)
            ? (string.IsNullOrWhiteSpace(compra.Documento) ? compra.TipoDocumento.ToString() : compra.Documento)
            : $"{compra.Serie}-{compra.Numero}";

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
        Observacion = compra.Observacion,
        SubTotal = compra.SubTotal,
        Igv = compra.Igv,
        Total = compra.Total,
        TotalPagado = compra.TotalPagado,
        SaldoPendiente = compra.SaldoPendiente,
        EstadoPago = compra.EstadoPago,
        EstadoDocumento = compra.EstadoDocumento,
        Detalles = compra.Detalles.Select(x => new CompraDetalleDto(
            x.Producto?.Nombre ?? string.Empty,
            x.UnidadMedida,
            x.Cantidad,
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
            x.EstadoPago,
            x.Observacion,
            x.EstadoPago == PagoProveedorEstado.ACTIVO && compra.EstadoDocumento == EstadoDocumentoCompra.ACTIVO)).ToList()
    };
}
