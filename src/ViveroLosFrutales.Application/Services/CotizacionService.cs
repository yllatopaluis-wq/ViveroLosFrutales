using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class CotizacionService(
    ICotizacionRepository cotizacionRepository,
    IClienteRepository clienteRepository,
    IProductoRepository productoRepository,
    IEmpresaRepository empresaRepository,
    INotaPedidoRepository notaPedidoRepository,
    IPdfService pdfService,
    IEmpresaContext empresaContext)
{
    public Task<PagedResult<CotizacionListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        cotizacionRepository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public async Task<CotizacionFormDataDto> CrearFormularioInicialAsync(CancellationToken cancellationToken)
    {
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");
        var dto = new CotizacionEditDto
        {
            Serie = empresa.SerieCotizacion,
            Correlativo = await cotizacionRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, empresa.SerieCotizacion, cancellationToken),
            FechaEmision = PeruDateTime.Today,
            CondicionesVenta = "Plazo de entrega: A los 10 dias de realizado el pago.\nLugar de entrega: En las instalaciones del vivero.\nForma de pago: Contado.\nGarantia: 1 meses\nMedios de Pago: En efectivo, deposito a cuenta corriente o pago con tarjeta de credito o debito (aceptamos todos los tipos de tarjetas debito y creditos).",
            CaracteristicasTecnicas = "Semillas seleccionadas de campos certificados con control fitosanitarios.\nEdad: Plantas de 4 meses.\nTamano de 40 - 50 cm\nBolsas medidas de 7 x 14\nPeso aprox. por planta 4 kilos"
        };
        return await ObtenerFormularioAsync(dto, cancellationToken);
    }

    public async Task<CotizacionFormDataDto> ObtenerFormularioAsync(CotizacionEditDto dto, CancellationToken cancellationToken)
    {
        var clienteSeleccionado = dto.ClienteId > 0
            ? await clienteRepository.ObtenerAsync(dto.ClienteId, cancellationToken)
            : null;
        var productoIds = dto.Detalles.Where(x => x.ProductoId > 0).Select(x => x.ProductoId).Distinct().ToArray();
        var productos = (await productoRepository.BuscarActivosAsync(empresaContext.EmpresaId, null, 50, cancellationToken)).ToList();
        foreach (var productoId in productoIds)
        {
            if (productos.Any(x => x.ProductoId == productoId)) continue;
            var producto = await productoRepository.ObtenerAsync(empresaContext.EmpresaId, productoId, cancellationToken);
            if (producto is null) continue;
            productos.Add(new ProductoListDto(producto.ProductoId, producto.Categoria, producto.Nombre, producto.PrecioVentaSinIgv, producto.PrecioVentaConIgv, producto.Stock, producto.AfectoIgv, producto.Estado));
        }

        var clientes = (await clienteRepository.BuscarActivosAsync(null, 50, cancellationToken)).ToList();
        if (clienteSeleccionado is not null && clientes.All(x => x.ClienteId != clienteSeleccionado.ClienteId))
        {
            clientes.Add(new ClienteListDto(clienteSeleccionado.ClienteId, clienteSeleccionado.NombreCompleto, clienteSeleccionado.TipoDocumento, clienteSeleccionado.NumeroDocumento, clienteSeleccionado.Direccion, clienteSeleccionado.Telefono, clienteSeleccionado.Estado));
        }

        return new CotizacionFormDataDto(
            dto,
            new CotizacionNumeracionDto(dto.Serie, dto.Correlativo),
            clientes
                .OrderBy(x => x.NombreCompleto)
                .Select(x => new ComprobanteClienteOptionDto(x.ClienteId, x.NombreCompleto, x.NumeroDocumento, x.Direccion))
                .ToArray(),
            productos.Select(x => new ComprobanteProductoOptionDto(x.ProductoId, x.Nombre, x.Categoria, x.PrecioVentaConIgv, x.Stock, x.AfectoIgv)).ToArray());
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

    public async Task<CotizacionEditDto> ObtenerParaEditarAsync(int id, CancellationToken cancellationToken)
    {
        var cotizacion = await cotizacionRepository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Cotizacion no encontrada.");
        if (cotizacion.EstadoCotizacion != CotizacionEstado.ACTIVA)
            throw new InvalidOperationException("No se puede editar una cotizacion convertida o cerrada.");

        return ToEditDto(cotizacion);
    }

    public async Task<CotizacionDetalleViewDto> ObtenerDetalleAsync(int id, CancellationToken cancellationToken)
    {
        var cotizacion = await cotizacionRepository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Cotizacion no encontrada.");
        var tieneDocumentos = await cotizacionRepository.TieneDocumentosRelacionadosAsync(empresaContext.EmpresaId, id, cancellationToken);

        return new CotizacionDetalleViewDto
        {
            CotizacionId = cotizacion.CotizacionId,
            Numero = $"{cotizacion.Serie}-{cotizacion.Correlativo:000000}",
            FechaEmision = cotizacion.FechaEmision,
            Cliente = cotizacion.Cliente?.NombreCompleto ?? string.Empty,
            TipoDocumento = cotizacion.Cliente?.TipoDocumento ?? TipoDocumentoCliente.DNI,
            Documento = cotizacion.Cliente?.NumeroDocumento ?? string.Empty,
            Direccion = cotizacion.Direccion,
            FormaPago = cotizacion.FormaPago,
            Subtotal = cotizacion.SubTotal,
            Igv = cotizacion.Igv,
            Total = cotizacion.Total,
            EstadoCotizacion = cotizacion.EstadoCotizacion,
            TieneDocumentosRelacionados = tieneDocumentos,
            Detalles = cotizacion.Detalles.Select(x => new NotaPedidoDetalleDto(
                x.Producto?.Nombre ?? x.ProductoId.ToString(),
                x.Cantidad,
                x.PrecioUnitario,
                x.Importe,
                x.ImporteIgv,
                x.Importe + x.ImporteIgv)).ToArray()
        };
    }

    public async Task<CotizacionResultadoDto> GuardarAsync(CotizacionEditDto dto, CancellationToken cancellationToken)
    {
        if (dto.ClienteId <= 0) throw new InvalidOperationException("Seleccione un cliente.");
        var detalles = dto.Detalles.Where(x => x.ProductoId > 0 && x.Cantidad > 0 && x.PrecioUnitario > 0).ToArray();
        if (detalles.Length == 0) throw new InvalidOperationException("Agregue al menos un producto.");
        var cliente = await clienteRepository.ObtenerAsync(dto.ClienteId, cancellationToken)
            ?? throw new InvalidOperationException("Cliente no encontrado.");
        var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
            ?? throw new InvalidOperationException("Empresa activa no encontrada.");

        Cotizacion cotizacion;
        if (dto.CotizacionId == 0)
        {
            cotizacion = new Cotizacion
            {
                EmpresaId = empresaContext.EmpresaId,
                Serie = empresa.SerieCotizacion,
                Correlativo = await cotizacionRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, empresa.SerieCotizacion, cancellationToken),
                UsuarioRegistro = empresaContext.UsuarioNombre
            };
        }
        else
        {
            cotizacion = await cotizacionRepository.ObtenerAsync(empresaContext.EmpresaId, dto.CotizacionId, cancellationToken)
                ?? throw new InvalidOperationException("Cotizacion no encontrada.");
            if (cotizacion.EstadoCotizacion != CotizacionEstado.ACTIVA)
                throw new InvalidOperationException("No se puede editar una cotizacion convertida o cerrada.");
            cotizacion.Detalles.Clear();
            cotizacion.FechaModificacion = DateTime.UtcNow;
            cotizacion.UsuarioModificacion = empresaContext.UsuarioNombre;
        }

        cotizacion.ClienteId = cliente.ClienteId;
        cotizacion.Cliente = cliente;
        cotizacion.Empresa = empresa;
        cotizacion.FechaEmision = dto.FechaEmision.Date;
        cotizacion.Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? cliente.Direccion : dto.Direccion.Trim();
        cotizacion.FormaPago = dto.FormaPago;
        cotizacion.EmpresaRazonSocial = empresa.RazonSocial;
        cotizacion.EmpresaNombreComercial = empresa.NombreComercial;
        cotizacion.EmpresaRuc = empresa.RUC;
        cotizacion.EmpresaDireccion = empresa.Direccion;
        cotizacion.EmpresaTelefono = empresa.Telefono;
        cotizacion.EmpresaEmail = empresa.Email;
        cotizacion.CondicionesVenta = dto.CondicionesVenta.Trim();
        cotizacion.CaracteristicasTecnicas = dto.CaracteristicasTecnicas.Trim();
        cotizacion.EstadoCotizacion = CotizacionEstado.ACTIVA;

        foreach (var item in detalles)
        {
            var producto = await productoRepository.ObtenerAsync(empresaContext.EmpresaId, item.ProductoId, cancellationToken)
                ?? throw new InvalidOperationException("Producto no encontrado.");
            var totalLinea = decimal.Round(item.Cantidad * item.PrecioUnitario, 2);
            var importe = producto.AfectoIgv ? decimal.Round(totalLinea / 1.18m, 2) : totalLinea;
            var igv = producto.AfectoIgv ? decimal.Round(totalLinea - importe, 2) : 0;
            cotizacion.Detalles.Add(new CotizacionDetalle
            {
                ProductoId = producto.ProductoId,
                Producto = producto,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Importe = importe,
                ImporteIgv = igv
            });
        }

        cotizacion.RecalcularTotales();
        await cotizacionRepository.GuardarAsync(cotizacion, cancellationToken);
        return new CotizacionResultadoDto(cotizacion.CotizacionId, cotizacion.Serie, cotizacion.Correlativo, cotizacion.Total, cotizacion.PdfUrl, cotizacion.EstadoCotizacion);
    }

    public async Task<CotizacionResultadoDto> ImprimirAsync(int id, CancellationToken cancellationToken)
    {
        var cotizacion = await cotizacionRepository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Cotizacion no encontrada.");
        cotizacion.PdfUrl = await pdfService.GenerarCotizacionAsync(cotizacion, cancellationToken);
        await cotizacionRepository.GuardarAsync(cotizacion, cancellationToken);
        return new CotizacionResultadoDto(cotizacion.CotizacionId, cotizacion.Serie, cotizacion.Correlativo, cotizacion.Total, cotizacion.PdfUrl, cotizacion.EstadoCotizacion);
    }

    public async Task<int> ConvertirANotaPedidoAsync(int id, CancellationToken cancellationToken)
    {
        var notaId = 0;
        await cotizacionRepository.EjecutarEnTransaccionAsync(async () =>
        {
            var cotizacion = await ObtenerConvertibleAsync(id, cancellationToken);
            var empresa = await empresaRepository.ObtenerAsync(empresaContext.EmpresaId, cancellationToken)
                ?? throw new InvalidOperationException("Empresa activa no encontrada.");
            var nota = new NotaPedido
            {
                EmpresaId = empresaContext.EmpresaId,
                CotizacionId = cotizacion.CotizacionId,
                ClienteId = cotizacion.ClienteId,
                Serie = empresa.SerieNotaPedido,
                Correlativo = await notaPedidoRepository.SiguienteCorrelativoAsync(empresaContext.EmpresaId, empresa.SerieNotaPedido, cancellationToken),
                Fecha = PeruDateTime.Today,
                UsuarioRegistro = empresaContext.UsuarioNombre
            };
            foreach (var d in cotizacion.Detalles)
                nota.Detalles.Add(new NotaPedidoDetalle { ProductoId = d.ProductoId, Cantidad = d.Cantidad, PrecioUnitario = d.PrecioUnitario, Subtotal = d.Importe, Igv = d.ImporteIgv, Total = d.Importe + d.ImporteIgv });
            nota.RecalcularTotales();
            nota.EstadoDocumento = NotaPedidoEstado.ACTIVO;
            nota.EstadoPago = EstadoPagoNotaPedido.PENDIENTE;
            await notaPedidoRepository.GuardarAsync(nota, cancellationToken);
            cotizacion.EstadoCotizacion = CotizacionEstado.CONVERTIDA;
            await cotizacionRepository.GuardarAsync(cotizacion, cancellationToken);
            notaId = nota.NotaPedidoId;
        }, cancellationToken);

        return notaId;
    }

    public async Task CambiarEstadoAsync(int id, CotizacionEstado estado, CancellationToken cancellationToken)
    {
        var cotizacion = await cotizacionRepository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Cotizacion no encontrada.");
        if (cotizacion.EstadoCotizacion == CotizacionEstado.ANULADA)
            throw new InvalidOperationException("No se puede cambiar una cotizacion anulada.");
        if (EsCotizacionConvertida(cotizacion.EstadoCotizacion))
            throw new InvalidOperationException("No se puede cambiar una cotizacion convertida.");
        if (estado == CotizacionEstado.ANULADA && await cotizacionRepository.TieneDocumentosRelacionadosAsync(empresaContext.EmpresaId, id, cancellationToken))
            throw new InvalidOperationException("No se puede anular una cotizacion con documentos relacionados.");
        cotizacion.EstadoCotizacion = estado;
        cotizacion.FechaModificacion = DateTime.UtcNow;
        cotizacion.UsuarioModificacion = empresaContext.UsuarioNombre;
        await cotizacionRepository.GuardarAsync(cotizacion, cancellationToken);
    }

    private async Task<Cotizacion> ObtenerConvertibleAsync(int id, CancellationToken cancellationToken)
    {
        var cotizacion = await cotizacionRepository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Cotizacion no encontrada.");
        if (cotizacion.EstadoCotizacion != CotizacionEstado.ACTIVA)
            throw new InvalidOperationException("No se puede convertir dos veces o convertir una cotizacion cerrada.");
        if (await cotizacionRepository.TieneDocumentosRelacionadosAsync(empresaContext.EmpresaId, id, cancellationToken))
            throw new InvalidOperationException("La cotizacion ya tiene documentos relacionados.");
        return cotizacion;
    }

    private static bool EsCotizacionConvertida(CotizacionEstado estado) =>
        (int)estado is 3 or 4 or 5;

    private static CotizacionEditDto ToEditDto(Cotizacion c) => new()
    {
        CotizacionId = c.CotizacionId,
        ClienteId = c.ClienteId,
        Serie = c.Serie,
        Correlativo = c.Correlativo,
        FechaEmision = c.FechaEmision,
        Direccion = c.Direccion,
        FormaPago = c.FormaPago,
        CondicionesVenta = c.CondicionesVenta,
        CaracteristicasTecnicas = c.CaracteristicasTecnicas,
        Detalles = c.Detalles.Select(x => new ComprobanteDetalleDto { ProductoId = x.ProductoId, Cantidad = x.Cantidad, PrecioUnitario = x.PrecioUnitario }).ToList()
    };
}
