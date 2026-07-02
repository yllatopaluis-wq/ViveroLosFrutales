using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class TransferenciaFinancieraRepository(ApplicationDbContext db) : ITransferenciaFinancieraRepository
{
    public Task<PagedResult<TransferenciaListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.TransferenciasFinancieras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId);

        if (request.FechaDesde is not null) query = query.Where(x => x.Fecha.Date >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null) query = query.Where(x => x.Fecha.Date <= request.FechaHasta.Value.Date);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.CuentaOrigen!.Nombre.Contains(term) || x.CuentaDestino!.Nombre.Contains(term) || x.Observacion.Contains(term));
        }

        return query.OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.TransferenciaFinancieraId)
            .Select(x => new TransferenciaListDto(
                x.TransferenciaFinancieraId,
                x.Fecha,
                x.CuentaOrigen!.Nombre,
                x.CuentaDestino!.Nombre,
                x.Monto,
                x.Observacion,
                x.Estado))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<TransferenciaFinanciera?> ObtenerAsync(int empresaId, int transferenciaFinancieraId, CancellationToken cancellationToken) =>
        db.TransferenciasFinancieras
            .Include(x => x.MovimientoEgreso)
            .Include(x => x.MovimientoIngreso)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.TransferenciaFinancieraId == transferenciaFinancieraId, cancellationToken);

    public async Task<IReadOnlyList<CuentaFinancieraOptionDto>> ListarCuentasActivasAsync(int empresaId, CancellationToken cancellationToken) =>
        await db.CuentasFinancieras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Activo)
            .OrderBy(x => x.Tipo)
            .ThenBy(x => x.Nombre)
            .Select(x => new CuentaFinancieraOptionDto(x.CuentaFinancieraId, x.Nombre, x.Tipo, x.Moneda))
            .ToListAsync(cancellationToken);

    public Task<CuentaFinanciera?> ObtenerCuentaActivaAsync(int empresaId, int cuentaFinancieraId, CancellationToken cancellationToken) =>
        db.CuentasFinancieras.AsNoTracking()
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.CuentaFinancieraId == cuentaFinancieraId && x.Activo, cancellationToken);

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

    public async Task GuardarAsync(TransferenciaFinanciera transferencia, CancellationToken cancellationToken)
    {
        if (transferencia.TransferenciaFinancieraId == 0) db.TransferenciasFinancieras.Add(transferencia);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task GuardarMovimientoAsync(MovimientoCaja movimiento, CancellationToken cancellationToken)
    {
        if (movimiento.MovimientoCajaId == 0) db.MovimientosCaja.Add(movimiento);
        await db.SaveChangesAsync(cancellationToken);
    }
}
