using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class GastoRepository(ApplicationDbContext db) : IGastoRepository
{
    public Task<PagedResult<GastoListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Gastos.AsNoTracking().Include(x => x.CuentaFinanciera).Where(x => x.EmpresaId == empresaId);
        if (request.FechaDesde is not null) query = query.Where(x => x.Fecha.Date >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null) query = query.Where(x => x.Fecha.Date <= request.FechaHasta.Value.Date);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.Categoria.Contains(term) || x.Descripcion.Contains(term) || x.MedioPago.Contains(term));
        }

        return query.OrderByDescending(x => x.Fecha)
            .Select(x => new GastoListDto(x.GastoId, x.Fecha, x.Categoria, x.Descripcion, x.Importe, x.MedioPago, x.CuentaFinanciera == null ? string.Empty : x.CuentaFinanciera.Nombre, x.Estado))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<Gasto?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.Gastos
            .Include(x => x.MovimientoCaja)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.GastoId == id, cancellationToken);

    public async Task<IReadOnlyList<CategoriaGastoOptionDto>> ListarCategoriasAsync(int empresaId, CancellationToken cancellationToken) =>
        await db.CategoriasGasto.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo)
            .OrderBy(x => x.Nombre)
            .Select(x => new CategoriaGastoOptionDto(x.CategoriaGastoId, x.Nombre))
            .ToListAsync(cancellationToken);

    public Task<CategoriaGasto?> ObtenerCategoriaAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.CategoriasGasto.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.CategoriaGastoId == id && x.Estado == EstadoRegistro.Activo, cancellationToken);

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

    public async Task GuardarAsync(Gasto gasto, CancellationToken cancellationToken)
    {
        if (gasto.GastoId == 0) db.Gastos.Add(gasto);
        await db.SaveChangesAsync(cancellationToken);
    }
}

