using ViveroLosFrutales.Domain.Common;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.DTOs;

public record CuentaFinancieraSaldoDto(
    int CuentaFinancieraId,
    string Cuenta,
    TipoCuentaFinanciera Tipo,
    string Banco,
    string NumeroCuenta,
    string Moneda,
    decimal SaldoInicial,
    DateTime FechaSaldoInicial,
    decimal Ingresos,
    decimal Egresos,
    decimal Saldo,
    bool Activo);

public record CajaBancosDto(
    decimal DineroDisponible,
    decimal TotalEfectivo,
    decimal TotalBancos,
    decimal TotalBilleteras,
    decimal IngresosPeriodo,
    decimal EgresosPeriodo,
    IReadOnlyList<CuentaFinancieraSaldoDto> Cuentas);

public record CuentaFinancieraOptionDto(
    int CuentaFinancieraId,
    string Nombre,
    TipoCuentaFinanciera Tipo,
    string Moneda);

public record CuentaFinancieraListDto(
    int CuentaFinancieraId,
    string Nombre,
    TipoCuentaFinanciera Tipo,
    string Banco,
    string NumeroCuenta,
    string Moneda,
    decimal SaldoInicial,
    DateTime FechaSaldoInicial,
    bool Activo,
    bool TieneMovimientos);

public class CuentaFinancieraEditDto
{
    public int CuentaFinancieraId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoCuentaFinanciera Tipo { get; set; } = TipoCuentaFinanciera.CAJA;
    public string Banco { get; set; } = string.Empty;
    public string NumeroCuenta { get; set; } = string.Empty;
    public string Moneda { get; set; } = "PEN";
    public decimal SaldoInicial { get; set; }
    public DateTime FechaSaldoInicial { get; set; } = PeruDateTime.Today;
    public bool Activo { get; set; } = true;
}

public class TransferenciaEditDto
{
    public DateTime Fecha { get; set; } = PeruDateTime.Today;
    public int CuentaOrigenId { get; set; }
    public int CuentaDestinoId { get; set; }
    public decimal Monto { get; set; }
    public string Observacion { get; set; } = string.Empty;
    public IReadOnlyList<CuentaFinancieraOptionDto> Cuentas { get; set; } = Array.Empty<CuentaFinancieraOptionDto>();
}

public record TransferenciaListDto(
    int TransferenciaFinancieraId,
    DateTime Fecha,
    string CuentaOrigen,
    string CuentaDestino,
    decimal Monto,
    string Observacion,
    EstadoRegistro Estado);
