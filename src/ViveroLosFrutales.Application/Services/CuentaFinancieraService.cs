using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class CuentaFinancieraService(ICuentaFinancieraRepository repository, IEmpresaContext empresaContext)
{
    public Task<PagedResult<CuentaFinancieraListDto>> BuscarAsync(SearchRequest request, TipoCuentaFinanciera? tipo, bool? activo, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, tipo, activo, cancellationToken);

    public Task<IReadOnlyList<CuentaFinancieraOptionDto>> ListarActivasAsync(CancellationToken cancellationToken) =>
        repository.ListarActivasAsync(empresaContext.EmpresaId, cancellationToken);

    public async Task<CuentaFinancieraEditDto> NuevoAsync(CancellationToken cancellationToken)
    {
        await repository.EnsureCuentaPrincipalAsync(empresaContext.EmpresaId, cancellationToken);
        return new CuentaFinancieraEditDto { FechaSaldoInicial = PeruDateTime.Today, Moneda = "PEN", Activo = true };
    }

    public async Task<CuentaFinancieraEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var cuenta = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken);
        return cuenta is null ? null : ToDto(cuenta);
    }

    public async Task GuardarAsync(CuentaFinancieraEditDto dto, CancellationToken cancellationToken)
    {
        var nombre = (dto.Nombre ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(nombre)) throw new InvalidOperationException("El nombre de la cuenta financiera es obligatorio.");
        if (dto.Tipo == TipoCuentaFinanciera.BANCO && string.IsNullOrWhiteSpace(dto.Banco)) throw new InvalidOperationException("El banco es obligatorio para cuentas bancarias.");

        var cuenta = dto.CuentaFinancieraId == 0
            ? new CuentaFinanciera { EmpresaId = empresaContext.EmpresaId, UsuarioRegistro = empresaContext.UsuarioNombre }
            : await repository.ObtenerAsync(empresaContext.EmpresaId, dto.CuentaFinancieraId, cancellationToken);
        if (cuenta is null) throw new InvalidOperationException("Cuenta financiera no encontrada.");

        cuenta.Nombre = nombre;
        cuenta.Tipo = dto.Tipo;
        cuenta.Banco = dto.Tipo == TipoCuentaFinanciera.CAJA ? string.Empty : (dto.Banco ?? string.Empty).Trim();
        cuenta.NumeroCuenta = dto.Tipo == TipoCuentaFinanciera.CAJA ? string.Empty : (dto.NumeroCuenta ?? string.Empty).Trim();
        cuenta.Moneda = string.IsNullOrWhiteSpace(dto.Moneda) ? "PEN" : dto.Moneda.Trim().ToUpperInvariant();
        cuenta.SaldoInicial = decimal.Round(dto.SaldoInicial, 2);
        cuenta.FechaSaldoInicial = dto.FechaSaldoInicial.Date;
        cuenta.Activo = dto.Activo;
        cuenta.FechaModificacion = DateTime.UtcNow;
        cuenta.UsuarioModificacion = empresaContext.UsuarioNombre;

        await repository.GuardarAsync(cuenta, cancellationToken);
    }

    public async Task CambiarEstadoAsync(int id, bool activo, CancellationToken cancellationToken)
    {
        var cuenta = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Cuenta financiera no encontrada.");
        cuenta.Activo = activo;
        cuenta.FechaModificacion = DateTime.UtcNow;
        cuenta.UsuarioModificacion = empresaContext.UsuarioNombre;
        await repository.GuardarAsync(cuenta, cancellationToken);
    }

    public async Task<int> ResolverCuentaIdAsync(int? cuentaFinancieraId, CancellationToken cancellationToken)
    {
        if (cuentaFinancieraId is int id && id > 0)
        {
            var cuenta = await repository.ObtenerActivaAsync(empresaContext.EmpresaId, id, cancellationToken)
                ?? throw new InvalidOperationException("La cuenta financiera seleccionada no existe o esta inactiva.");
            return cuenta.CuentaFinancieraId;
        }

        var principal = await repository.EnsureCuentaPrincipalAsync(empresaContext.EmpresaId, cancellationToken);
        return principal.CuentaFinancieraId;
    }

    public Task<CajaBancosDto> ObtenerCajaBancosAsync(DateTime? fechaDesde, DateTime? fechaHasta, TipoCuentaFinanciera? tipo, string? search, CancellationToken cancellationToken) =>
        repository.ObtenerCajaBancosAsync(empresaContext.EmpresaId, fechaDesde, fechaHasta, tipo, search, cancellationToken);

    public Task<CajaBancosDto> ObtenerCajaBancosAsync(CancellationToken cancellationToken) =>
        ObtenerCajaBancosAsync(null, null, null, null, cancellationToken);

    private static CuentaFinancieraEditDto ToDto(CuentaFinanciera cuenta) => new()
    {
        CuentaFinancieraId = cuenta.CuentaFinancieraId,
        Nombre = cuenta.Nombre,
        Tipo = cuenta.Tipo,
        Banco = cuenta.Banco,
        NumeroCuenta = cuenta.NumeroCuenta,
        Moneda = cuenta.Moneda,
        SaldoInicial = cuenta.SaldoInicial,
        FechaSaldoInicial = cuenta.FechaSaldoInicial,
        Activo = cuenta.Activo
    };
}
