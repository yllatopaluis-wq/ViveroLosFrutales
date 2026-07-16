using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Infrastructure.Data;

public class PagoProveedorAplicacionRepository(ApplicationDbContext db) : IPagoProveedorAplicacionRepository
{
    public async Task<IReadOnlyList<PagoProveedorAplicacion>> ListarPorCompraAsync(int empresaId, int compraId, CancellationToken cancellationToken) =>
        await db.PagoProveedorAplicaciones
            .Include(x => x.PagoProveedor)
            .Where(x => x.EmpresaId == empresaId && x.CompraId == compraId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PagoProveedorAplicacion>> ListarPorPagoAsync(int empresaId, int pagoProveedorId, CancellationToken cancellationToken) =>
        await db.PagoProveedorAplicaciones
            .Include(x => x.Compra)
            .Where(x => x.EmpresaId == empresaId && x.PagoProveedorId == pagoProveedorId)
            .ToListAsync(cancellationToken);

    public Task<PagoProveedorAplicacion?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.PagoProveedorAplicaciones
            .Include(x => x.PagoProveedor)
            .Include(x => x.Compra)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.PagoProveedorAplicacionId == id, cancellationToken);

    public async Task GuardarAsync(PagoProveedorAplicacion aplicacion, CancellationToken cancellationToken)
    {
        if (aplicacion.PagoProveedorAplicacionId == 0) db.PagoProveedorAplicaciones.Add(aplicacion);
        await db.SaveChangesAsync(cancellationToken);
    }
}
