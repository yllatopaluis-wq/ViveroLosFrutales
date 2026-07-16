using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class ComprobanteService(
    IComprobanteRepository comprobanteRepository,
    IClienteRepository clienteRepository,
    IProductoRepository productoRepository,
    IEmpresaRepository empresaRepository,
    IMotivoNotaCreditoRepository motivoNotaCreditoRepository,
    INubefactOperacionRepository nubefactOperacionRepository,
    INubefactService nubefactService,
    IPdfService pdfService,
    CobroClienteService cobroClienteService,
    DevolucionService devolucionService,
    CuentaFinancieraService cuentaFinancieraService,
    IFormularioConfiguracionService formularioConfiguracionService,
    IEmpresaContext empresaContext)
{
    public Task<PagedResult<ComprobanteListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        comprobanteRepository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<PagedResult<ComprobanteListDto>> BuscarCotizacionesAsync(SearchRequest request, CancellationToken cancellationToken) =>
        comprobanteRepository.BuscarPorTipoAsync(empresaContext.EmpresaId, TipoComprobante.COT, request, cancellationToken);

    public Task<PagedResult<ComprobanteListDto>> BuscarNotasCreditoAsync(SearchRequest request, CancellationToken cancellationToken) =>
        comprobanteRepository.BuscarPorTipoAsync(empresaContext.EmpresaId, TipoComprobante.NCR, request, cancellationToken);

    public Task<PagedResult<NotaCreditoOrigenDto>> BuscarOrigenesNotaCreditoAsync(NotaCreditoOrigenSearchRequest request, CancellationToken cancellationToken) =>
        comprobanteRepository.BuscarOrigenesNotaCreditoAsync(empresaContext.EmpresaId, request, cancellationToken);

    public Task<FormularioConfiguracionDto> ObtenerFormularioNotaCreditoAsync(CancellationToken cancellationToken) =>
        formularioConfiguracionService.ObtenerConfiguracionAsync(FormularioConfiguracionService.TipoNotaCredito, empresaContext.EmpresaId, null, cancellationToken);

    public async Task<ComprobanteResultadoDto> ImprimirCotizacionAsync(int cotizacionId, CancellationToken cancellationToken)
    {
        var cotizacion = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, cotizacionId, cancellationToken)
            ?? throw new InvalidOperationException("Cotizacion no encontrada.");

        if (cotizacion.TipoComprobante != TipoComprobante.COT)
        {
            throw new InvalidOperationException("Solo se pueden imprimir cotizaciones desde esta opcion.");
        }

        if (string.IsNullOrWhiteSpace(cotizacion.PdfUrl))
        {
            cotizacion.PdfUrl = await pdfService.GenerarComprobanteLocalAsync(cotizacion, cancellationToken);
            await comprobanteRepository.GuardarAsync(cotizacion, cancellationToken);
        }

        return new ComprobanteResultadoDto(cotizacion.ComprobanteId, cotizacion.Serie, cotizacion.Correlativo, cotizacion.Total, cotizacion.PdfUrl, cotizacion.EstadoSunat);
    }

    public async Task<ComprobanteEditDto> CrearFormularioInicialAsync(TipoComprobante tipoComprobante, CancellationToken cancellationToken)
    {
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");

        return new ComprobanteEditDto
        {
            TipoComprobante = tipoComprobante,
            FechaEmision = PeruDateTime.Today,
            EmpresaRazonSocial = empresa.RazonSocial,
            EmpresaNombreComercial = empresa.NombreComercial,
            EmpresaRuc = empresa.RUC,
            EmpresaDireccion = empresa.Direccion,
            EmpresaTelefono = empresa.Telefono,
            EmpresaEmail = empresa.Email,
            CondicionesVenta = "â€¢ Plazo de entrega: Entrega programada a los 6 meses despuÃ©s de aceptaciÃ³n de cotizaciÃ³n.\nâ€¢ Lugar de entrega: En las instalaciones del Vivero (Huaral)\nâ€¢ Forma de pago: Contado.\nâ€¢ GarantÃ­a: 1 meses\nâ€¢ Medios de pago: En efectivo, depÃ³sito a cuenta corriente",
            CaracteristicasTecnicas = "â€¢ Semillas vegetativas seleccionadas de campos certificados con control fitosanitario.\nâ€¢ Edad: Plantas de 5 meses.\nâ€¢ TamaÃ±o de 50 â€“ 70 cm\nâ€¢ Bolsas medidas de 7 x 12\nâ€¢ Peso aprox. por planta 3 kilos"
        };
    }

    public async Task<ComprobanteFormDataDto> ObtenerFormularioAsync(ComprobanteEditDto dto, CancellationToken cancellationToken)
    {
        var numeracion = dto.ComprobanteId == 0
            ? await ObtenerNumeracionAsync(dto.TipoComprobante, cancellationToken)
            : new ComprobanteNumeracionDto(dto.Serie, dto.Correlativo);
        var clienteSeleccionado = dto.ClienteId > 0
            ? await clienteRepository.ObtenerAsync(empresaContext.EmpresaId, dto.ClienteId, cancellationToken)
            : null;
        var productos = (await productoRepository.BuscarActivosAsync(empresaContext.EmpresaId, null, 50, cancellationToken)).ToList();
        foreach (var producto in await ProductosSeleccionadosAsync(dto.Detalles.Select(x => x.ProductoId), cancellationToken))
        {
            if (productos.All(x => x.ProductoId != producto.ProductoId)) productos.Add(producto);
        }

        var clientes = (await clienteRepository.BuscarActivosAsync(empresaContext.EmpresaId, null, 50, cancellationToken)).ToList();
        if (clienteSeleccionado is not null && clientes.All(x => x.ClienteId != clienteSeleccionado.ClienteId))
        {
            clientes.Add(new ClienteListDto(clienteSeleccionado.ClienteId, clienteSeleccionado.NombreCompleto, clienteSeleccionado.TipoDocumento, clienteSeleccionado.NumeroDocumento, clienteSeleccionado.Direccion, clienteSeleccionado.Telefono, clienteSeleccionado.Email, clienteSeleccionado.Estado));
        }

        return new ComprobanteFormDataDto(
            dto,
            numeracion,
            clientes
                .OrderBy(x => x.NombreCompleto)
                .Select(x => new ComprobanteClienteOptionDto(x.ClienteId, x.NombreCompleto, x.NumeroDocumento, x.Direccion, x.Telefono, x.Email))
                .ToArray(),
            productos
                .OrderBy(x => x.Nombre)
                .Select(x => new ComprobanteProductoOptionDto(x.ProductoId, x.Nombre, x.Categoria, x.UnidadMedida, x.PrecioVentaConIgv, x.Stock, x.AfectoIgv))
                .ToArray(),
            await cuentaFinancieraService.ListarActivasAsync(cancellationToken),
            await formularioConfiguracionService.ObtenerConfiguracionAsync(FormularioConfiguracionService.TipoComprobante, empresaContext.EmpresaId, null, cancellationToken));
    }

    public async Task<ComprobanteFormDataDto> ObtenerFormularioLecturaAsync(ComprobanteEditDto dto, CancellationToken cancellationToken)
    {
        var clienteSeleccionado = dto.ClienteId > 0
            ? await clienteRepository.ObtenerAsync(empresaContext.EmpresaId, dto.ClienteId, cancellationToken)
            : null;
        var clientes = clienteSeleccionado is null
            ? Array.Empty<ComprobanteClienteOptionDto>()
            : new[]
            {
                new ComprobanteClienteOptionDto(
                    clienteSeleccionado.ClienteId,
                    string.IsNullOrWhiteSpace(dto.ClienteNombre) ? clienteSeleccionado.NombreCompleto : dto.ClienteNombre,
                    string.IsNullOrWhiteSpace(dto.ClienteNumeroDocumento) ? clienteSeleccionado.NumeroDocumento : dto.ClienteNumeroDocumento,
                    string.IsNullOrWhiteSpace(dto.ClienteDireccion) ? clienteSeleccionado.Direccion : dto.ClienteDireccion,
                    string.IsNullOrWhiteSpace(dto.ClienteTelefono) ? clienteSeleccionado.Telefono : dto.ClienteTelefono,
                    string.IsNullOrWhiteSpace(dto.ClienteEmail) ? clienteSeleccionado.Email : dto.ClienteEmail)
            };
        var productos = (await ProductosSeleccionadosAsync(dto.Detalles.Select(x => x.ProductoId), cancellationToken))
            .OrderBy(x => x.Nombre)
            .Select(x => new ComprobanteProductoOptionDto(x.ProductoId, x.Nombre, x.Categoria, x.UnidadMedida, x.PrecioVentaConIgv, x.Stock, x.AfectoIgv))
            .ToArray();

        return new ComprobanteFormDataDto(
            dto,
            new ComprobanteNumeracionDto(dto.Serie, dto.Correlativo),
            clientes,
            productos,
            await cuentaFinancieraService.ListarActivasAsync(cancellationToken),
            await formularioConfiguracionService.ObtenerConfiguracionAsync(FormularioConfiguracionService.TipoComprobante, empresaContext.EmpresaId, null, cancellationToken));
    }

    public async Task<IReadOnlyList<ComprobanteClienteOptionDto>> BuscarClientesAsync(string? search, CancellationToken cancellationToken)
    {
        var clientes = await clienteRepository.BuscarActivosAsync(empresaContext.EmpresaId, search, 20, cancellationToken);
        return clientes.Select(x => new ComprobanteClienteOptionDto(x.ClienteId, x.NombreCompleto, x.NumeroDocumento, x.Direccion, x.Telefono, x.Email)).ToArray();
    }

    public async Task<IReadOnlyList<ComprobanteProductoOptionDto>> BuscarProductosAsync(string? search, CancellationToken cancellationToken)
    {
        var productos = await productoRepository.BuscarActivosAsync(empresaContext.EmpresaId, search, 20, cancellationToken);
        return productos.Select(x => new ComprobanteProductoOptionDto(x.ProductoId, x.Nombre, x.Categoria, x.UnidadMedida, x.PrecioVentaConIgv, x.Stock, x.AfectoIgv)).ToArray();
    }

    private async Task<IReadOnlyList<ProductoListDto>> ProductosSeleccionadosAsync(IEnumerable<int> productoIds, CancellationToken cancellationToken)
    {
        return await productoRepository.BuscarPorIdsAsync(
            empresaContext.EmpresaId,
            productoIds.Where(x => x > 0).Distinct().ToArray(),
            cancellationToken);
    }

    public async Task<ComprobanteEditDto> ObtenerCotizacionParaEditarAsync(int cotizacionId, CancellationToken cancellationToken)
    {
        var cotizacion = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, cotizacionId, cancellationToken)
            ?? throw new InvalidOperationException("Cotizacion no encontrada.");

        if (cotizacion.TipoComprobante != TipoComprobante.COT)
        {
            throw new InvalidOperationException("Solo se pueden editar cotizaciones desde esta opcion.");
        }

        return new ComprobanteEditDto
        {
            ComprobanteId = cotizacion.ComprobanteId,
            TipoComprobante = cotizacion.TipoComprobante,
            Serie = cotizacion.Serie,
            Correlativo = cotizacion.Correlativo,
            ClienteId = cotizacion.ClienteId,
            CotizacionId = cotizacion.CotizacionId,
            NotaPedidoId = cotizacion.NotaPedidoId,
            Direccion = cotizacion.Direccion,
            FechaEmision = cotizacion.FechaEmision,
            FormaPago = cotizacion.FormaPago,
            EstadoPago = cotizacion.EstadoPago,
            EmpresaRazonSocial = cotizacion.EmpresaRazonSocial,
            EmpresaNombreComercial = cotizacion.EmpresaNombreComercial,
            EmpresaRuc = cotizacion.EmpresaRuc,
            EmpresaDireccion = cotizacion.EmpresaDireccion,
            EmpresaTelefono = cotizacion.EmpresaTelefono,
            EmpresaEmail = cotizacion.EmpresaEmail,
            CondicionesVenta = cotizacion.CondicionesVenta,
            CaracteristicasTecnicas = cotizacion.CaracteristicasTecnicas,
            Detalles = cotizacion.Detalles
                .Select(x => new ComprobanteDetalleDto
                {
                    ProductoId = x.ProductoId,
                    Cantidad = x.Cantidad,
                    PrecioUnitario = x.PrecioUnitario
                })
                .ToList()
        };
    }

    public async Task<ComprobanteEditDto> ObtenerComprobanteParaVisualizarAsync(int comprobanteId, CancellationToken cancellationToken)
    {
        var comprobante = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken)
            ?? throw new InvalidOperationException("Comprobante no encontrado.");

        return ToEditDto(comprobante);
    }

    public async Task<ComprobanteEditDto> ObtenerComprobanteParaEditarAsync(int comprobanteId, CancellationToken cancellationToken)
    {
        var comprobante = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken)
            ?? throw new InvalidOperationException("Comprobante no encontrado.");

        if (comprobante.TipoComprobante == TipoComprobante.COT)
        {
            throw new InvalidOperationException("Use la opcion Cotizaciones para editar una cotizacion.");
        }

        if (comprobante.Estado == EstadoRegistro.Anulado)
        {
            throw new InvalidOperationException("No se puede editar un comprobante anulado.");
        }

        if (!PuedeEditar(comprobante))
        {
            throw new InvalidOperationException("No se puede editar el comprobante porque ya fue impreso, aceptado por SUNAT o se encuentra anulado.");
        }

        return ToEditDto(comprobante);
    }

    public async Task<IReadOnlyList<NubefactOperacionDto>> ListarOperacionesNubefactAsync(int comprobanteId, CancellationToken cancellationToken)
    {
        _ = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken)
            ?? throw new InvalidOperationException("Comprobante no encontrado.");

        return await nubefactOperacionRepository.ListarPorComprobanteAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken);
    }

    public async Task<ComprobanteNumeracionDto> ObtenerNumeracionAsync(TipoComprobante tipoComprobante, CancellationToken cancellationToken)
    {
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");

        var serie = ObtenerSerie(empresa, tipoComprobante);
        var correlativo = await comprobanteRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, tipoComprobante, serie, cancellationToken);
        return new ComprobanteNumeracionDto(serie, correlativo);
    }

    public async Task<ComprobanteResultadoDto> GuardarAsync(ComprobanteEditDto dto, bool imprimir, CancellationToken cancellationToken)
    {
        if (dto.TipoComprobante is not (TipoComprobante.BOL or TipoComprobante.FAC))
        {
            throw new InvalidOperationException("El formulario de comprobantes solo permite boletas y facturas. Use la opcion especifica para notas de credito.");
        }

        if (dto.ClienteId == 0) throw new InvalidOperationException("El cliente es obligatorio.");
        var detallesValidos = dto.Detalles
            .Where(x => x.ProductoId > 0 && x.Cantidad > 0 && x.PrecioUnitario > 0)
            .ToArray();
        if (detallesValidos.Length == 0) throw new InvalidOperationException("Agregue al menos un producto.");

        var cliente = await clienteRepository.ObtenerAsync(empresaContext.EmpresaId, dto.ClienteId, cancellationToken)
            ?? throw new InvalidOperationException("Cliente no encontrado.");

        ValidarTipoComprobantePorDocumento(cliente.TipoDocumento, dto.TipoComprobante);

        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");

        Comprobante comprobante;
        var esNuevo = dto.ComprobanteId == 0;
        if (dto.ComprobanteId > 0)
        {
            comprobante = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, dto.ComprobanteId, cancellationToken)
                ?? throw new InvalidOperationException("Comprobante no encontrado.");

            if (comprobante.Estado == EstadoRegistro.Anulado)
            {
                throw new InvalidOperationException("No se puede editar un comprobante anulado.");
            }

            if (!PuedeEditar(comprobante))
            {
                throw new InvalidOperationException("No se puede editar el comprobante porque ya fue impreso, aceptado por SUNAT o se encuentra anulado.");
            }

            if (comprobante.TipoComprobante != TipoComprobante.COT)
            {
                foreach (var detalle in comprobante.Detalles)
                {
                    if (detalle.Producto is not null)
                    {
                        detalle.Producto.Stock += detalle.Cantidad;
                    }
                }
            }

            comprobante.Detalles.Clear();
            comprobante.DocumentoImpreso = false;
            comprobante.PdfUrl = string.Empty;
            comprobante.XmlUrl = string.Empty;
            comprobante.NubefactHash = string.Empty;
            comprobante.NubefactRespuesta = string.Empty;
            comprobante.EstadoSunat = comprobante.TipoComprobante is TipoComprobante.BOL or TipoComprobante.FAC or TipoComprobante.NCR ? EstadoSunat.Pendiente : EstadoSunat.NoAplica;
        }
        else
        {
            comprobante = new Comprobante
            {
                EmpresaId = empresaContext.EmpresaId,
                TipoComprobante = dto.TipoComprobante,
                Serie = ObtenerSerie(empresa, dto.TipoComprobante),
                Correlativo = await comprobanteRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, dto.TipoComprobante, ObtenerSerie(empresa, dto.TipoComprobante), cancellationToken),
                EstadoSunat = dto.TipoComprobante is TipoComprobante.BOL or TipoComprobante.FAC or TipoComprobante.NCR ? EstadoSunat.Pendiente : EstadoSunat.NoAplica,
                UsuarioRegistro = empresaContext.UsuarioNombre
            };
        }

        comprobante.Empresa = empresa;
        comprobante.ClienteId = cliente.ClienteId;
        comprobante.Cliente = cliente;
        comprobante.CotizacionId = dto.CotizacionId;
        comprobante.NotaPedidoId = dto.NotaPedidoId;
        comprobante.Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? cliente.Direccion : dto.Direccion.Trim();
        comprobante.AplicarSnapshotCliente(cliente, comprobante.Direccion);
        comprobante.FechaEmision = dto.FechaEmision;
        comprobante.FormaPago = dto.FormaPago;
        comprobante.EmpresaRazonSocial = string.IsNullOrWhiteSpace(dto.EmpresaRazonSocial) ? empresa.RazonSocial : dto.EmpresaRazonSocial.Trim();
        comprobante.EmpresaNombreComercial = string.IsNullOrWhiteSpace(dto.EmpresaNombreComercial) ? empresa.NombreComercial : dto.EmpresaNombreComercial.Trim();
        comprobante.EmpresaRuc = string.IsNullOrWhiteSpace(dto.EmpresaRuc) ? empresa.RUC : dto.EmpresaRuc.Trim();
        comprobante.EmpresaDireccion = string.IsNullOrWhiteSpace(dto.EmpresaDireccion) ? empresa.Direccion : dto.EmpresaDireccion.Trim();
        comprobante.EmpresaTelefono = string.IsNullOrWhiteSpace(dto.EmpresaTelefono) ? empresa.Telefono : dto.EmpresaTelefono.Trim();
        comprobante.EmpresaEmail = string.IsNullOrWhiteSpace(dto.EmpresaEmail) ? empresa.Email : dto.EmpresaEmail.Trim();
        comprobante.CondicionesVenta = dto.CondicionesVenta?.Trim() ?? string.Empty;
        comprobante.CaracteristicasTecnicas = dto.CaracteristicasTecnicas?.Trim() ?? string.Empty;

        foreach (var item in detallesValidos)
        {
            var producto = await productoRepository.ObtenerAsync(empresaContext.EmpresaId, item.ProductoId, cancellationToken)
                ?? throw new InvalidOperationException("Producto no encontrado.");

            if (comprobante.TipoComprobante != TipoComprobante.COT && producto.Stock < item.Cantidad)
            {
                throw new InvalidOperationException($"Stock insuficiente para {producto.Nombre}.");
            }

            var totalLinea = decimal.Round(item.Cantidad * item.PrecioUnitario, 2);
            var importe = producto.AfectoIgv ? decimal.Round(totalLinea / 1.18m, 2) : totalLinea;
            var igv = producto.AfectoIgv ? decimal.Round(totalLinea - importe, 2) : 0;
            var detraccion = producto.TieneDetraccion ? decimal.Round(totalLinea * producto.PorcentajeDetraccion / 100m, 2) : 0;

            if (comprobante.TipoComprobante != TipoComprobante.COT)
            {
                producto.Stock -= item.Cantidad;
            }

            comprobante.Detalles.Add(new ComprobanteDetalle
            {
                ProductoId = producto.ProductoId,
                Producto = producto,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Importe = importe,
                ImporteIgv = igv,
                MontoDetraccion = detraccion
            });
        }

        comprobante.RecalcularTotales();
        await comprobanteRepository.EjecutarEnTransaccionAsync(async () =>
        {
            ActualizarResumenPago(comprobante, TotalPagadoComprobante(comprobante));
            var generaCobroAutomatico = comprobante.TipoComprobante is TipoComprobante.BOL or TipoComprobante.FAC
                && esNuevo
                && comprobante.NotaPedidoId is null
                && comprobante.FormaPago == FormaPago.Contado;

            if (generaCobroAutomatico)
            {
                await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);
                await cobroClienteService.RegistrarAsync(new RegistrarCobroDto
                {
                    ComprobanteId = comprobante.ComprobanteId,
                    FechaCobro = comprobante.FechaEmision,
                    Monto = comprobante.Total,
                    MedioPago = NormalizarMedioPago(dto.MedioPago),
                    CuentaFinancieraId = dto.CuentaFinancieraId,
                    Observacion = "Venta directa al contado"
                }, cancellationToken);
            }
            else
            {
                await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);
            }

            ActualizarResumenPago(comprobante, generaCobroAutomatico ? comprobante.Total : TotalPagadoComprobante(comprobante));
            await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);
        }, cancellationToken);

        if (comprobante.TipoComprobante is TipoComprobante.COT || imprimir)
        {
            var mensaje = string.Empty;
            if (comprobante.TipoComprobante is TipoComprobante.BOL or TipoComprobante.FAC or TipoComprobante.NCR)
            {
                var nubefact = await nubefactService.EmitirComprobanteAsync(comprobante, cancellationToken);
                comprobante.EstadoSunat = nubefact.EstadoSunat;
                comprobante.PdfUrl = nubefact.PdfUrl;
                comprobante.XmlUrl = nubefact.XmlUrl;
                comprobante.NubefactHash = nubefact.Hash;
                comprobante.NubefactRespuesta = nubefact.RespuestaCompleta;
                mensaje = nubefact.Mensaje;
                if (comprobante.EstadoSunat != EstadoSunat.Aceptado && !string.IsNullOrWhiteSpace(comprobante.PdfUrl))
                {
                    mensaje = await ConsultarEstadoSunatPendienteAsync(comprobante, mensaje, cancellationToken);
                }
            }
            else
            {
                comprobante.PdfUrl = await pdfService.GenerarComprobanteLocalAsync(comprobante, cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(comprobante.PdfUrl))
            {
                comprobante.DocumentoImpreso = true;
            }

            await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);
            if (string.IsNullOrWhiteSpace(comprobante.PdfUrl) && !string.IsNullOrWhiteSpace(mensaje))
            {
                throw new InvalidOperationException(mensaje);
            }
        }

        return new ComprobanteResultadoDto(comprobante.ComprobanteId, comprobante.Serie, comprobante.Correlativo, comprobante.Total, comprobante.PdfUrl, comprobante.EstadoSunat);
    }

    public async Task<ComprobanteResultadoDto> ImprimirComprobanteAsync(int comprobanteId, CancellationToken cancellationToken)
    {
        var comprobante = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken)
            ?? throw new InvalidOperationException("Comprobante no encontrado.");

        if (comprobante.TipoComprobante == TipoComprobante.COT)
        {
            throw new InvalidOperationException("Use la opcion Cotizaciones para imprimir una cotizacion.");
        }

        if (comprobante.Estado == EstadoRegistro.Anulado)
        {
            throw new InvalidOperationException("No se puede imprimir un comprobante anulado.");
        }

        if (!string.IsNullOrWhiteSpace(comprobante.PdfUrl))
        {
            var debeGuardar = false;
            if (comprobante.TipoComprobante is TipoComprobante.BOL or TipoComprobante.FAC or TipoComprobante.NCR
                && comprobante.EstadoSunat is not (EstadoSunat.Aceptado or EstadoSunat.Anulado))
            {
                await ConsultarEstadoSunatPendienteAsync(comprobante, string.Empty, cancellationToken);
                debeGuardar = true;
            }

            if (!comprobante.DocumentoImpreso)
            {
                comprobante.DocumentoImpreso = true;
                debeGuardar = true;
            }

            if (debeGuardar)
            {
                await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);
            }

            return new ComprobanteResultadoDto(comprobante.ComprobanteId, comprobante.Serie, comprobante.Correlativo, comprobante.Total, comprobante.PdfUrl, comprobante.EstadoSunat);
        }

        var mensaje = string.Empty;
        if (comprobante.TipoComprobante is TipoComprobante.BOL or TipoComprobante.FAC or TipoComprobante.NCR)
        {
            var nubefact = await nubefactService.EmitirComprobanteAsync(comprobante, cancellationToken);
            comprobante.EstadoSunat = nubefact.EstadoSunat;
            comprobante.PdfUrl = nubefact.PdfUrl;
            comprobante.XmlUrl = nubefact.XmlUrl;
            comprobante.NubefactHash = nubefact.Hash;
            comprobante.NubefactRespuesta = nubefact.RespuestaCompleta;
            mensaje = nubefact.Mensaje;
            if (comprobante.EstadoSunat != EstadoSunat.Aceptado && !string.IsNullOrWhiteSpace(comprobante.PdfUrl))
            {
                mensaje = await ConsultarEstadoSunatPendienteAsync(comprobante, mensaje, cancellationToken);
            }
        }
        else
        {
            comprobante.PdfUrl = await pdfService.GenerarComprobanteLocalAsync(comprobante, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(comprobante.PdfUrl))
        {
            comprobante.DocumentoImpreso = true;
        }

        await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);
        return new ComprobanteResultadoDto(comprobante.ComprobanteId, comprobante.Serie, comprobante.Correlativo, comprobante.Total, comprobante.PdfUrl, comprobante.EstadoSunat, mensaje);
    }

    private async Task<string> ConsultarEstadoSunatPendienteAsync(Comprobante comprobante, string mensajeActual, CancellationToken cancellationToken)
    {
        var consulta = await nubefactService.ConsultarEstadoAsync(comprobante, cancellationToken);
        comprobante.EstadoSunat = consulta.EstadoSunat;
        comprobante.PdfUrl = string.IsNullOrWhiteSpace(consulta.PdfUrl) ? comprobante.PdfUrl : consulta.PdfUrl;
        comprobante.XmlUrl = string.IsNullOrWhiteSpace(consulta.XmlUrl) ? comprobante.XmlUrl : consulta.XmlUrl;
        comprobante.NubefactHash = string.IsNullOrWhiteSpace(consulta.Hash) ? comprobante.NubefactHash : consulta.Hash;
        comprobante.NubefactRespuesta = consulta.RespuestaCompleta;
        return string.IsNullOrWhiteSpace(consulta.Mensaje) ? mensajeActual : consulta.Mensaje;
    }

    public async Task<ComprobanteResultadoDto> ConsultarEstadoSunatAsync(int comprobanteId, CancellationToken cancellationToken)
    {
        var comprobante = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken)
            ?? throw new InvalidOperationException("Comprobante no encontrado.");

        if (comprobante.TipoComprobante is not (TipoComprobante.BOL or TipoComprobante.FAC or TipoComprobante.NCR))
        {
            throw new InvalidOperationException("La consulta SUNAT solo aplica para boletas, facturas y notas de credito.");
        }

        var nubefact = await nubefactService.ConsultarEstadoAsync(comprobante, cancellationToken);
        comprobante.EstadoSunat = nubefact.EstadoSunat;
        comprobante.PdfUrl = string.IsNullOrWhiteSpace(nubefact.PdfUrl) ? comprobante.PdfUrl : nubefact.PdfUrl;
        comprobante.XmlUrl = string.IsNullOrWhiteSpace(nubefact.XmlUrl) ? comprobante.XmlUrl : nubefact.XmlUrl;
        comprobante.NubefactHash = string.IsNullOrWhiteSpace(nubefact.Hash) ? comprobante.NubefactHash : nubefact.Hash;
        comprobante.NubefactRespuesta = nubefact.RespuestaCompleta;
        await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);

        return new ComprobanteResultadoDto(comprobante.ComprobanteId, comprobante.Serie, comprobante.Correlativo, comprobante.Total, comprobante.PdfUrl, comprobante.EstadoSunat, nubefact.Mensaje);
    }

    public async Task AnularCotizacionAsync(int cotizacionId, CancellationToken cancellationToken)
    {
        var cotizacion = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, cotizacionId, cancellationToken)
            ?? throw new InvalidOperationException("Cotizacion no encontrada.");

        if (cotizacion.TipoComprobante != TipoComprobante.COT)
        {
            throw new InvalidOperationException("Solo se pueden anular cotizaciones desde esta opcion.");
        }

        cotizacion.Estado = EstadoRegistro.Anulado;
        await comprobanteRepository.GuardarAsync(cotizacion, cancellationToken);
    }

    public async Task<AnularNotaPedidoResultadoDto> AnularComprobanteAsync(int comprobanteId, string motivo, CancellationToken cancellationToken)
    {
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo))
        {
            throw new InvalidOperationException("Ingrese el motivo de anulacion del comprobante.");
        }

        var comprobante = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken)
            ?? throw new InvalidOperationException("Comprobante no encontrado.");

        if (comprobante.TipoComprobante == TipoComprobante.COT)
        {
            throw new InvalidOperationException("Use la opcion Cotizaciones para anular una cotizacion.");
        }

        if (comprobante.Estado == EstadoRegistro.Anulado)
        {
            throw new InvalidOperationException("El comprobante ya esta anulado.");
        }

        if (PeruDateTime.Today > comprobante.FechaEmision.Date.AddDays(2))
        {
            throw new InvalidOperationException("No se puede anular el comprobante porque esta fuera de fecha. Emita una nota de credito para reversar la operacion.");
        }

        if (comprobante.TipoComprobante is TipoComprobante.BOL or TipoComprobante.FAC or TipoComprobante.NCR && !comprobante.DocumentoImpreso)
        {
            throw new InvalidOperationException("No se puede anular el comprobante porque aun no fue enviado a Nubefact. Primero debe imprimir el comprobante para poder anularlo.");
        }

        var totalCobradoActivo = TotalPagadoComprobante(comprobante);
        comprobante.MotivoAnulacion = motivo;

        if (comprobante.TipoComprobante is TipoComprobante.BOL or TipoComprobante.FAC or TipoComprobante.NCR)
        {
            var nubefact = await nubefactService.AnularComprobanteAsync(comprobante, cancellationToken);
            comprobante.EstadoSunat = nubefact.EstadoSunat;
            comprobante.NubefactRespuesta = nubefact.RespuestaCompleta;
            if (!nubefact.Exitoso)
            {
                await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(nubefact.Mensaje)
                    ? "No se pudo anular el comprobante en Nubefact."
                    : $"No se pudo anular el comprobante en Nubefact. {nubefact.Mensaje}");
            }
        }
        else
        {
            comprobante.EstadoSunat = EstadoSunat.NoAplica;
        }

        ActualizarResumenPago(comprobante, totalCobradoActivo);
        comprobante.Estado = EstadoRegistro.Anulado;
        Devolucion? devolucion = null;
        await comprobanteRepository.EjecutarEnTransaccionAsync(async () =>
        {
            await comprobanteRepository.GuardarAsync(comprobante, cancellationToken);
            devolucion = await devolucionService.CrearDevolucionPorAnulacionComprobanteAsync(comprobante, totalCobradoActivo, motivo, cancellationToken);
        }, cancellationToken);

        if (devolucion is not null)
        {
            return new AnularNotaPedidoResultadoDto(
                true,
                devolucion.MontoOriginal,
                $"Comprobante anulado. Los cobros historicos permaneceran registrados. Se genero una devolucion pendiente por S/ {devolucion.MontoOriginal:N2}.");
        }

        return new AnularNotaPedidoResultadoDto(false, 0, "Comprobante anulado.");
    }

    public Task<ComprobanteResultadoDto> ConvertirCotizacionAsync(int cotizacionId, TipoComprobante tipoDestino, bool imprimir, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("La cotizacion solo puede convertirse a Nota de Pedido.");

    public async Task<NotaCreditoEditDto> PrepararNotaCreditoAsync(int comprobanteId, CancellationToken cancellationToken)
    {
        var comprobante = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, comprobanteId, cancellationToken)
            ?? throw new InvalidOperationException("Comprobante no encontrado.");

        await ValidarPuedeEmitirNotaCreditoAsync(comprobante, cancellationToken);
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");
        var motivos = await motivoNotaCreditoRepository.ListarActivosAsync(cancellationToken);
        var serie = ObtenerSerieNotaCredito(empresa, comprobante.TipoComprobante);
        var totalCobrado = TotalPagadoComprobante(comprobante);
        var totalNotasCredito = await comprobanteRepository.TotalNotasCreditoActivasAsync(empresaContext.EmpresaId, comprobante.ComprobanteId, cancellationToken);
        var saldoDisponible = Math.Max(0, comprobante.Total - totalNotasCredito);
        return new NotaCreditoEditDto
        {
            ComprobanteReferenciaId = comprobante.ComprobanteId,
            Serie = serie,
            Correlativo = await comprobanteRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, TipoComprobante.NCR, serie, cancellationToken),
            FechaEmision = PeruDateTime.Today,
            Referencia = $"{comprobante.Serie}-{comprobante.Correlativo:000000}",
            TipoComprobanteOrigen = comprobante.TipoComprobante,
            FechaOrigen = comprobante.FechaEmision,
            Cliente = comprobante.ClienteNombreMostrar,
            DocumentoCliente = comprobante.ClienteNumeroDocumentoMostrar,
            Total = comprobante.Total,
            TotalCobradoOrigen = totalCobrado,
            TotalNotasCreditoEmitidas = totalNotasCredito,
            SaldoDisponible = saldoDisponible,
            NuevoTotalValido = 0,
            MontoDevolucionEstimado = totalCobrado > 0 ? totalCobrado : 0,
            MotivoNotaCreditoId = motivos.FirstOrDefault()?.MotivoNotaCreditoId ?? 0,
            Motivo = motivos.FirstOrDefault()?.Nombre ?? string.Empty,
            Motivos = motivos,
            Detalles = comprobante.Detalles.Select(x => new NotaCreditoDetalleDto
            {
                ProductoId = x.ProductoId,
                Producto = x.Producto?.Nombre ?? string.Empty,
                CantidadOriginal = x.Cantidad,
                Cantidad = x.Cantidad,
                PrecioUnitario = x.PrecioUnitario,
                AfectoIgv = x.Producto?.AfectoIgv ?? true
            }).ToList()
        };
    }

    public async Task<NotaCreditoEditDto> PrepararNotaCreditoInicialAsync(CancellationToken cancellationToken)
    {
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");
        var serie = string.IsNullOrWhiteSpace(empresa.SerieNotaCreditoFactura) ? empresa.SerieNotaCredito : empresa.SerieNotaCreditoFactura;
        var motivos = await motivoNotaCreditoRepository.ListarActivosAsync(cancellationToken);
        return new NotaCreditoEditDto
        {
            Serie = serie,
            Correlativo = await comprobanteRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, TipoComprobante.NCR, serie, cancellationToken),
            FechaEmision = PeruDateTime.Today,
            MotivoNotaCreditoId = motivos.FirstOrDefault()?.MotivoNotaCreditoId ?? 0,
            Motivo = motivos.FirstOrDefault()?.Nombre ?? string.Empty,
            SaldoDisponible = 0,
            Motivos = motivos
        };
    }

    public async Task<ComprobanteResultadoDto> EmitirNotaCreditoAsync(NotaCreditoEditDto dto, CancellationToken cancellationToken)
    {
        if (dto.MotivoNotaCreditoId <= 0)
        {
            throw new InvalidOperationException("Seleccione el motivo de la nota de credito.");
        }

        var original = await comprobanteRepository.ObtenerAsync(empresaContext.EmpresaId, dto.ComprobanteReferenciaId, cancellationToken)
            ?? throw new InvalidOperationException("Comprobante original no encontrado.");

        await ValidarPuedeEmitirNotaCreditoAsync(original, cancellationToken);
        var motivo = await motivoNotaCreditoRepository.ObtenerAsync(dto.MotivoNotaCreditoId, cancellationToken)
            ?? throw new InvalidOperationException("Motivo de nota de credito no encontrado.");
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");
        var detallesSolicitados = dto.Detalles
            .Where(x => x.ProductoId > 0 && x.Cantidad > 0)
            .ToDictionary(x => x.ProductoId, x => x.Cantidad);
        if (detallesSolicitados.Count == 0)
        {
            throw new InvalidOperationException("Ingrese al menos una cantidad para la nota de credito.");
        }

        var serie = ObtenerSerieNotaCredito(empresa, original.TipoComprobante);
        var notaCredito = new Comprobante
        {
            EmpresaId = empresaContext.EmpresaId,
            TipoComprobante = TipoComprobante.NCR,
            Serie = serie,
            Correlativo = await comprobanteRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, TipoComprobante.NCR, serie, cancellationToken),
            ClienteId = original.ClienteId,
            Cliente = original.Cliente,
            Direccion = original.ClienteDireccionMostrar,
            FechaEmision = dto.FechaEmision.Date,
            FormaPago = FormaPago.Contado,
            Empresa = empresa,
            EmpresaRazonSocial = empresa.RazonSocial,
            EmpresaNombreComercial = empresa.NombreComercial,
            EmpresaRuc = empresa.RUC,
            EmpresaDireccion = empresa.Direccion,
            EmpresaTelefono = empresa.Telefono,
            EmpresaEmail = empresa.Email,
            CondicionesVenta = original.CondicionesVenta,
            CaracteristicasTecnicas = original.CaracteristicasTecnicas,
            ComprobanteReferenciaId = original.ComprobanteId,
            ComprobanteReferencia = original,
            MotivoNotaCreditoId = motivo.MotivoNotaCreditoId,
            MotivoNotaCredito = string.IsNullOrWhiteSpace(dto.SustentoDescripcion) ? motivo.Nombre : $"{motivo.Nombre} - {dto.SustentoDescripcion.Trim()}",
            EstadoSunat = EstadoSunat.Pendiente,
            UsuarioRegistro = empresaContext.UsuarioNombre
        };
        notaCredito.AplicarSnapshotClienteDesde(original);

        foreach (var item in original.Detalles)
        {
            if (!detallesSolicitados.TryGetValue(item.ProductoId, out var cantidadNc) || cantidadNc <= 0)
            {
                continue;
            }

            if (cantidadNc > item.Cantidad)
            {
                throw new InvalidOperationException($"La cantidad NC de {item.Producto?.Nombre ?? item.ProductoId.ToString()} no puede superar la cantidad original.");
            }

            var factor = item.Cantidad <= 0 ? 0 : cantidadNc / item.Cantidad;
            notaCredito.Detalles.Add(new ComprobanteDetalle
            {
                ProductoId = item.ProductoId,
                Producto = item.Producto,
                Cantidad = cantidadNc,
                PrecioUnitario = item.PrecioUnitario,
                Importe = decimal.Round(item.Importe * factor, 2),
                ImporteIgv = decimal.Round(item.ImporteIgv * factor, 2),
                MontoDetraccion = decimal.Round(item.MontoDetraccion * factor, 2)
            });
        }

        if (notaCredito.Detalles.Count == 0)
        {
            throw new InvalidOperationException("Ingrese al menos una cantidad para la nota de credito.");
        }

        notaCredito.RecalcularTotales();
        notaCredito.TotalPagado = 0;
        notaCredito.SaldoPendiente = 0;
        notaCredito.EstadoPago = EstadoPagoComprobante.PENDIENTE;

        await comprobanteRepository.EjecutarEnTransaccionAsync(async () =>
        {
            await comprobanteRepository.GuardarAsync(notaCredito, cancellationToken);
        }, cancellationToken);

        var nubefact = await nubefactService.EmitirComprobanteAsync(notaCredito, cancellationToken);
        notaCredito.EstadoSunat = nubefact.EstadoSunat;
        notaCredito.PdfUrl = nubefact.PdfUrl;
        notaCredito.XmlUrl = nubefact.XmlUrl;
        notaCredito.NubefactHash = nubefact.Hash;
        notaCredito.NubefactRespuesta = nubefact.RespuestaCompleta;
        notaCredito.DocumentoImpreso = !string.IsNullOrWhiteSpace(notaCredito.PdfUrl);
        await comprobanteRepository.GuardarAsync(notaCredito, cancellationToken);

        var totalCobradoOriginal = TotalPagadoComprobante(original);
        var totalNotasCredito = await comprobanteRepository.TotalNotasCreditoActivasAsync(empresaContext.EmpresaId, original.ComprobanteId, cancellationToken);
        ActualizarResumenPago(original, totalCobradoOriginal, totalNotasCredito);
        await comprobanteRepository.GuardarAsync(original, cancellationToken);

        var devolucion = await devolucionService.CrearDevolucionPorNotaCreditoAsync(original, notaCredito, totalCobradoOriginal, cancellationToken);
        var mensaje = nubefact.Mensaje;
        if (devolucion is not null)
        {
            mensaje = $"{mensaje} Los cobros existentes no fueron anulados. Se genero una devolucion pendiente por S/ {devolucion.MontoOriginal:N2}.";
        }

        return new ComprobanteResultadoDto(notaCredito.ComprobanteId, notaCredito.Serie, notaCredito.Correlativo, notaCredito.Total, notaCredito.PdfUrl, notaCredito.EstadoSunat, mensaje);
    }

    private static string ObtenerSerie(Empresa empresa, TipoComprobante tipo) => tipo switch
    {
        TipoComprobante.BOL => empresa.SerieBoleta,
        TipoComprobante.FAC => empresa.SerieFactura,
        TipoComprobante.NCR => empresa.SerieNotaCredito,
        TipoComprobante.NPE => empresa.SerieNotaPedido,
        _ => empresa.SerieCotizacion
    };

    private static void ValidarTipoComprobantePorDocumento(TipoDocumentoCliente tipoDocumento, TipoComprobante tipoComprobante)
    {
        if (tipoDocumento == TipoDocumentoCliente.RUC && tipoComprobante == TipoComprobante.BOL)
        {
            throw new InvalidOperationException("No se puede generar una boleta para un cliente con RUC. Seleccione Factura o cambie el cliente.");
        }
    }
    private static string ObtenerSerieNotaCredito(Empresa empresa, TipoComprobante tipoOrigen) => tipoOrigen switch
    {
        TipoComprobante.FAC => string.IsNullOrWhiteSpace(empresa.SerieNotaCreditoFactura) ? empresa.SerieNotaCredito : empresa.SerieNotaCreditoFactura,
        TipoComprobante.BOL => string.IsNullOrWhiteSpace(empresa.SerieNotaCreditoBoleta) ? empresa.SerieNotaCredito : empresa.SerieNotaCreditoBoleta,
        _ => empresa.SerieNotaCredito
    };

    private async Task ValidarPuedeEmitirNotaCreditoAsync(Comprobante comprobante, CancellationToken cancellationToken)
    {
        if (comprobante.TipoComprobante is not (TipoComprobante.BOL or TipoComprobante.FAC))
        {
            throw new InvalidOperationException("La nota de credito solo se emite contra boletas o facturas.");
        }

        if (comprobante.Estado != EstadoRegistro.Activo)
        {
            throw new InvalidOperationException("No se puede emitir nota de credito sobre un comprobante anulado.");
        }

        if (comprobante.EstadoSunat != EstadoSunat.Aceptado)
        {
            throw new InvalidOperationException("Solo se puede emitir nota de credito sobre boletas o facturas aceptadas por SUNAT.");
        }

        if (await comprobanteRepository.TieneNotaCreditoActivaAsync(empresaContext.EmpresaId, comprobante.ComprobanteId, cancellationToken))
        {
            throw new InvalidOperationException("El comprobante ya tiene una nota de credito total activa.");
        }
    }

    private static bool PuedeEditar(Comprobante comprobante)
    {
        return comprobante.Estado == EstadoRegistro.Activo
            && !comprobante.DocumentoImpreso
            && comprobante.EstadoSunat != EstadoSunat.Aceptado;
    }

    private static string NormalizarMedioPago(string? medioPago) =>
        string.IsNullOrWhiteSpace(medioPago) ? "EFECTIVO" : medioPago.Trim().ToUpperInvariant();

    private static decimal TotalPagadoComprobante(Comprobante comprobante) =>
        comprobante.Cobros
            .Where(x => x.Estado == CobroClienteEstado.ACTIVO
                && !comprobante.CobrosAplicados.Any(a => a.CobroClienteId == x.CobroClienteId))
            .Sum(x => x.Monto)
        + comprobante.CobrosAplicados
            .Where(x => x.CobroCliente?.Estado == CobroClienteEstado.ACTIVO)
            .Sum(x => x.MontoAplicado);

    private static void ActualizarResumenPago(Comprobante comprobante, decimal totalPagado, decimal totalNotasCredito = 0)
    {
        comprobante.TotalPagado = decimal.Round(totalPagado, 2);
        var saldo = comprobante.Total - totalNotasCredito - comprobante.TotalPagado;
        comprobante.SaldoPendiente = decimal.Round(saldo < 0 ? 0 : saldo, 2);
        comprobante.EstadoPago = comprobante.TotalPagado <= 0
            ? EstadoPagoComprobante.PENDIENTE
            : comprobante.SaldoPendiente <= 0
                ? EstadoPagoComprobante.PAGADO
                : EstadoPagoComprobante.PAGO_PARCIAL;
    }

    private static ComprobanteEditDto ToEditDto(Comprobante comprobante) => new()
    {
        ComprobanteId = comprobante.ComprobanteId,
        TipoComprobante = comprobante.TipoComprobante,
        Serie = comprobante.Serie,
        Correlativo = comprobante.Correlativo,
        ClienteId = comprobante.ClienteId,
        ClienteTipoDocumento = comprobante.ClienteTipoDocumentoMostrar,
        ClienteNumeroDocumento = comprobante.ClienteNumeroDocumentoMostrar,
        ClienteNombre = comprobante.ClienteNombreMostrar,
        ClienteNombreComercial = comprobante.ClienteNombreComercialMostrar,
        ClienteDireccion = comprobante.ClienteDireccionMostrar,
        ClienteTelefono = comprobante.ClienteTelefonoMostrar,
        ClienteEmail = comprobante.ClienteEmailMostrar,
        CotizacionId = comprobante.CotizacionId,
        NotaPedidoId = comprobante.NotaPedidoId,
        DocumentoOrigen = comprobante.NotaPedido is null ? string.Empty : $"{comprobante.NotaPedido.Serie}-{comprobante.NotaPedido.Correlativo:000000}",
        ComprobanteReferenciaId = comprobante.ComprobanteReferenciaId,
        ComprobanteReferenciaNumero = comprobante.ComprobanteReferencia is null ? string.Empty : $"{comprobante.ComprobanteReferencia.Serie}-{comprobante.ComprobanteReferencia.Correlativo:000000}",
        MotivoNotaCredito = comprobante.MotivoNotaCredito,
        Direccion = comprobante.Direccion,
        FechaEmision = comprobante.FechaEmision,
        FormaPago = comprobante.FormaPago,
        EstadoPago = comprobante.EstadoPago,
        Estado = comprobante.Estado,
        EstadoSunat = comprobante.EstadoSunat,
        DocumentoImpreso = comprobante.DocumentoImpreso,
        Subtotal = comprobante.SubTotal,
        Igv = comprobante.Igv,
        Total = comprobante.Total,
        TotalPagado = TotalPagadoComprobante(comprobante),
        SaldoPendiente = SaldoPendiente(comprobante.Total, TotalPagadoComprobante(comprobante)),
        EmpresaRazonSocial = comprobante.EmpresaRazonSocial,
        EmpresaNombreComercial = comprobante.EmpresaNombreComercial,
        EmpresaRuc = comprobante.EmpresaRuc,
        EmpresaDireccion = comprobante.EmpresaDireccion,
        EmpresaTelefono = comprobante.EmpresaTelefono,
        EmpresaEmail = comprobante.EmpresaEmail,
        CondicionesVenta = comprobante.CondicionesVenta,
        CaracteristicasTecnicas = comprobante.CaracteristicasTecnicas,
        Detalles = comprobante.Detalles
            .Select(x => new ComprobanteDetalleDto
            {
                ProductoId = x.ProductoId,
                Producto = x.Producto?.Nombre ?? string.Empty,
                Cantidad = x.Cantidad,
                PrecioUnitario = x.PrecioUnitario
            })
            .ToList()
    };

    private static decimal SaldoPendiente(decimal total, decimal pagado)
    {
        var saldo = total - pagado;
        return decimal.Round(saldo < 0 ? 0 : saldo, 2);
    }
}









