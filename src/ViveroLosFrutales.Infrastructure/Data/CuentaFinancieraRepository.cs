using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Data;

public class CuentaFinancieraRepository(ApplicationDbContext db) : ICuentaFinancieraRepository
{
    private const string CuentaPrincipalNombre = "Caja principal";

    public async Task<CuentaFinanciera> EnsureCuentaPrincipalAsync(int empresaId, CancellationToken cancellationToken)
    {
        var cuenta = await db.CuentasFinancieras
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.Nombre == CuentaPrincipalNombre, cancellationToken);

        if (cuenta is not null) return cuenta;

        cuenta = new CuentaFinanciera
        {
            EmpresaId = empresaId,
            Nombre = CuentaPrincipalNombre,
            Tipo = TipoCuentaFinanciera.CAJA,
            Moneda = "PEN",
            SaldoInicial = 0,
            FechaSaldoInicial = PeruDateTime.Today,
            Activo = true
        };

        db.CuentasFinancieras.Add(cuenta);
        await db.SaveChangesAsync(cancellationToken);
        return cuenta;
    }

    public Task<PagedResult<CuentaFinancieraListDto>> BuscarAsync(int empresaId, SearchRequest request, TipoCuentaFinanciera? tipo, bool? activo, CancellationToken cancellationToken)
    {
        var query = db.CuentasFinancieras.AsNoTracking().Where(x => x.EmpresaId == empresaId);
        if (tipo is not null) query = query.Where(x => x.Tipo == tipo);
        if (activo is not null) query = query.Where(x => x.Activo == activo);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            query = query.Where(x => x.Nombre.Contains(term) || x.Banco.Contains(term) || x.NumeroCuenta.Contains(term));
        }

        return query.OrderBy(x => x.Tipo).ThenBy(x => x.Nombre)
            .Select(x => new CuentaFinancieraListDto(
                x.CuentaFinancieraId,
                x.Nombre,
                x.Tipo,
                x.Banco,
                x.NumeroCuenta,
                x.Moneda,
                x.SaldoInicial,
                x.FechaSaldoInicial,
                x.Activo,
                db.MovimientosCaja.Any(m => m.EmpresaId == empresaId && m.CuentaFinancieraId == x.CuentaFinancieraId)))
            .ToPagedAsync(request, cancellationToken);
    }

    public async Task<IReadOnlyList<CuentaFinancieraOptionDto>> ListarActivasAsync(int empresaId, CancellationToken cancellationToken)
    {
        await EnsureCuentaPrincipalAsync(empresaId, cancellationToken);
        return await db.CuentasFinancieras.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Activo)
            .OrderBy(x => x.Tipo)
            .ThenBy(x => x.Nombre)
            .Select(x => new CuentaFinancieraOptionDto(x.CuentaFinancieraId, x.Nombre, x.Tipo, x.Moneda))
            .ToListAsync(cancellationToken);
    }

    public Task<CuentaFinanciera?> ObtenerAsync(int empresaId, int cuentaFinancieraId, CancellationToken cancellationToken) =>
        db.CuentasFinancieras.FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.CuentaFinancieraId == cuentaFinancieraId, cancellationToken);

    public Task<CuentaFinanciera?> ObtenerActivaAsync(int empresaId, int cuentaFinancieraId, CancellationToken cancellationToken) =>
        db.CuentasFinancieras.AsNoTracking().FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.CuentaFinancieraId == cuentaFinancieraId && x.Activo, cancellationToken);

    public Task<bool> TieneMovimientosAsync(int empresaId, int cuentaFinancieraId, CancellationToken cancellationToken) =>
        db.MovimientosCaja.AnyAsync(x => x.EmpresaId == empresaId && x.CuentaFinancieraId == cuentaFinancieraId, cancellationToken);

    public async Task GuardarAsync(CuentaFinanciera cuenta, CancellationToken cancellationToken)
    {
        if (cuenta.CuentaFinancieraId == 0) db.CuentasFinancieras.Add(cuenta);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<CajaBancosDto> ObtenerCajaBancosAsync(int empresaId, DateTime? fechaDesde, DateTime? fechaHasta, TipoCuentaFinanciera? tipo, string? search, CancellationToken cancellationToken)
    {
        var cuentaPrincipal = await EnsureCuentaPrincipalAsync(empresaId, cancellationToken);
        var cuentasQuery = db.CuentasFinancieras.AsNoTracking().Where(x => x.EmpresaId == empresaId);
        if (tipo is not null) cuentasQuery = cuentasQuery.Where(x => x.Tipo == tipo);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            cuentasQuery = cuentasQuery.Where(x => x.Nombre.Contains(term) || x.Banco.Contains(term) || x.NumeroCuenta.Contains(term));
        }

        var cuentas = await cuentasQuery.OrderBy(x => x.Tipo).ThenBy(x => x.Nombre).ToListAsync(cancellationToken);
        var hastaExclusivo = fechaHasta?.Date.AddDays(1);

        var movimientosAcumuladosQuery = db.MovimientosCaja.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo);
        if (hastaExclusivo is not null) movimientosAcumuladosQuery = movimientosAcumuladosQuery.Where(x => x.Fecha < hastaExclusivo.Value);

        var acumulados = await movimientosAcumuladosQuery
            .GroupBy(x => x.CuentaFinancieraId ?? cuentaPrincipal.CuentaFinancieraId)
            .Select(x => new
            {
                CuentaFinancieraId = x.Key,
                Ingresos = x.Where(m => m.TipoMovimiento == TipoMovimientoCaja.INGRESO).Sum(m => (decimal?)m.Monto) ?? 0,
                Egresos = x.Where(m => m.TipoMovimiento == TipoMovimientoCaja.EGRESO).Sum(m => (decimal?)m.Monto) ?? 0
            })
            .ToDictionaryAsync(x => x.CuentaFinancieraId, cancellationToken);

        var movimientosPeriodoQuery = db.MovimientosCaja.AsNoTracking()
            .Where(x => x.EmpresaId == empresaId && x.Estado == EstadoRegistro.Activo);
        if (fechaDesde is not null) movimientosPeriodoQuery = movimientosPeriodoQuery.Where(x => x.Fecha >= fechaDesde.Value.Date);
        if (hastaExclusivo is not null) movimientosPeriodoQuery = movimientosPeriodoQuery.Where(x => x.Fecha < hastaExclusivo.Value);

        var periodo = await movimientosPeriodoQuery
            .GroupBy(x => x.CuentaFinancieraId ?? cuentaPrincipal.CuentaFinancieraId)
            .Select(x => new
            {
                CuentaFinancieraId = x.Key,
                Ingresos = x.Where(m => m.TipoMovimiento == TipoMovimientoCaja.INGRESO).Sum(m => (decimal?)m.Monto) ?? 0,
                Egresos = x.Where(m => m.TipoMovimiento == TipoMovimientoCaja.EGRESO).Sum(m => (decimal?)m.Monto) ?? 0
            })
            .ToDictionaryAsync(x => x.CuentaFinancieraId, cancellationToken);

        var saldos = cuentas.Select(cuenta =>
        {
            acumulados.TryGetValue(cuenta.CuentaFinancieraId, out var acumulado);
            periodo.TryGetValue(cuenta.CuentaFinancieraId, out var movPeriodo);
            var saldo = cuenta.SaldoInicial + (acumulado?.Ingresos ?? 0) - (acumulado?.Egresos ?? 0);

            return new CuentaFinancieraSaldoDto(
                cuenta.CuentaFinancieraId,
                cuenta.Nombre,
                cuenta.Tipo,
                cuenta.Banco,
                cuenta.NumeroCuenta,
                cuenta.Moneda,
                cuenta.SaldoInicial,
                cuenta.FechaSaldoInicial,
                movPeriodo?.Ingresos ?? 0,
                movPeriodo?.Egresos ?? 0,
                saldo,
                cuenta.Activo);
        }).ToList();

        return new CajaBancosDto(
            saldos.Sum(x => x.Saldo),
            saldos.Where(x => x.Tipo == TipoCuentaFinanciera.CAJA).Sum(x => x.Saldo),
            saldos.Where(x => x.Tipo == TipoCuentaFinanciera.BANCO).Sum(x => x.Saldo),
            saldos.Where(x => x.Tipo == TipoCuentaFinanciera.BILLETERA).Sum(x => x.Saldo),
            saldos.Sum(x => x.Ingresos),
            saldos.Sum(x => x.Egresos),
            saldos);
    }
}
