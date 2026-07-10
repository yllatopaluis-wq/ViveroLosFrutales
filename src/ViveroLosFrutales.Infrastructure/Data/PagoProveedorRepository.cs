using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class PagoProveedorRepository(ApplicationDbContext db) : IPagoProveedorRepository
{
    public Task<PagedResult<PagoProveedorTesoreriaListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.PagosProveedor.AsNoTracking()
            .Include(x => x.Proveedor)
            .Include(x => x.Compra)
            .Include(x => x.CuentaFinanciera)
            .Where(x => x.EmpresaId == empresaId);

        if (request.FechaDesde is not null) query = query.Where(x => x.FechaPago >= request.FechaDesde.Value.Date);
        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.FechaPago < hasta);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x =>
                x.Proveedor!.RazonSocial.Contains(term)
                || x.Proveedor.NumeroDocumento.Contains(term)
                || x.Compra!.Documento.Contains(term)
                || x.Compra.Serie.Contains(term)
                || x.Compra.Numero.Contains(term)
                || x.MedioPago.Contains(term)
                || x.Observacion.Contains(term));
        }

        return query
            .OrderByDescending(x => x.FechaPago)
            .ThenByDescending(x => x.PagoProveedorId)
            .Select(x => new PagoProveedorTesoreriaListDto(
                x.PagoProveedorId,
                x.CompraId,
                x.FechaPago,
                x.Proveedor!.RazonSocial,
                x.Compra!.Documento == string.Empty
                    ? (x.Compra.Serie == string.Empty || x.Compra.Numero == string.Empty
                        ? x.Compra.TipoDocumento == TipoDocumentoCompra.FACTURA ? "FACTURA"
                            : x.Compra.TipoDocumento == TipoDocumentoCompra.BOLETA ? "BOLETA"
                            : x.Compra.TipoDocumento == TipoDocumentoCompra.LIQUIDACION_COMPRA ? "LIQUIDACION COMPRA"
                            : x.Compra.TipoDocumento == TipoDocumentoCompra.RECIBO ? "RECIBO"
                            : x.Compra.TipoDocumento == TipoDocumentoCompra.NOTA_VENTA ? "NOTA VENTA"
                            : x.Compra.TipoDocumento == TipoDocumentoCompra.PENDIENTE_COMPROBANTE ? "PENDIENTE COMPROBANTE"
                            : x.Compra.TipoDocumento == TipoDocumentoCompra.SIN_DOCUMENTO ? "SIN DOCUMENTO"
                            : string.Empty
                        : x.Compra.Serie + "-" + x.Compra.Numero)
                    : x.Compra.Documento,
                x.Monto,
                x.MedioPago,
                x.CuentaFinanciera == null ? "Caja principal" : x.CuentaFinanciera.Nombre,
                x.EstadoPago,
                x.Observacion,
                x.EstadoPago == PagoProveedorEstado.ACTIVO && x.Compra!.EstadoDocumento == EstadoDocumentoCompra.ACTIVO))
            .ToPagedAsync(request, cancellationToken);
    }

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



