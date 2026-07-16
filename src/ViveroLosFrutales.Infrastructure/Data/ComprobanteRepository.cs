using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class ComprobanteRepository(ApplicationDbContext db) : IComprobanteRepository
{
    public Task<PagedResult<ComprobanteListDto>> BuscarAsync(int empresaId, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Comprobantes.AsNoTracking().Where(x => x.EmpresaId == empresaId && x.TipoComprobante != TipoComprobante.COT && x.TipoComprobante != TipoComprobante.NCR);
        return BuscarAsync(query, request, cancellationToken);
    }

    public Task<PagedResult<ComprobanteListDto>> BuscarPorTipoAsync(int empresaId, TipoComprobante tipo, SearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Comprobantes.AsNoTracking().Where(x => x.EmpresaId == empresaId && x.TipoComprobante == tipo);
        return BuscarAsync(query, request, cancellationToken);
    }

    public Task<PagedResult<NotaCreditoOrigenDto>> BuscarOrigenesNotaCreditoAsync(int empresaId, NotaCreditoOrigenSearchRequest request, CancellationToken cancellationToken)
    {
        var query = db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && (x.TipoComprobante == TipoComprobante.BOL || x.TipoComprobante == TipoComprobante.FAC)
                && x.Estado == EstadoRegistro.Activo
                && x.EstadoSunat == EstadoSunat.Aceptado
                && !db.Comprobantes.Any(nc => nc.EmpresaId == empresaId
                    && nc.TipoComprobante == TipoComprobante.NCR
                    && nc.Estado == EstadoRegistro.Activo
                    && nc.ComprobanteReferenciaId == x.ComprobanteId));

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            var parts = term.Split('-', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var correlativo = 0;
            var hasNumero = parts.Length == 2 && int.TryParse(parts[1], out correlativo);
            var serie = hasNumero ? parts[0] : string.Empty;

            query = query.Where(x =>
                x.Serie.Contains(term)
                || (x.Serie + "-" + x.Correlativo).Contains(term)
                || (hasNumero && x.Serie == serie && x.Correlativo == correlativo)
                || (x.ClienteNombre ?? string.Empty).Contains(term)
                || (x.ClienteNumeroDocumento ?? string.Empty).Contains(term)
                || x.Cliente!.NombreCompleto.Contains(term)
                || x.Cliente.NumeroDocumento.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(request.Serie))
        {
            var serie = request.Serie.Trim();
            query = query.Where(x => x.Serie.Contains(serie));
        }

        if (request.Correlativo is int correlativoFiltro)
        {
            query = query.Where(x => x.Correlativo == correlativoFiltro);
        }

        if (!string.IsNullOrWhiteSpace(request.Cliente))
        {
            var cliente = request.Cliente.Trim();
            query = query.Where(x => (x.ClienteNombre ?? string.Empty).Contains(cliente)
                || (x.ClienteNumeroDocumento ?? string.Empty).Contains(cliente)
                || x.Cliente!.NombreCompleto.Contains(cliente)
                || x.Cliente.NumeroDocumento.Contains(cliente));
        }

        if (request.TipoComprobante is TipoComprobante tipo)
        {
            query = query.Where(x => x.TipoComprobante == tipo);
        }

        if (request.FechaDesde is not null)
        {
            var desde = request.FechaDesde.Value.Date;
            query = query.Where(x => x.FechaEmision >= desde);
        }

        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.FechaEmision < hasta);
        }

        return query.OrderByDescending(x => x.FechaEmision)
            .ThenByDescending(x => x.ComprobanteId)
            .Select(x => new NotaCreditoOrigenDto(
                x.ComprobanteId,
                x.TipoComprobante,
                x.Serie,
                x.Correlativo,
                x.Serie + "-" + x.Correlativo.ToString(),
                x.FechaEmision,
                x.ClienteNombre != null && x.ClienteNombre != string.Empty ? x.ClienteNombre : x.Cliente!.NombreCompleto,
                x.ClienteNumeroDocumento != null && x.ClienteNumeroDocumento != string.Empty ? x.ClienteNumeroDocumento : x.Cliente!.NumeroDocumento,
                x.Total,
                x.EstadoSunat))
            .ToPagedAsync(request, cancellationToken);
    }

    private Task<PagedResult<ComprobanteListDto>> BuscarAsync(IQueryable<Comprobante> query, SearchRequest request, CancellationToken cancellationToken)
    {
        var hoy = PeruDateTime.Today;
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            var parts = term.Split('-', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var correlativo = 0;
            var hasNumero = parts.Length == 2 && int.TryParse(parts[1], out correlativo);
            var serie = hasNumero ? parts[0] : string.Empty;

            query = query.Where(x =>
                x.Serie.Contains(term)
                || (x.Serie + "-" + x.Correlativo).Contains(term)
                || (hasNumero && x.Serie == serie && x.Correlativo == correlativo)
                || (x.ClienteNombre ?? string.Empty).Contains(term)
                || (x.ClienteNumeroDocumento ?? string.Empty).Contains(term)
                || x.Cliente!.NombreCompleto.Contains(term)
                || x.Cliente.NumeroDocumento.Contains(term));
        }

        if (request.FechaDesde is not null)
        {
            var desde = request.FechaDesde.Value.Date;
            query = query.Where(x => x.FechaEmision >= desde);
        }

        if (request.FechaHasta is not null)
        {
            var hasta = request.FechaHasta.Value.Date.AddDays(1);
            query = query.Where(x => x.FechaEmision < hasta);
        }

        if (request.Estado is int estado && Enum.IsDefined(typeof(EstadoRegistro), estado))
        {
            query = query.Where(x => x.Estado == (EstadoRegistro)estado);
        }

        return query.OrderByDescending(x => x.FechaEmision)
            .Select(x => new
            {
                Comprobante = x,
                TotalPagado = x.Cobros
                    .Where(c => c.Estado == CobroClienteEstado.ACTIVO
                        && !x.CobrosAplicados.Any(a => a.CobroClienteId == c.CobroClienteId))
                    .Sum(c => (decimal?)c.Monto) ?? 0,
                TotalAplicado = x.CobrosAplicados
                    .Where(c => c.CobroCliente != null && c.CobroCliente.Estado == CobroClienteEstado.ACTIVO)
                    .Sum(c => (decimal?)c.MontoAplicado) ?? 0,
                TotalNotasCredito = db.Comprobantes
                    .Where(nc => nc.EmpresaId == x.EmpresaId
                        && nc.TipoComprobante == TipoComprobante.NCR
                        && nc.Estado == EstadoRegistro.Activo
                        && nc.ComprobanteReferenciaId == x.ComprobanteId)
                    .Sum(nc => (decimal?)nc.Total) ?? 0
            })
            .Select(x => new ComprobanteListDto(
                x.Comprobante.ComprobanteId,
                x.Comprobante.TipoComprobante,
                x.Comprobante.Serie,
                x.Comprobante.Correlativo,
                x.Comprobante.FechaEmision,
                x.Comprobante.ClienteNombre != null && x.Comprobante.ClienteNombre != string.Empty ? x.Comprobante.ClienteNombre : x.Comprobante.Cliente!.NombreCompleto,
                x.Comprobante.ClienteNumeroDocumento != null && x.Comprobante.ClienteNumeroDocumento != string.Empty ? x.Comprobante.ClienteNumeroDocumento : x.Comprobante.Cliente!.NumeroDocumento,
                x.Comprobante.ClienteDireccion != null && x.Comprobante.ClienteDireccion != string.Empty ? x.Comprobante.ClienteDireccion : x.Comprobante.Direccion,
                x.Comprobante.Total,
                x.Comprobante.TipoComprobante == TipoComprobante.NCR ? x.Comprobante.TotalPagado : x.TotalPagado + x.TotalAplicado,
                x.Comprobante.TipoComprobante == TipoComprobante.NCR
                    ? x.Comprobante.SaldoPendiente
                    : x.Comprobante.Total - x.TotalNotasCredito - (x.TotalPagado + x.TotalAplicado) < 0 ? 0 : x.Comprobante.Total - x.TotalNotasCredito - (x.TotalPagado + x.TotalAplicado),
                x.Comprobante.TipoComprobante == TipoComprobante.NCR
                    ? x.Comprobante.EstadoPago
                    : x.TotalPagado + x.TotalAplicado <= 0
                    ? EstadoPagoComprobante.PENDIENTE
                    : x.TotalPagado + x.TotalAplicado >= x.Comprobante.Total - x.TotalNotasCredito
                        ? EstadoPagoComprobante.PAGADO
                        : EstadoPagoComprobante.PAGO_PARCIAL,
                x.Comprobante.EstadoSunat,
                x.Comprobante.DocumentoImpreso || !string.IsNullOrWhiteSpace(x.Comprobante.PdfUrl),
                x.Comprobante.EstadoSunat == EstadoSunat.Aceptado,
                x.Comprobante.Estado,
                x.Comprobante.Estado == EstadoRegistro.Activo
                    && !(x.Comprobante.DocumentoImpreso || !string.IsNullOrWhiteSpace(x.Comprobante.PdfUrl))
                    && x.Comprobante.EstadoSunat != EstadoSunat.Aceptado,
                x.Comprobante.Estado == EstadoRegistro.Activo
                    && x.Comprobante.TipoComprobante != TipoComprobante.COT
                    && x.Comprobante.TipoComprobante != TipoComprobante.NPE
                    && x.Comprobante.TipoComprobante != TipoComprobante.NCR
                    && x.TotalPagado + x.TotalAplicado < x.Comprobante.Total - x.TotalNotasCredito,
                x.Comprobante.Estado == EstadoRegistro.Activo
                    && x.Comprobante.TipoComprobante != TipoComprobante.COT
                    && x.Comprobante.TipoComprobante != TipoComprobante.NPE
                    && hoy <= x.Comprobante.FechaEmision.Date.AddDays(2)))
            .ToPagedAsync(request, cancellationToken);
    }

    public Task<Comprobante?> ObtenerAsync(int empresaId, int id, CancellationToken cancellationToken) =>
        db.Comprobantes.Include(x => x.Detalles).ThenInclude(x => x.Producto)
            .Include(x => x.Cliente)
            .Include(x => x.Empresa)
            .Include(x => x.ComprobanteReferencia)
            .Include(x => x.NotaPedido)
            .Include(x => x.Cobros)
            .Include(x => x.CobrosAplicados).ThenInclude(x => x.CobroCliente)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.ComprobanteId == id, cancellationToken);

    public Task<bool> TieneNotaCreditoActivaAsync(int empresaId, int comprobanteReferenciaId, CancellationToken cancellationToken) =>
        db.Comprobantes.AsNoTracking().AnyAsync(x => x.EmpresaId == empresaId
            && x.TipoComprobante == TipoComprobante.NCR
            && x.Estado == EstadoRegistro.Activo
            && x.ComprobanteReferenciaId == comprobanteReferenciaId, cancellationToken);

    public async Task<decimal> TotalNotasCreditoActivasAsync(int empresaId, int comprobanteReferenciaId, CancellationToken cancellationToken)
    {
        var total = await db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId
                && x.TipoComprobante == TipoComprobante.NCR
                && x.Estado == EstadoRegistro.Activo
                && x.ComprobanteReferenciaId == comprobanteReferenciaId)
            .SumAsync(x => (decimal?)x.Total, cancellationToken);
        return total ?? 0;
    }

    public async Task<int> SiguienteCorrelativoAsync(int empresaId, TipoComprobante tipo, string serie, CancellationToken cancellationToken)
    {
        var ultimo = await db.Comprobantes.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.TipoComprobante == tipo && x.Serie == serie)
            .OrderByDescending(x => x.Correlativo)
            .Select(x => x.Correlativo)
            .FirstOrDefaultAsync(cancellationToken);

        var siguienteCalculado = ultimo + 1;
        var clave = $"Correlativo.{tipo}.{serie}";
        var configurado = await db.ConfiguracionesEmpresa.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Clave == clave)
            .Select(x => x.Valor)
            .FirstOrDefaultAsync(cancellationToken);

        return int.TryParse(configurado, out var siguienteConfigurado) && siguienteConfigurado > siguienteCalculado
            ? siguienteConfigurado
            : siguienteCalculado;
    }

    public async Task GuardarAsync(Comprobante comprobante, CancellationToken cancellationToken)
    {
        if (comprobante.ComprobanteId == 0) db.Comprobantes.Add(comprobante);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task EjecutarEnTransaccionAsync(Func<Task> operacion, CancellationToken cancellationToken)
    {
        if (db.Database.CurrentTransaction is not null)
        {
            await operacion();
            return;
        }

        var strategy = db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
            await operacion();
            await transaction.CommitAsync(cancellationToken);
        });
    }
}


