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
            .Include(x => x.Detalles).ThenInclude(x => x.Producto)
            .Include(x => x.Pagos).ThenInclude(x => x.CuentaFinanciera)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.CompraId == id, cancellationToken);

    public Task<bool> ExisteDocumentoAsync(int empresaId, int proveedorId, TipoDocumentoCompra tipoDocumento, string serie, string numero, int? excluirCompraId, CancellationToken cancellationToken)
    {
        serie = serie?.Trim() ?? string.Empty;
        numero = numero?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(serie) || string.IsNullOrWhiteSpace(numero)) return Task.FromResult(false);

        return db.Compras.AsNoTracking().AnyAsync(x => x.EmpresaId == empresaId
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
            producto.Stock += detalle.Cantidad;
            db.MovimientosInventario.Add(new MovimientoInventario
            {
                EmpresaId = compra.EmpresaId,
                ProductoId = producto.ProductoId,
                Tipo = TipoMovimientoInventario.ENTRADA_COMPRA,
                Fecha = compra.Fecha,
                Cantidad = detalle.Cantidad,
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
            if (producto.Stock < detalle.Cantidad)
            {
                throw new InvalidOperationException("No se puede anular la compra porque parte del stock ya fue vendido o consumido.");
            }

            var stockAnterior = producto.Stock;
            producto.Stock -= detalle.Cantidad;
            db.MovimientosInventario.Add(new MovimientoInventario
            {
                EmpresaId = compra.EmpresaId,
                ProductoId = producto.ProductoId,
                Tipo = TipoMovimientoInventario.REVERSA_COMPRA,
                Fecha = PeruDateTime.Today,
                Cantidad = -detalle.Cantidad,
                StockAnterior = stockAnterior,
                StockNuevo = producto.Stock,
                Referencia = Documento(compra),
                UsuarioRegistro = compra.UsuarioAnulacion
            });
        }
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
                ? (x.Serie == string.Empty || x.Numero == string.Empty ? "-" : x.Serie + "-" + x.Numero)
                : x.Documento,
            x.SubTotal,
            x.Igv,
            x.Total,
            x.TotalPagado,
            x.SaldoPendiente,
            x.EstadoPago,
            x.EstadoDocumento,
            x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO && x.EstadoPago != EstadoPagoCompra.PAGADO && x.SaldoPendiente > 0,
            x.EstadoDocumento == EstadoDocumentoCompra.ACTIVO));

    private static string Documento(Compra compra) =>
        string.IsNullOrWhiteSpace(compra.Documento)
            ? (string.IsNullOrWhiteSpace(compra.Serie) || string.IsNullOrWhiteSpace(compra.Numero) ? compra.TipoDocumento.ToString() : $"{compra.Serie}-{compra.Numero}")
            : compra.Documento;
}

