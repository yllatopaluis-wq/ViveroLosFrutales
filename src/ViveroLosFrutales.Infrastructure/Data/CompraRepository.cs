using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class CompraRepository(ApplicationDbContext db) : ICompraRepository
{
    public Task<PagedResult<CompraListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = Filtrar(db.Compras.AsNoTracking().Where(x => x.EmpresaId == empresaId), request)
            .OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.CompraId);

        return Proyectar(query)
            .ToPagedAsync(request, cancellationToken);
    }

    public async Task<IReadOnlyList<CompraListDto>> BuscarCuentasPorPagarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = Filtrar(db.Compras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO
                && x.SaldoPendiente > 0), request);

        query = query
            .OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.CompraId);

        return await Proyectar(query)
            .Take(200)
            .ToListAsync(cancellationToken);
    }

    public Task<Compra?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.Compras
            .Include(x => x.Proveedor)
            .Include(x => x.OrdenCompra)
            .Include(x => x.Detalles).ThenInclude(x => x.Producto)
            .Include(x => x.Pagos).ThenInclude(x => x.CuentaFinanciera)
            .Include(x => x.PagoAplicaciones).ThenInclude(x => x.PagoProveedor)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.CompraId == id, cancellationToken);


    public async Task<IReadOnlyList<CompraListDto>> ListarPorOrdenCompraAsync(int empresaId, int ordenCompraId, CancellationToken cancellationToken)
    {
        var query = db.Compras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.OrdenCompraId == ordenCompraId)
            .OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.CompraId);

        return await Proyectar(query).ToListAsync(cancellationToken);
    }
    public Task<bool> ExisteDocumentoAsync(int empresaId, int proveedorId, TipoDocumentoCompra tipoDocumento, string serie, string numero, int? excluirCompraId, CancellationToken cancellationToken)
    {
        serie = serie?.Trim() ?? string.Empty;
        numero = numero?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(serie) || string.IsNullOrWhiteSpace(numero)) return Task.FromResult(false);

        return db.Compras.AsNoTracking().AnyAsync(x => x.EmpresaId == empresaId
            && x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO
            && x.ProveedorId == proveedorId
            && x.TipoDocumento == tipoDocumento
            && x.Serie == serie
            && x.Numero == numero
            && (!excluirCompraId.HasValue || x.CompraId != excluirCompraId.Value), cancellationToken);
    }

    public async Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken)
    {
        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
            await operacion();
            await tx.CommitAsync(cancellationToken);
        });
    }

    public async Task AumentarStockAsync(Compra compra, CancellationToken cancellationToken)
    {
        foreach (var detalle in compra.Detalles)
        {
            var producto = await db.Productos.FirstAsync(x => x.EmpresaId == compra.EmpresaId && x.ProductoId == detalle.ProductoId, cancellationToken);
            var stockAnterior = producto.Stock;
            if (detalle.CantidadRecibida <= 0) continue;
            producto.Stock += detalle.CantidadRecibida;
            db.MovimientosInventario.Add(new MovimientoInventario
            {
                EmpresaId = compra.EmpresaId,
                ProductoId = producto.ProductoId,
                Tipo = TipoMovimientoInventario.ENTRADA_COMPRA,
                Fecha = compra.Fecha,
                Cantidad = detalle.CantidadRecibida,
                StockAnterior = stockAnterior,
                StockNuevo = producto.Stock,
                Referencia = Documento(compra),
                UsuarioRegistro = compra.UsuarioRegistro
            });
        }
    }

    public async Task RevertirStockAsync(Compra compra, CancellationToken cancellationToken)
    {
        foreach (var detalle in compra.Detalles)
        {
            var producto = await db.Productos.FirstAsync(x => x.EmpresaId == compra.EmpresaId && x.ProductoId == detalle.ProductoId, cancellationToken);
            if (detalle.CantidadRecibida <= 0) continue;
            if (producto.Stock < detalle.CantidadRecibida)
            {
                throw new InvalidOperationException("No se puede anular la compra porque parte del stock ya fue vendido o consumido.");
            }

            var stockAnterior = producto.Stock;
            producto.Stock -= detalle.CantidadRecibida;
            db.MovimientosInventario.Add(new MovimientoInventario
            {
                EmpresaId = compra.EmpresaId,
                ProductoId = producto.ProductoId,
                Tipo = TipoMovimientoInventario.REVERSA_COMPRA,
                Fecha = PeruDateTime.Today,
                Cantidad = -detalle.CantidadRecibida,
                StockAnterior = stockAnterior,
                StockNuevo = producto.Stock,
                Referencia = Documento(compra),
                UsuarioRegistro = compra.UsuarioAnulacion
            });
        }
    }

    public Task<bool> TieneMovimientosInventarioActivosAsync(Compra compra, CancellationToken cancellationToken)
    {
        var productoIds = compra.Detalles.Select(x => x.ProductoId).Distinct().ToArray();
        var referencia = Documento(compra);
        return db.MovimientosInventario.AnyAsync(x =>
            x.EmpresaId == compra.EmpresaId
            && x.Estado == EstadoRegistro.Activo
            && x.Tipo == TipoMovimientoInventario.ENTRADA_COMPRA
            && x.Referencia == referencia
            && productoIds.Contains(x.ProductoId), cancellationToken);
    }
    public Task EliminarDetallesAsync(Compra compra, CancellationToken cancellationToken)
    {
        db.CompraDetalles.RemoveRange(compra.Detalles);
        compra.Detalles.Clear();
        return Task.CompletedTask;
    }
    public async Task GuardarAsync(Compra compra, CancellationToken cancellationToken)
    {
        if (compra.CompraId == 0) db.Compras.Add(compra);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Compra> Filtrar(IQueryable<Compra> query, SearchRequest request)
    {
        if (request.FechaDesde is not null) query = query.Where(x => x.Fecha >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.Fecha < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x =>
                x.Documento.Contains(term)
                || x.Serie.Contains(term)
                || x.Numero.Contains(term)
                || x.Proveedor!.RazonSocial.Contains(term)
                || x.Proveedor.NumeroDocumento.Contains(term));
        }

        return query;
    }

    private static IQueryable<CompraListDto> Proyectar(IQueryable<Compra> query) =>
        query.Select(x => new CompraListDto(
            x.CompraId,
            x.Fecha,
            x.Proveedor!.RazonSocial,
            x.Documento == string.Empty
                ? (x.Serie == string.Empty || x.Numero == string.Empty
                    ? x.TipoDocumento == TipoDocumentoCompra.FACTURA ? "FACTURA"
                        : x.TipoDocumento == TipoDocumentoCompra.BOLETA ? "BOLETA"
                        : x.TipoDocumento == TipoDocumentoCompra.LIQUIDACION_COMPRA ? "LIQUIDACION COMPRA"
                        : x.TipoDocumento == TipoDocumentoCompra.RECIBO ? "RECIBO"
                        : x.TipoDocumento == TipoDocumentoCompra.NOTA_VENTA ? "NOTA VENTA"
                        : x.TipoDocumento == TipoDocumentoCompra.PENDIENTE_COMPROBANTE ? "PENDIENTE COMPROBANTE"
                        : x.TipoDocumento == TipoDocumentoCompra.SIN_DOCUMENTO ? "SIN DOCUMENTO"
                        : string.Empty
                    : x.Serie + "-" + x.Numero)
                : x.Documento,
            x.SubTotal,
            x.Igv,
            x.Total,
            x.TotalPagado,
            x.SaldoPendiente,
            x.EstadoPago,
            x.EstadoEntrega,
            x.EstadoDocumento,
            x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO && x.EstadoPago != EstadoPagoCompra.PAGADO && decimal.Round(x.SaldoPendiente, 2) > 0,
            x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO));

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

    private static string Documento(Compra compra) =>
        string.IsNullOrWhiteSpace(compra.Documento)
            ? (string.IsNullOrWhiteSpace(compra.Serie) || string.IsNullOrWhiteSpace(compra.Numero) ? TipoDocumentoEtiqueta(compra.TipoDocumento) : $"{compra.Serie}-{compra.Numero}")
            : compra.Documento;
}




