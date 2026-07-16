using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class OrdenCompraRepository(ApplicationDbContext db) : IOrdenCompraRepository
{
    public Task<PagedResult<OrdenCompraListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.OrdenesCompra.AsNoTracking().Where(x => x.EmpresaId == empresaId);
        if (request.FechaDesde is not null) query = query.Where(x => x.Fecha >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.Fecha < hasta);
        }
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.Serie.Contains(term)
                || x.Correlativo.ToString().Contains(term)
                || x.ProveedorRazonSocial.Contains(term)
                || x.ProveedorNumeroDocumento.Contains(term)
                || x.Proveedor!.RazonSocial.Contains(term)
                || x.Proveedor.NumeroDocumento.Contains(term));
        }

        return query.OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.OrdenCompraId)
            .Select(x => new OrdenCompraListDto(
                x.OrdenCompraId,
                x.Fecha,
                x.Serie + "-" + x.Correlativo.ToString("000000"),
                x.ProveedorRazonSocial == string.Empty ? x.Proveedor!.RazonSocial : x.ProveedorRazonSocial,
                x.Total,
                x.TotalFacturado,
                x.TotalPagado,
                x.TotalAplicado,
                x.SaldoDisponible,
                x.PendienteFacturar,
                x.EstadoFacturacion,
                x.EstadoFinanciero,
                x.EstadoDocumento,
                x.EstadoDocumento == EstadoDocumentoOrdenCompra.ACTIVO,
                x.EstadoDocumento == EstadoDocumentoOrdenCompra.ACTIVO,
                x.EstadoDocumento == EstadoDocumentoOrdenCompra.ACTIVO && x.SaldoDisponible > 0,
                x.EstadoDocumento == EstadoDocumentoOrdenCompra.ACTIVO && x.SaldoDisponible > 0,
                x.EstadoDocumento == EstadoDocumentoOrdenCompra.ACTIVO,
                x.EstadoDocumento == EstadoDocumentoOrdenCompra.ACTIVO))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<OrdenCompra?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.OrdenesCompra
            .Include(x => x.Proveedor)
            .Include(x => x.Detalles).ThenInclude(x => x.Producto)
            .Include(x => x.Compras).ThenInclude(x => x.Proveedor)
            .Include(x => x.Pagos).ThenInclude(x => x.CuentaFinanciera)
            .Include(x => x.Pagos).ThenInclude(x => x.Aplicaciones).ThenInclude(x => x.Compra)
            .Include(x => x.Devoluciones)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.OrdenCompraId == id, cancellationToken);

    public async Task<int> SiguienteCorrelativoAsync(int empresaId, string serie, CancellationToken cancellationToken)
    {
        var ultimo = await db.OrdenesCompra.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Serie == serie)
            .MaxAsync(x => (int?)x.Correlativo, cancellationToken);
        return (ultimo ?? 0) + 1;
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

    public async Task GuardarAsync(OrdenCompra ordenCompra, CancellationToken cancellationToken)
    {
        if (ordenCompra.OrdenCompraId == 0) db.OrdenesCompra.Add(ordenCompra);
        await db.SaveChangesAsync(cancellationToken);
    }
}
