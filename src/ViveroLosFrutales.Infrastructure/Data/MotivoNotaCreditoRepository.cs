using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class MotivoNotaCreditoRepository(ApplicationDbContext db) : IMotivoNotaCreditoRepository
{
    public async Task<IReadOnlyList<MotivoNotaCreditoOptionDto>> ListarActivosAsync(CancellationToken cancellationToken) =>
        await db.MotivosNotaCredito.AsNoTracking()
            .Where(x => x.Estado == EstadoRegistro.Activo)
            .OrderBy(x => x.Nombre)
            .Select(x => new MotivoNotaCreditoOptionDto(x.MotivoNotaCreditoId, x.Nombre))
            .ToListAsync(cancellationToken);

    public Task<MotivoNotaCredito?> ObtenerAsync(int id, CancellationToken cancellationToken) =>
        db.MotivosNotaCredito.AsNoTracking()
            .FirstOrDefaultAsync(x => x.MotivoNotaCreditoId == id && x.Estado == EstadoRegistro.Activo, cancellationToken);
}
