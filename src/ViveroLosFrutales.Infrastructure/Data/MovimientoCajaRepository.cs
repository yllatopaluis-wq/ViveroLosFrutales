using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class MovimientoCajaRepository(ApplicationDbContext db) : IMovimientoCajaRepository
{
    public async Task<CajaIndexDto> BuscarAsync(int empresaId, SearchRequest request, string? medioPago, TipoMovimientoCaja? tipoMovimiento, CancellationToken cancellationToken)
    {
        var query = db.MovimientosCaja.AsNoTracking().Where(x => x.EmpresaId == empresaId);
        if (request.FechaDesde is not null) query = query.Where(x => x.Fecha >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.Fecha < hasta);
        }

        if (!string.IsNullOrWhiteSpace(medioPago))
        {
            var term = medioPago.Trim();
            query = query.Where(x => x.MedioPago.Contains(term));
        }

        if (tipoMovimiento is not null) query = query.Where(x => x.TipoMovimiento == tipoMovimiento);

        var movimientosBase = await query.OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.MovimientoCajaId)
            .ToListAsync(cancellationToken);

        var cobroIds = movimientosBase
            .Where(x => x.Origen == OrigenMovimientoCaja.COBRO_CLIENTE)
            .Select(x => x.OrigenId)
            .Distinct()
            .ToArray();
        var devolucionIds = movimientosBase
            .Where(x => x.Origen == OrigenMovimientoCaja.DEVOLUCION_CLIENTE)
            .Select(x => x.OrigenId)
            .Distinct()
            .ToArray();
        var devolucionProveedorIds = movimientosBase
            .Where(x => x.Origen == OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR)
            .Select(x => x.OrigenId)
            .Distinct()
            .ToArray();
        var gastoIds = movimientosBase
            .Where(x => x.Origen == OrigenMovimientoCaja.GASTO)
            .Select(x => x.OrigenId)
            .Distinct()
            .ToArray();
        var pagoProveedorIds = movimientosBase
            .Where(x => x.Origen == OrigenMovimientoCaja.PAGO_PROVEEDOR)
            .Select(x => x.OrigenId)
            .Distinct()
            .ToArray();
        var ingresoIds = movimientosBase
            .Where(x => x.Origen == OrigenMovimientoCaja.INGRESO_MANUAL)
            .Select(x => x.OrigenId)
            .Distinct()
            .ToArray();

        var cobros = await db.CobrosCliente.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && cobroIds.Contains(x.CobroClienteId))
            .Select(x => new
            {
                x.CobroClienteId,
                Cliente = x.Cliente!.NombreCompleto,
                NotaSerie = x.NotaPedido == null ? string.Empty : x.NotaPedido.Serie,
                NotaCorrelativo = x.NotaPedido == null ? 0 : x.NotaPedido.Correlativo,
                ComprobanteSerie = x.Comprobante == null ? string.Empty : x.Comprobante.Serie,
                ComprobanteCorrelativo = x.Comprobante == null ? 0 : x.Comprobante.Correlativo,
                Aplicado = x.Aplicaciones
                    .OrderByDescending(a => a.FechaAplicacion)
                    .Select(a => new { a.Comprobante!.Serie, a.Comprobante.Correlativo })
                    .FirstOrDefault()
            })
            .ToDictionaryAsync(x => x.CobroClienteId, cancellationToken);

        var comprobantes = await db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && devolucionIds.Contains(x.ComprobanteId))
            .Select(x => new
            {
                x.ComprobanteId,
                Cliente = x.Cliente!.NombreCompleto,
                x.Serie,
                x.Correlativo
            })
            .ToDictionaryAsync(x => x.ComprobanteId, cancellationToken);

        var devoluciones = await db.Devoluciones.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && (devolucionIds.Contains(x.DevolucionId) || devolucionProveedorIds.Contains(x.DevolucionId)))
            .Select(x => new
            {
                x.DevolucionId,
                Tercero = x.TipoTercero == TipoTerceroDevolucion.CLIENTE ? x.Cliente!.NombreCompleto : x.Proveedor!.RazonSocial,
                NotaSerie = x.NotaPedido == null ? string.Empty : x.NotaPedido.Serie,
                NotaCorrelativo = x.NotaPedido == null ? 0 : x.NotaPedido.Correlativo,
                ComprobanteSerie = x.Comprobante == null ? string.Empty : x.Comprobante.Serie,
                ComprobanteCorrelativo = x.Comprobante == null ? 0 : x.Comprobante.Correlativo,
                NotaCreditoSerie = x.NotaCredito == null ? string.Empty : x.NotaCredito.Serie,
                NotaCreditoCorrelativo = x.NotaCredito == null ? 0 : x.NotaCredito.Correlativo,
                CompraDocumento = x.Compra == null ? string.Empty : x.Compra.Documento
            })
            .ToDictionaryAsync(x => x.DevolucionId, cancellationToken);

        var gastos = await db.Gastos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && gastoIds.Contains(x.GastoId))
            .Select(x => new
            {
                x.GastoId,
                x.Descripcion,
                x.Categoria
            })
            .ToDictionaryAsync(x => x.GastoId, cancellationToken);

        var pagosProveedor = await db.PagosProveedor.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && pagoProveedorIds.Contains(x.PagoProveedorId))
            .Select(x => new
            {
                x.PagoProveedorId,
                Proveedor = x.Proveedor!.RazonSocial,
                Documento = x.Compra == null ? string.Empty : x.Compra.Documento,
                CompraId = x.CompraId
            })
            .ToDictionaryAsync(x => x.PagoProveedorId, cancellationToken);

        var ingresosManuales = await db.Ingresos.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && ingresoIds.Contains(x.IngresoId))
            .Select(x => new
            {
                x.IngresoId,
                x.TipoIngreso,
                x.Descripcion
            })
            .ToDictionaryAsync(x => x.IngresoId, cancellationToken);

        var movimientosEnriquecidos = movimientosBase
            .Select(x =>
            {
                var clienteProveedor = "-";
                var documento = "-";
                var textoBusquedaExtra = string.Empty;

                if (x.Origen == OrigenMovimientoCaja.COBRO_CLIENTE && cobros.TryGetValue(x.OrigenId, out var cobro))
                {
                    clienteProveedor = cobro.Cliente;
                    documento = DocumentoCobro(
                        Numero(cobro.NotaSerie, cobro.NotaCorrelativo),
                        Numero(cobro.ComprobanteSerie, cobro.ComprobanteCorrelativo),
                        cobro.Aplicado is null ? string.Empty : Numero(cobro.Aplicado.Serie, cobro.Aplicado.Correlativo));
                    textoBusquedaExtra = $"{cobro.Cliente} {documento}";
                }
                else if (x.Origen == OrigenMovimientoCaja.DEVOLUCION_CLIENTE && devoluciones.TryGetValue(x.OrigenId, out var devolucion))
                {
                    clienteProveedor = devolucion.Tercero;
                    documento = DocumentoDevolucion(
                        Numero(devolucion.NotaSerie, devolucion.NotaCorrelativo),
                        Numero(devolucion.ComprobanteSerie, devolucion.ComprobanteCorrelativo),
                        Numero(devolucion.NotaCreditoSerie, devolucion.NotaCreditoCorrelativo),
                        devolucion.CompraDocumento);
                    textoBusquedaExtra = $"{devolucion.Tercero} {documento}";
                }
                else if (x.Origen == OrigenMovimientoCaja.DEVOLUCION_CLIENTE && comprobantes.TryGetValue(x.OrigenId, out var comprobante))
                {
                    clienteProveedor = comprobante.Cliente;
                    documento = Numero(comprobante.Serie, comprobante.Correlativo);
                    textoBusquedaExtra = $"{comprobante.Cliente} {documento}";
                }
                else if (x.Origen == OrigenMovimientoCaja.GASTO && gastos.TryGetValue(x.OrigenId, out var gasto))
                {
                    documento = $"GASTO-{gasto.GastoId:000000}";
                    textoBusquedaExtra = $"{documento} {gasto.Categoria} {gasto.Descripcion}";
                }
                else if (x.Origen == OrigenMovimientoCaja.PAGO_PROVEEDOR && pagosProveedor.TryGetValue(x.OrigenId, out var pagoProveedor))
                {
                    clienteProveedor = pagoProveedor.Proveedor;
                    documento = string.IsNullOrWhiteSpace(pagoProveedor.Documento) ? $"COMPRA-{pagoProveedor.CompraId:000000}" : pagoProveedor.Documento;
                    textoBusquedaExtra = $"{pagoProveedor.Proveedor} {documento}";
                }
                else if (x.Origen == OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR && devoluciones.TryGetValue(x.OrigenId, out var devolucionProveedor))
                {
                    clienteProveedor = devolucionProveedor.Tercero;
                    documento = DocumentoDevolucion(
                        Numero(devolucionProveedor.NotaSerie, devolucionProveedor.NotaCorrelativo),
                        Numero(devolucionProveedor.ComprobanteSerie, devolucionProveedor.ComprobanteCorrelativo),
                        Numero(devolucionProveedor.NotaCreditoSerie, devolucionProveedor.NotaCreditoCorrelativo),
                        devolucionProveedor.CompraDocumento);
                    textoBusquedaExtra = $"{devolucionProveedor.Tercero} {documento}";
                }
                else if (x.Origen == OrigenMovimientoCaja.INGRESO_MANUAL && ingresosManuales.TryGetValue(x.OrigenId, out var ingreso))
                {
                    documento = $"INGRESO-{ingreso.IngresoId:000000}";
                    textoBusquedaExtra = $"{documento} {ingreso.TipoIngreso} {ingreso.Descripcion}";
                }

                return new MovimientoCajaItem(
                    new MovimientoCajaListDto(
                        x.MovimientoCajaId,
                        x.Fecha,
                        x.TipoMovimiento,
                        x.Origen,
                        OrigenDescripcion(x.Origen),
                        clienteProveedor,
                        documento,
                        x.MedioPago,
                        x.Monto,
                        x.Estado),
                    $"{textoBusquedaExtra} {x.MedioPago} {x.Descripcion}");
            });

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            movimientosEnriquecidos = movimientosEnriquecidos
                .Where(x => x.SearchText.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var movimientosFiltrados = movimientosEnriquecidos.ToList();
        var movimientosActivos = movimientosFiltrados.Where(x => x.Dto.Estado == EstadoRegistro.Activo).ToList();
        var ingresos = movimientosActivos
            .Where(x => x.Dto.TipoMovimiento == TipoMovimientoCaja.INGRESO)
            .Sum(x => x.Dto.Monto);
        var egresos = movimientosActivos
            .Where(x => x.Dto.TipoMovimiento == TipoMovimientoCaja.EGRESO)
            .Sum(x => x.Dto.Monto);
        var take = request.PageSize <= 0 ? 100 : Math.Min(request.PageSize, 100);

        return new CajaIndexDto
        {
            Movimientos = movimientosFiltrados.Select(x => x.Dto).Take(take).ToList(),
            Resumen = new CajaResumenDto(ingresos, egresos, ingresos - egresos, movimientosActivos.Count)
        };
    }

    public async Task GuardarAsync(MovimientoCaja movimiento, CancellationToken cancellationToken)
    {
        if (movimiento.MovimientoCajaId == 0) db.MovimientosCaja.Add(movimiento);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<MovimientoCaja?> ObtenerPorOrigenAsync(int empresaId, OrigenMovimientoCaja origen, int origenId, CancellationToken cancellationToken) =>
        db.MovimientosCaja.FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.Origen == origen && x.OrigenId == origenId, cancellationToken);

    private static string DocumentoCobro(string notaNumero, string comprobanteNumero, string aplicadoNumero)
    {
        if (!string.IsNullOrWhiteSpace(notaNumero) && !string.IsNullOrWhiteSpace(aplicadoNumero))
        {
            return $"{notaNumero} -> {aplicadoNumero}";
        }

        if (!string.IsNullOrWhiteSpace(notaNumero)) return notaNumero;
        if (!string.IsNullOrWhiteSpace(comprobanteNumero)) return comprobanteNumero;
        if (!string.IsNullOrWhiteSpace(aplicadoNumero)) return aplicadoNumero;
        return "-";
    }

    private static string Numero(string serie, int correlativo) =>
        string.IsNullOrWhiteSpace(serie) || correlativo <= 0 ? string.Empty : $"{serie}-{correlativo:000000}";

    private static string DocumentoDevolucion(string notaNumero, string comprobanteNumero, string notaCreditoNumero, string compraDocumento)
    {
        if (!string.IsNullOrWhiteSpace(notaNumero)) return notaNumero;
        if (!string.IsNullOrWhiteSpace(comprobanteNumero) && !string.IsNullOrWhiteSpace(notaCreditoNumero)) return $"{comprobanteNumero} -> {notaCreditoNumero}";
        if (!string.IsNullOrWhiteSpace(notaCreditoNumero)) return notaCreditoNumero;
        if (!string.IsNullOrWhiteSpace(comprobanteNumero)) return comprobanteNumero;
        if (!string.IsNullOrWhiteSpace(compraDocumento)) return compraDocumento;
        return "-";
    }

    private static string OrigenDescripcion(OrigenMovimientoCaja origen) => origen switch
    {
        OrigenMovimientoCaja.COBRO_CLIENTE => "Cobro Cliente",
        OrigenMovimientoCaja.PAGO_PROVEEDOR => "Pago Proveedor",
        OrigenMovimientoCaja.GASTO => "Gasto",
        OrigenMovimientoCaja.INGRESO_MANUAL => "Ingreso Manual",
        OrigenMovimientoCaja.DEVOLUCION_CLIENTE => "Devolucion Cliente",
        OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR => "Devolucion Proveedor",
        OrigenMovimientoCaja.OTRO => "Otro",
        _ => origen.ToString()
    };

    private sealed record MovimientoCajaItem(MovimientoCajaListDto Dto, string SearchText);
}
