using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class IngresoRepository(ApplicationDbContext db) : IIngresoRepository
{
    public Task<PagedResult<IngresoListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Ingresos.AsNoTracking().Where(x => x.EmpresaId == empresaId);
        if (request.FechaDesde is not null) query = query.Where(x => x.Fecha.Date >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null) query = query.Where(x => x.Fecha.Date <= request.FechaHasta.Value.Date);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.TipoIngreso.Contains(term) || x.Descripcion.Contains(term) || x.MedioPago.Contains(term));
        }

        return query.OrderByDescending(x => x.Fecha)
            .Select(x => new IngresoListDto(x.IngresoId, x.Fecha, x.TipoIngreso, x.Descripcion, x.MedioPago, x.Importe, x.Estado))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<Ingreso?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.Ingresos
            .Include(x => x.MovimientoCaja)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.IngresoId == id, cancellationToken);

    public async Task<IReadOnlyList<CategoriaIngresoOptionDto>> ListarCategoriasAsync(int empresaId, CancellationToken cancellationToken) =>
        await db.CategoriasIngreso.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo)
            .OrderBy(x => x.Nombre)
            .Select(x => new CategoriaIngresoOptionDto(x.CategoriaIngresoId, x.Nombre))
            .ToListAsync(cancellationToken);

    public Task<CategoriaIngreso?> ObtenerCategoriaAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.CategoriasIngreso.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.CategoriaIngresoId == id && x.Estado == EstadoRegistro.Activo, cancellationToken);

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

    public async Task GuardarAsync(Ingreso ingreso, CancellationToken cancellationToken)
    {
        if (ingreso.IngresoId == 0) db.Ingresos.Add(ingreso);
        await db.SaveChangesAsync(cancellationToken);
    }
}
