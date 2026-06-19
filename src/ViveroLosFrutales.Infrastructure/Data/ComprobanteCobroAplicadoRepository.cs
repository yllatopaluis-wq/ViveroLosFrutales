using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Infrastructure.Data;

public class ComprobanteCobroAplicadoRepository(ApplicationDbContext db) : IComprobanteCobroAplicadoRepository
{
    public async Task<IReadOnlyList<ComprobanteCobroAplicado>> ListarPorComprobanteAsync(int empresaId, int comprobanteId, CancellationToken cancellationToken) =>
        await db.ComprobanteCobrosAplicados.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.ComprobanteId == comprobanteId)
            .ToListAsync(cancellationToken);

    public async Task GuardarAsync(ComprobanteCobroAplicado aplicacion, CancellationToken cancellationToken)
    {
        if (aplicacion.ComprobanteCobroAplicadoId == 0) db.ComprobanteCobrosAplicados.Add(aplicacion);
        await db.SaveChangesAsync(cancellationToken);
    }
}
