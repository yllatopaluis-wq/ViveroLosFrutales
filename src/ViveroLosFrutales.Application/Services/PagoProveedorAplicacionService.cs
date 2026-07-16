using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class PagoProveedorAplicacionService(
    ICompraRepository compraRepository,
    IPagoProveedorRepository pagoProveedorRepository,
    IPagoProveedorAplicacionRepository aplicacionRepository,
    IOrdenCompraRepository ordenCompraRepository,
    IEmpresaContext empresaContext)
{

    public async Task<AplicarPagoProveedorFormDto> ObtenerFormularioAplicacionAsync(int compraId, CancellationToken cancellationToken)
    {
        var compra = await ObtenerCompraValidaAsync(compraId, cancellationToken);
        RecalcularCompra(compra);
        var disponibles = await ObtenerDisponiblesAsync(compraId, cancellationToken);
        return new AplicarPagoProveedorFormDto
        {
            CompraId = compra.CompraId,
            Compra = string.IsNullOrWhiteSpace(compra.Documento) ? $"{compra.Serie}-{compra.Numero}" : compra.Documento,
            Proveedor = compra.Proveedor?.RazonSocial ?? string.Empty,
            OrdenCompra = compra.OrdenCompra is null ? string.Empty : $"{compra.OrdenCompra.Serie}-{compra.OrdenCompra.Correlativo:000000}",
            TotalCompra = compra.Total,
            TotalPagado = compra.TotalPagado,
            SaldoPendiente = compra.SaldoPendiente,
            PagosDisponibles = disponibles,
            Pagos = disponibles.Select(x => new AplicarPagoProveedorItemDto
            {
                PagoProveedorId = x.PagoProveedorId,
                MontoAplicar = Math.Min(x.SaldoDisponible, compra.SaldoPendiente)
            }).ToList()
        };
    }
    public async Task AplicarAsync(AplicarPagoProveedorDto dto, CancellationToken cancellationToken)
    {
        if (dto.CompraId <= 0) throw new InvalidOperationException("Seleccione una compra.");
        await compraRepository.EjecutarEnTransaccionAsync(async () =>
        {
            var compra = await ObtenerCompraValidaAsync(dto.CompraId, cancellationToken);
            foreach (var item in dto.Pagos.Where(x => x.MontoAplicar > 0))
            {
                await AplicarPagoAsync(compra, item.PagoProveedorId, item.MontoAplicar, cancellationToken);
            }
            RecalcularCompra(compra);
            await compraRepository.GuardarAsync(compra, cancellationToken);
            await RecalcularOrdenSiCorrespondeAsync(compra.OrdenCompraId, cancellationToken);
        }, cancellationToken);
    }

    public async Task AplicarAutomaticamenteAsync(int compraId, CancellationToken cancellationToken)
    {
        await compraRepository.EjecutarEnTransaccionAsync(async () =>
        {
            var compra = await ObtenerCompraValidaAsync(compraId, cancellationToken);
            var saldoCompra = Math.Max(compra.Total - compra.PagoAplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado), 0);
            if (saldoCompra <= 0) throw new InvalidOperationException("La compra no tiene saldo pendiente.");
            var pagos = compra.OrdenCompraId.HasValue
                ? await pagoProveedorRepository.ListarPorOrdenCompraAsync(empresaContext.EmpresaId, compra.OrdenCompraId.Value, cancellationToken)
                : Array.Empty<PagoProveedor>();
            foreach (var pago in pagos.Where(x => x.EstadoPago == PagoProveedorEstado.ACTIVO).OrderBy(x => x.FechaPago).ThenBy(x => x.PagoProveedorId))
            {
                var disponible = SaldoDisponible(pago);
                if (disponible <= 0) continue;
                var monto = Math.Min(disponible, saldoCompra);
                await AplicarPagoAsync(compra, pago.PagoProveedorId, monto, cancellationToken);
                saldoCompra -= monto;
                if (saldoCompra <= 0) break;
            }
            RecalcularCompra(compra);
            await compraRepository.GuardarAsync(compra, cancellationToken);
            await RecalcularOrdenSiCorrespondeAsync(compra.OrdenCompraId, cancellationToken);
        }, cancellationToken);
    }

    public async Task AnularAplicacionAsync(int aplicacionId, string motivo, CancellationToken cancellationToken)
    {
        motivo = (motivo ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(motivo)) throw new InvalidOperationException("Ingrese el motivo de anulacion de la aplicacion.");
        await compraRepository.EjecutarEnTransaccionAsync(async () =>
        {
            var aplicacion = await aplicacionRepository.ObtenerAsync(empresaContext.EmpresaId, aplicacionId, cancellationToken)
                ?? throw new InvalidOperationException("Aplicacion no encontrada.");
            if (aplicacion.Estado == EstadoPagoProveedorAplicacion.ANULADO) return;
            aplicacion.Estado = EstadoPagoProveedorAplicacion.ANULADO;
            aplicacion.MotivoAnulacion = motivo;
            aplicacion.FechaAnulacion = DateTime.UtcNow;
            aplicacion.UsuarioAnulacion = empresaContext.UsuarioNombre;
            await aplicacionRepository.GuardarAsync(aplicacion, cancellationToken);
            var compra = await compraRepository.ObtenerAsync(empresaContext.EmpresaId, aplicacion.CompraId, cancellationToken)
                ?? throw new InvalidOperationException("Compra no encontrada.");
            RecalcularCompra(compra);
            await compraRepository.GuardarAsync(compra, cancellationToken);
            await RecalcularOrdenSiCorrespondeAsync(compra.OrdenCompraId, cancellationToken);
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<PagoProveedorDisponibleDto>> ObtenerDisponiblesAsync(int compraId, CancellationToken cancellationToken)
    {
        var compra = await ObtenerCompraValidaAsync(compraId, cancellationToken);
        if (!compra.OrdenCompraId.HasValue) return Array.Empty<PagoProveedorDisponibleDto>();
        var pagos = await pagoProveedorRepository.ListarPorOrdenCompraAsync(empresaContext.EmpresaId, compra.OrdenCompraId.Value, cancellationToken);
        return pagos.Where(x => x.EstadoPago == PagoProveedorEstado.ACTIVO)
            .Select(x => new PagoProveedorDisponibleDto(x.PagoProveedorId, x.FechaPago, $"Pago {x.PagoProveedorId}", x.Monto, Aplicado(x), 0, 0, SaldoDisponible(x)))
            .Where(x => x.SaldoDisponible > 0)
            .ToList();
    }

    private async Task<Compra> ObtenerCompraValidaAsync(int compraId, CancellationToken cancellationToken)
    {
        var compra = await compraRepository.ObtenerAsync(empresaContext.EmpresaId, compraId, cancellationToken)
            ?? throw new InvalidOperationException("Compra no encontrada.");
        if (compra.EstadoDocumento == EstadoDocumentoCompra.ANULADO) throw new InvalidOperationException("No se puede aplicar pagos a una compra anulada.");
        return compra;
    }

    private async Task AplicarPagoAsync(Compra compra, int pagoProveedorId, decimal monto, CancellationToken cancellationToken)
    {
        if (monto <= 0) throw new InvalidOperationException("El monto a aplicar debe ser mayor a cero.");
        var pago = await pagoProveedorRepository.ObtenerAsync(empresaContext.EmpresaId, pagoProveedorId, cancellationToken)
            ?? throw new InvalidOperationException("Pago proveedor no encontrado.");
        if (pago.EstadoPago == PagoProveedorEstado.ANULADO) throw new InvalidOperationException("No se puede aplicar un pago anulado.");
        if (pago.ProveedorId != compra.ProveedorId) throw new InvalidOperationException("El pago y la compra pertenecen a proveedores diferentes.");
        if (pago.OrdenCompraId.HasValue && pago.OrdenCompraId != compra.OrdenCompraId) throw new InvalidOperationException("El pago pertenece a otra orden de compra.");
        var disponible = SaldoDisponible(pago);
        if (monto > disponible) throw new InvalidOperationException("El monto a aplicar supera el saldo disponible del pago.");
        var saldoCompra = Math.Max(compra.Total - compra.PagoAplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado), 0);
        if (monto > saldoCompra) throw new InvalidOperationException("El monto a aplicar supera el saldo pendiente de la compra.");
        var aplicacion = new PagoProveedorAplicacion
        {
            EmpresaId = empresaContext.EmpresaId,
            PagoProveedorId = pago.PagoProveedorId,
            CompraId = compra.CompraId,
            MontoAplicado = decimal.Round(monto, 2),
            FechaAplicacion = DateTime.UtcNow,
            UsuarioRegistro = empresaContext.UsuarioNombre
        };
        compra.PagoAplicaciones.Add(aplicacion);
        pago.Aplicaciones.Add(aplicacion);
        await aplicacionRepository.GuardarAsync(aplicacion, cancellationToken);
    }

    private static decimal Aplicado(PagoProveedor pago) => decimal.Round(pago.Aplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado), 2);
    private static decimal SaldoDisponible(PagoProveedor pago) => Math.Max(decimal.Round(pago.Monto - Aplicado(pago), 2), 0);

    private static void RecalcularCompra(Compra compra)
    {
        compra.TotalPagado = decimal.Round(compra.PagoAplicaciones.Where(x => x.Estado == EstadoPagoProveedorAplicacion.ACTIVO).Sum(x => x.MontoAplicado), 2);
        compra.SaldoPendiente = Math.Max(decimal.Round(compra.Total - compra.TotalPagado, 2), 0);
        compra.EstadoPago = compra.TotalPagado <= 0 ? EstadoPagoCompra.PENDIENTE
            : compra.TotalPagado >= compra.Total ? EstadoPagoCompra.PAGADO
            : EstadoPagoCompra.PARCIAL;
    }

    private async Task RecalcularOrdenSiCorrespondeAsync(int? ordenCompraId, CancellationToken cancellationToken)
    {
        if (!ordenCompraId.HasValue) return;
        var orden = await ordenCompraRepository.ObtenerAsync(empresaContext.EmpresaId, ordenCompraId.Value, cancellationToken);
        if (orden is null) return;
        OrdenCompraService.RecalcularEstados(orden);
        await ordenCompraRepository.GuardarAsync(orden, cancellationToken);
    }
}
