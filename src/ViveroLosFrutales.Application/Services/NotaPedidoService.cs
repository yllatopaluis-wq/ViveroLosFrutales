using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class NotaPedidoService(
    INotaPedidoRepository notaPedidoRepository,
    IClienteRepository clienteRepository,
    IProductoRepository productoRepository,
    IEmpresaRepository empresaRepository,
    IComprobanteRepository comprobanteRepository,
    IComprobanteCobroAplicadoRepository cobroAplicadoRepository,
    CobroClienteService cobroClienteService,
    DevolucionService devolucionService,
    IPdfService pdfService,
    IEmpresaContext empresaContext)
{
    public Task<PagedResult<NotaPedidoListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        notaPedidoRepository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public async Task<NotaPedidoFormDataDto> NuevoAsync(CancellationToken cancellationToken)
    {
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");

        return await FormDataAsync(new NotaPedidoEditDto
        {
            Serie = empresa.SerieNotaPedido,
            Correlativo = await notaPedidoRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, empresa.SerieNotaPedido, cancellationToken),
            Fecha = DateTime.Today
        }, cancellationToken);
    }

    public async Task<NotaPedidoFormDataDto> FormDataAsync(NotaPedidoEditDto dto, CancellationToken cancellationToken)
    {
        var clienteSeleccionado = dto.ClienteId > 0
            ? await clienteRepository.ObtenerAsync(dto.ClienteId, cancellationToken)
            : null;
        var productos = (await productoRepository.BuscarActivosAsync(empresaContext.EmpresaId, null, 50, cancellationToken)).ToList();
        foreach (var producto in await ProductosSeleccionadosAsync(dto.Detalles.Select(x => x.ProductoId), cancellationToken))
        {
            if (productos.All(x => x.ProductoId != producto.ProductoId)) productos.Add(producto);
        }

        var clientes = (await clienteRepository.BuscarActivosAsync(null, 50, cancellationToken)).ToList();
        if (clienteSeleccionado is not null && clientes.All(x => x.ClienteId != clienteSeleccionado.ClienteId))
        {
            clientes.Add(new ClienteListDto(clienteSeleccionado.ClienteId, clienteSeleccionado.NombreCompleto, clienteSeleccionado.TipoDocumento, clienteSeleccionado.NumeroDocumento, clienteSeleccionado.Direccion, clienteSeleccionado.Telefono, clienteSeleccionado.Estado));
        }

        return new NotaPedidoFormDataDto
        {
            NotaPedido = dto,
            Clientes = clientes
                .OrderBy(x => x.NombreCompleto)
                .Select(x => new ComprobanteClienteOptionDto(x.ClienteId, x.NombreCompleto, x.NumeroDocumento, x.Direccion))
                .ToArray(),
            Productos = productos
                .OrderBy(x => x.Nombre)
                .Select(x => new ComprobanteProductoOptionDto(x.ProductoId, x.Nombre, x.Categoria, x.PrecioVentaConIgv, x.Stock, x.AfectoIgv))
                .ToArray()
        };
    }

    public async Task<IReadOnlyList<ComprobanteClienteOptionDto>> BuscarClientesAsync(string? search, CancellationToken cancellationToken)
    {
        var clientes = await clienteRepository.BuscarActivosAsync(search, 20, cancellationToken);
        return clientes.Select(x => new ComprobanteClienteOptionDto(x.ClienteId, x.NombreCompleto, x.NumeroDocumento, x.Direccion)).ToArray();
    }

    public async Task<IReadOnlyList<ComprobanteProductoOptionDto>> BuscarProductosAsync(string? search, CancellationToken cancellationToken)
    {
        var productos = await productoRepository.BuscarActivosAsync(empresaContext.EmpresaId, search, 20, cancellationToken);
        return productos.Select(x => new ComprobanteProductoOptionDto(x.ProductoId, x.Nombre, x.Categoria, x.PrecioVentaConIgv, x.Stock, x.AfectoIgv)).ToArray();
    }

    private async Task<IReadOnlyList<ProductoListDto>> ProductosSeleccionadosAsync(IEnumerable<int> productoIds, CancellationToken cancellationToken)
    {
        var productos = new List<ProductoListDto>();
        foreach (var productoId in productoIds.Where(x => x > 0).Distinct())
        {
            var producto = await productoRepository.ObtenerAsync(empresaContext.EmpresaId, productoId, cancellationToken);
            if (producto is null) continue;
            productos.Add(new ProductoListDto(producto.ProductoId, producto.Categoria, producto.Nombre, producto.PrecioVentaSinIgv, producto.PrecioVentaConIgv, producto.Stock, producto.AfectoIgv, producto.Estado));
        }

        return productos;
    }

    public async Task<NotaPedidoEditDto> ObtenerParaEditarAsync(int id, CancellationToken cancellationToken)
    {
        var nota = await notaPedidoRepository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Nota de pedido no encontrada.");

        if (nota.EstadoDocumento == NotaPedidoEstado.ANULADO || nota.ComprobanteId is not null)
        {
            throw new InvalidOperationException("La nota de pedido no puede editarse en su estado actual.");
        }

        return new NotaPedidoEditDto
        {
            NotaPedidoId = nota.NotaPedidoId,
            CotizacionId = nota.CotizacionId,
            ClienteId = nota.ClienteId,
            Serie = nota.Serie,
            Correlativo = nota.Correlativo,
            Fecha = nota.Fecha,
            Detalles = nota.Detalles.Select(x => new NotaPedidoDetalleEditDto
            {
                ProductoId = x.ProductoId,
                Cantidad = x.Cantidad,
                PrecioUnitario = x.PrecioUnitario
            }).ToList()
        };
    }

    public async Task<NotaPedidoDetalleViewDto> ObtenerDetalleAsync(int id, CancellationToken cancellationToken)
    {
        var nota = await notaPedidoRepository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Nota de pedido no encontrada.");
        var cobros = await cobroClienteService.ListarPorNotaPedidoAsync(nota.NotaPedidoId, cancellationToken);
        var totalCobrado = cobros.Where(x => x.Estado != CobroClienteEstado.ANULADO).Sum(x => x.Monto);
        var saldoPendiente = SaldoPendiente(nota.Total, totalCobrado);

        return new NotaPedidoDetalleViewDto
        {
            NotaPedidoId = nota.NotaPedidoId,
            Numero = $"{nota.Serie}-{nota.Correlativo:000000}",
            Fecha = nota.Fecha,
            ClienteId = nota.ClienteId,
            Cliente = nota.Cliente?.NombreCompleto ?? string.Empty,
            ClienteTipoDocumento = nota.Cliente?.TipoDocumento ?? TipoDocumentoCliente.DNI,
            Subtotal = nota.Subtotal,
            Igv = nota.Igv,
            Total = nota.Total,
            TotalCobrado = totalCobrado,
            SaldoPendiente = saldoPendiente,
            EstadoDocumento = nota.EstadoDocumento,
            EstadoPago = EstadoPago(totalCobrado, nota.Total),
            ComprobanteId = nota.ComprobanteId,
            Detalles = nota.Detalles.Select(x => new NotaPedidoDetalleDto(
                x.Producto?.Nombre ?? string.Empty,
                x.Cantidad,
                x.PrecioUnitario,
                x.Subtotal,
                x.Igv,
                x.Total)).ToArray(),
            Cobros = cobros,
            Comprobantes = nota.Comprobantes.Select(x => new ComprobanteListDto(
                x.ComprobanteId,
                x.TipoComprobante,
                x.Serie,
                x.Correlativo,
                x.FechaEmision,
                x.Cliente?.NombreCompleto ?? string.Empty,
                x.Cliente?.NumeroDocumento ?? string.Empty,
                x.Direccion,
                x.Total,
                x.TotalPagado,
                x.SaldoPendiente,
                x.EstadoPago,
                x.EstadoSunat,
                x.DocumentoImpreso,
                x.EstadoSunat == EstadoSunat.Aceptado,
                x.Estado,
                false,
                x.Estado == EstadoRegistro.Activo && x.EstadoPago != EstadoPagoComprobante.PAGADO,
                false)).ToArray()
        };
    }

    public async Task GuardarAsync(NotaPedidoEditDto dto, CancellationToken cancellationToken)
    {
        if (dto.ClienteId <= 0) throw new InvalidOperationException("Seleccione un cliente.");
        var detallesValidos = dto.Detalles.Where(x => x.ProductoId > 0 || x.Cantidad > 0 || x.PrecioUnitario > 0).ToArray();
        if (detallesValidos.Length == 0) throw new InvalidOperationException("Ingrese al menos un producto.");

        _ = await clienteRepository.ObtenerAsync(dto.ClienteId, cancellationToken)
            ?? throw new InvalidOperationException("Cliente no encontrado.");

        NotaPedido nota;
        if (dto.NotaPedidoId == 0)
        {
            var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
                ?? throw new InvalidOperationException("Empresa activa no encontrada.");
            nota = new NotaPedido
            {
                EmpresaId = empresaContext.EmpresaId,
                CotizacionId = dto.CotizacionId,
                Serie = empresa.SerieNotaPedido,
                Correlativo = await notaPedidoRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, empresa.SerieNotaPedido, cancellationToken),
                UsuarioRegistro = empresaContext.UsuarioNombre
            };
        }
        else
        {
            nota = await notaPedidoRepository.ObtenerAsync(empresaContext.EmpresaId, dto.NotaPedidoId, cancellationToken)
                ?? throw new InvalidOperationException("Nota de pedido no encontrada.");
            if (nota.EstadoDocumento == NotaPedidoEstado.ANULADO || nota.ComprobanteId is not null)
            {
                throw new InvalidOperationException("La nota de pedido no puede editarse en su estado actual.");
            }

            nota.Detalles.Clear();
            nota.FechaModificacion = DateTime.UtcNow;
            nota.UsuarioModificacion = empresaContext.UsuarioNombre;
        }

        nota.ClienteId = dto.ClienteId;
        nota.CotizacionId = dto.CotizacionId;
        nota.Fecha = dto.Fecha.Date;

        foreach (var item in detallesValidos)
        {
            if (item.ProductoId <= 0) throw new InvalidOperationException("Seleccione un producto en todos los detalles.");
            if (item.Cantidad <= 0) throw new InvalidOperationException("La cantidad debe ser mayor a cero.");
            if (item.PrecioUnitario <= 0) throw new InvalidOperationException("El precio debe ser mayor a cero.");

            var producto = await productoRepository.ObtenerAsync(empresaContext.EmpresaId, item.ProductoId, cancellationToken)
                ?? throw new InvalidOperationException("Producto no encontrado.");
            var totalLinea = decimal.Round(item.Cantidad * item.PrecioUnitario, 2);
            var subtotal = producto.AfectoIgv ? decimal.Round(totalLinea / 1.18m, 2) : totalLinea;
            var igv = producto.AfectoIgv ? decimal.Round(totalLinea - subtotal, 2) : 0;
            nota.Detalles.Add(new NotaPedidoDetalle
            {
                ProductoId = producto.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Subtotal = subtotal,
                Igv = igv,
                Total = totalLinea
            });
        }

        nota.TotalCobrado = nota.Cobros.Where(x => x.Estado != CobroClienteEstado.ANULADO).Sum(x => x.Monto);
        nota.RecalcularTotales();
        nota.EstadoDocumento = NotaPedidoEstado.ACTIVO;

        await notaPedidoRepository.GuardarAsync(nota, cancellationToken);
    }

    public Task RegistrarCobroAsync(RegistrarCobroDto dto, CancellationToken cancellationToken) =>
        cobroClienteService.RegistrarAsync(dto, cancellationToken);

    public async Task<ComprobanteResultadoDto> ImprimirAsync(int id, CancellationToken cancellationToken)
    {
        var nota = await notaPedidoRepository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Nota de pedido no encontrada.");
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");

        ActualizarEstadoPago(nota);

        var comprobanteShape = new Comprobante
        {
            EmpresaId = nota.EmpresaId,
            Empresa = empresa,
            ClienteId = nota.ClienteId,
            Cliente = nota.Cliente,
            NotaPedidoId = nota.NotaPedidoId,
            TipoComprobante = TipoComprobante.NPE,
            Serie = nota.Serie,
            Correlativo = nota.Correlativo,
            FechaEmision = nota.Fecha,
            Direccion = nota.Cliente?.Direccion ?? string.Empty,
            FormaPago = FormaPago.Contado,
            EmpresaRazonSocial = empresa.RazonSocial,
            EmpresaNombreComercial = empresa.NombreComercial,
            EmpresaRuc = empresa.RUC,
            EmpresaDireccion = empresa.Direccion,
            EmpresaTelefono = empresa.Telefono,
            EmpresaEmail = empresa.Email,
            SubTotal = nota.Subtotal,
            Igv = nota.Igv,
            Total = nota.Total,
            TotalPagado = nota.TotalCobrado,
            SaldoPendiente = nota.SaldoPendiente,
            EstadoSunat = EstadoSunat.NoAplica
        };

        foreach (var item in nota.Detalles)
        {
            comprobanteShape.Detalles.Add(new ComprobanteDetalle
            {
                ProductoId = item.ProductoId,
                Producto = item.Producto,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Importe = item.Subtotal,
                ImporteIgv = item.Igv
            });
        }

        var pdfUrl = await pdfService.GenerarComprobanteLocalAsync(comprobanteShape, cancellationToken);
        return new ComprobanteResultadoDto(nota.NotaPedidoId, nota.Serie, nota.Correlativo, nota.Total, pdfUrl, EstadoSunat.NoAplica);
    }

    public async Task<AnularNotaPedidoResultadoDto> AnularAsync(int id, string motivo, CancellationToken cancellationToken)
    {
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo))
        {
            throw new InvalidOperationException("Ingrese el motivo de anulacion de la nota de pedido.");
        }

        var nota = await notaPedidoRepository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Nota de pedido no encontrada.");
        if (nota.EstadoDocumento != NotaPedidoEstado.ACTIVO)
        {
            throw new InvalidOperationException("La nota de pedido no puede anularse en su estado actual.");
        }
        if (nota.ComprobanteId is not null || nota.Comprobantes.Count > 0)
        {
            throw new InvalidOperationException("No se puede anular una nota de pedido con comprobante relacionado.");
        }

        ActualizarEstadoPago(nota);
        var totalCobradoActivo = nota.Cobros.Where(x => x.Estado == CobroClienteEstado.ACTIVO).Sum(x => x.Monto);

        nota.EstadoDocumento = NotaPedidoEstado.ANULADO;
        nota.FechaModificacion = DateTime.UtcNow;
        nota.UsuarioModificacion = empresaContext.UsuarioNombre;
        await notaPedidoRepository.GuardarAsync(nota, cancellationToken);

        var devolucion = await devolucionService.CrearDevolucionPorAnulacionNotaPedidoAsync(nota, totalCobradoActivo, motivo, cancellationToken);
        if (devolucion is not null)
        {
            return new AnularNotaPedidoResultadoDto(
                true,
                devolucion.MontoOriginal,
                "Nota de pedido anulada. Los ingresos historicos permaneceran registrados. Se genero una devolucion pendiente al cliente.");
        }

        return new AnularNotaPedidoResultadoDto(false, 0, "Nota de pedido anulada.");
    }

    public async Task<ComprobanteResultadoDto> ConvertirAsync(int notaPedidoId, CancellationToken cancellationToken)
    {
        var nota = await notaPedidoRepository.ObtenerAsync(empresaContext.EmpresaId, notaPedidoId, cancellationToken)
            ?? throw new InvalidOperationException("Nota de pedido no encontrada.");
        var cliente = nota.Cliente ?? await clienteRepository.ObtenerAsync(nota.ClienteId, cancellationToken)
            ?? throw new InvalidOperationException("Cliente no encontrado.");
        var tipoDestino = cliente.TipoDocumento == TipoDocumentoCliente.RUC ? TipoComprobante.FAC : TipoComprobante.BOL;

        return await ConvertirAsync(notaPedidoId, tipoDestino, 0, "EFECTIVO", cancellationToken);
    }

    private async Task<ComprobanteResultadoDto> ConvertirAsync(int notaPedidoId, TipoComprobante tipoDestino, decimal pagoAdicional, string medioPago, CancellationToken cancellationToken)
    {
        if (tipoDestino is not (TipoComprobante.BOL or TipoComprobante.FAC))
        {
            throw new InvalidOperationException("La nota de pedido solo puede convertirse a boleta o factura.");
        }

        var nota = await notaPedidoRepository.ObtenerAsync(empresaContext.EmpresaId, notaPedidoId, cancellationToken)
            ?? throw new InvalidOperationException("Nota de pedido no encontrada.");
        ActualizarEstadoPago(nota);
        if (nota.EstadoDocumento != NotaPedidoEstado.ACTIVO)
        {
            throw new InvalidOperationException("La nota de pedido no puede convertirse en su estado actual.");
        }
        if (nota.ComprobanteId is not null || nota.Comprobantes.Count > 0)
        {
            throw new InvalidOperationException("La nota de pedido ya tiene un comprobante relacionado.");
        }
        if (nota.EstadoPago != EstadoPagoNotaPedido.PAGADO || nota.SaldoPendiente > 0)
        {
            throw new InvalidOperationException("No es posible convertir la Nota de Pedido porque aún tiene saldo pendiente.");
        }
        if (pagoAdicional > 0)
        {
            throw new InvalidOperationException("No es posible convertir la Nota de Pedido porque aún tiene saldo pendiente.");
        }

        var cliente = nota.Cliente ?? await clienteRepository.ObtenerAsync(nota.ClienteId, cancellationToken)
            ?? throw new InvalidOperationException("Cliente no encontrado.");
        ValidarTipoComprobantePorDocumento(cliente.TipoDocumento, tipoDestino);
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");
        var serie = tipoDestino == TipoComprobante.BOL ? empresa.SerieBoleta : empresa.SerieFactura;

        var comprobante = new Comprobante
        {
            EmpresaId = empresaContext.EmpresaId,
            CotizacionId = nota.CotizacionId,
            NotaPedidoId = nota.NotaPedidoId,
            TipoComprobante = tipoDestino,
            Serie = serie,
            Correlativo = await comprobanteRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, tipoDestino, serie, cancellationToken),
            ClienteId = cliente.ClienteId,
            Cliente = cliente,
            Direccion = cliente.Direccion,
            FechaEmision = DateTime.Today,
            FormaPago = FormaPago.Contado,
            Empresa = empresa,
            EmpresaRazonSocial = empresa.RazonSocial,
            EmpresaNombreComercial = empresa.NombreComercial,
            EmpresaRuc = empresa.RUC,
            EmpresaDireccion = empresa.Direccion,
            EmpresaTelefono = empresa.Telefono,
            EmpresaEmail = empresa.Email,
            EstadoSunat = EstadoSunat.Pendiente,
            UsuarioRegistro = empresaContext.UsuarioNombre
        };

        foreach (var item in nota.Detalles)
        {
            comprobante.Detalles.Add(new ComprobanteDetalle
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Importe = item.Subtotal,
                ImporteIgv = item.Igv
            });
        }

        comprobante.RecalcularTotales();
        var totalCobradoNota = nota.Cobros.Where(x => x.Estado == CobroClienteEstado.ACTIVO).Sum(x => x.Monto);
        if (pagoAdicional > decimal.Round(comprobante.Total - totalCobradoNota, 2))
        {
            throw new InvalidOperationException("El pago adicional no puede superar el saldo pendiente.");
        }

        await comprobanteRepository.EjecutarEnTransaccionAsync(async () =>
        {
            var cobrosActivos = nota.Cobros.Where(x => x.Estado == CobroClienteEstado.ACTIVO).ToArray();
            var totalAplicado = cobrosActivos.Sum(x => x.Monto);
            ActualizarResumenPago(comprobante, totalAplicado);

            await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);

            foreach (var cobro in cobrosActivos)
            {
                cobro.ComprobanteId = comprobante.ComprobanteId;
                await cobroAplicadoRepository.GuardarAsync(new ComprobanteCobroAplicado
                {
                    EmpresaId = empresaContext.EmpresaId,
                    ComprobanteId = comprobante.ComprobanteId,
                    CobroClienteId = cobro.CobroClienteId,
                    MontoAplicado = cobro.Monto,
                    FechaAplicacion = DateTime.UtcNow,
                    UsuarioRegistro = empresaContext.UsuarioNombre
                }, cancellationToken);
            }

            if (pagoAdicional > 0)
            {
                await cobroClienteService.RegistrarAsync(new RegistrarCobroDto
                {
                    ComprobanteId = comprobante.ComprobanteId,
                    FechaCobro = DateTime.Today,
                    Monto = pagoAdicional,
                    MedioPago = medioPago,
                    Observacion = "Pago adicional al emitir comprobante"
                }, cancellationToken);
                totalAplicado += pagoAdicional;
            }

            ActualizarResumenPago(comprobante, totalAplicado);
            await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);

            nota.ComprobanteId = comprobante.ComprobanteId;
            nota.FechaModificacion = DateTime.UtcNow;
            nota.UsuarioModificacion = empresaContext.UsuarioNombre;
            await notaPedidoRepository.GuardarAsync(nota, cancellationToken);
        }, cancellationToken);

        return new ComprobanteResultadoDto(comprobante.ComprobanteId, comprobante.Serie, comprobante.Correlativo, comprobante.Total, comprobante.PdfUrl, comprobante.EstadoSunat);
    }

    private static void ActualizarResumenPago(Comprobante comprobante, decimal totalPagado)
    {
        comprobante.TotalPagado = decimal.Round(totalPagado, 2);
        comprobante.SaldoPendiente = SaldoPendiente(comprobante.Total, comprobante.TotalPagado);
        comprobante.EstadoPago = comprobante.TotalPagado <= 0
            ? EstadoPagoComprobante.PENDIENTE
            : comprobante.SaldoPendiente <= 0
                ? EstadoPagoComprobante.PAGADO
                : EstadoPagoComprobante.PAGO_PARCIAL;
    }

    private static EstadoPagoNotaPedido EstadoPago(decimal totalCobrado, decimal total)
    {
        var saldo = SaldoPendiente(total, totalCobrado);
        return totalCobrado <= 0
            ? EstadoPagoNotaPedido.PENDIENTE
            : saldo <= 0
                ? EstadoPagoNotaPedido.PAGADO
                : EstadoPagoNotaPedido.PAGO_PARCIAL;
    }

    private static void ActualizarEstadoPago(NotaPedido nota)
    {
        nota.TotalCobrado = nota.Cobros.Where(x => x.Estado != CobroClienteEstado.ANULADO).Sum(x => x.Monto);
        nota.SaldoPendiente = SaldoPendiente(nota.Total, nota.TotalCobrado);
        nota.EstadoPago = EstadoPago(nota.TotalCobrado, nota.Total);
    }

    private static void ValidarTipoComprobantePorDocumento(TipoDocumentoCliente tipoDocumento, TipoComprobante tipoComprobante)
    {
        if (tipoDocumento == TipoDocumentoCliente.RUC && tipoComprobante != TipoComprobante.FAC)
        {
            throw new InvalidOperationException("El cliente con RUC solo puede convertirse a Factura.");
        }

        if (tipoDocumento != TipoDocumentoCliente.RUC && tipoComprobante != TipoComprobante.BOL)
        {
            throw new InvalidOperationException("El cliente sin RUC solo puede convertirse a Boleta.");
        }
    }

    private static decimal SaldoPendiente(decimal total, decimal pagado)
    {
        var saldo = total - pagado;
        return decimal.Round(saldo < 0 ? 0 : saldo, 2);
    }
}
