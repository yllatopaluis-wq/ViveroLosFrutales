using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Infrastructure.Data;

public class PagoProveedorRepository(ApplicationDbContext db) : IPagoProveedorRepository
{
    public Task<PagoProveedor?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.PagosProveedor
            .Include(x => x.Compra)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.PagoProveedorId == id, cancellationToken);

    public async Task GuardarAsync(PagoProveedor pago, CancellationToken cancellationToken)
    {
        if (pago.PagoProveedorId == 0) db.PagosProveedor.Add(pago);
        await db.SaveChangesAsync(cancellationToken);
    }
}
