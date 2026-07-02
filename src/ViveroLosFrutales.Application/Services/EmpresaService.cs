using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class EmpresaService(IEmpresaRepository repository)
{
    public Task<PagedResult<EmpresaListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(request, cancellationToken);

    public async Task<EmpresaEditDto?> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var empresa = await repository.ObtenerAsync(id, cancellationToken);
        return empresa is null ? null : ToDto(empresa);
    }

    public Task<EmpresaMarcaDto?> ObtenerMarcaActivaAsync(int empresaId, string usuarioId, CancellationToken cancellationToken) =>
        repository.ObtenerMarcaActivaAsync(empresaId, usuarioId, cancellationToken);

    public async Task<EmpresaEditDto?> ObtenerLogoActivaAsync(int empresaId, string usuarioId, CancellationToken cancellationToken)
    {
        var empresa = await repository.ObtenerLogoActivaAsync(empresaId, usuarioId, cancellationToken);
        return empresa is null ? null : ToDto(empresa);
    }

    public async Task GuardarAsync(EmpresaEditDto dto, CancellationToken cancellationToken)
    {
        Validar(dto);
        var empresa = dto.EmpresaId == 0 ? new Empresa() : await repository.ObtenerAsync(dto.EmpresaId, cancellationToken);
        if (empresa is null) throw new InvalidOperationException("Empresa no encontrada.");

        empresa.RUC = dto.RUC.Trim();
        empresa.RazonSocial = dto.RazonSocial.Trim();
        empresa.NombreComercial = dto.NombreComercial?.Trim() ?? string.Empty;
        empresa.Direccion = dto.Direccion?.Trim() ?? string.Empty;
        empresa.Telefono = dto.Telefono?.Trim() ?? string.Empty;
        empresa.Email = dto.Email?.Trim() ?? string.Empty;
        empresa.MonedaPredeterminada = dto.MonedaPredeterminada.Trim();
        empresa.UrlNubefact = dto.UrlNubefact?.Trim() ?? string.Empty;
        var tokenNubefact = dto.TokenNubefact?.Trim() ?? string.Empty;
        if (dto.EmpresaId == 0 || !string.IsNullOrWhiteSpace(tokenNubefact))
        {
            empresa.TokenNubefact = tokenNubefact;
        }
        empresa.LogoPath = dto.LogoPath?.Trim() ?? string.Empty;
        empresa.RepresentanteLegalNombre = dto.RepresentanteLegalNombre?.Trim() ?? string.Empty;
        empresa.RepresentanteLegalDocumento = dto.RepresentanteLegalDocumento?.Trim() ?? string.Empty;
        empresa.RepresentanteLegalCargo = dto.RepresentanteLegalCargo?.Trim() ?? string.Empty;
        if (dto.LogoContenido is { Length: > 0 })
        {
            empresa.LogoContenido = dto.LogoContenido;
            empresa.LogoContentType = dto.LogoContentType?.Trim() ?? string.Empty;
            empresa.LogoNombre = dto.LogoNombre?.Trim() ?? string.Empty;
        }
        if (dto.FirmaContenido is { Length: > 0 })
        {
            empresa.FirmaContenido = dto.FirmaContenido;
            empresa.FirmaContentType = dto.FirmaContentType?.Trim() ?? string.Empty;
            empresa.FirmaNombre = dto.FirmaNombre?.Trim() ?? string.Empty;
        }
        empresa.SerieBoleta = dto.SerieBoleta.Trim();
        empresa.SerieFactura = dto.SerieFactura.Trim();
        empresa.SerieNotaCredito = dto.SerieNotaCredito.Trim();
        empresa.SerieNotaCreditoFactura = dto.SerieNotaCreditoFactura.Trim();
        empresa.SerieNotaCreditoBoleta = dto.SerieNotaCreditoBoleta.Trim();
        empresa.SerieNotaPedido = dto.SerieNotaPedido.Trim();
        empresa.SerieCotizacion = dto.SerieCotizacion.Trim();
        empresa.Estado = dto.Estado;

        await repository.GuardarAsync(empresa, cancellationToken);
    }

    public async Task AnularAsync(int id, CancellationToken cancellationToken)
    {
        var empresa = await repository.ObtenerAsync(id, cancellationToken) ?? throw new InvalidOperationException("Empresa no encontrada.");
        empresa.Estado = EstadoRegistro.Anulado;
        await repository.GuardarAsync(empresa, cancellationToken);
    }

    private static void Validar(EmpresaEditDto dto)
    {
        if (dto.RUC.Trim().Length != 11) throw new InvalidOperationException("El RUC debe tener 11 digitos.");
        if (string.IsNullOrWhiteSpace(dto.RazonSocial)) throw new InvalidOperationException("La razon social es obligatoria.");
    }

    private static EmpresaEditDto ToDto(Empresa empresa) => new()
    {
        EmpresaId = empresa.EmpresaId,
        RUC = empresa.RUC,
        RazonSocial = empresa.RazonSocial,
        NombreComercial = empresa.NombreComercial,
        Direccion = empresa.Direccion,
        Telefono = empresa.Telefono,
        Email = empresa.Email,
        MonedaPredeterminada = empresa.MonedaPredeterminada,
        UrlNubefact = empresa.UrlNubefact,
        TokenNubefact = empresa.TokenNubefact,
        LogoPath = empresa.LogoPath,
        LogoContenido = empresa.LogoContenido,
        LogoContentType = empresa.LogoContentType,
        LogoNombre = empresa.LogoNombre,
        RepresentanteLegalNombre = empresa.RepresentanteLegalNombre,
        RepresentanteLegalDocumento = empresa.RepresentanteLegalDocumento,
        RepresentanteLegalCargo = empresa.RepresentanteLegalCargo,
        FirmaContenido = empresa.FirmaContenido,
        FirmaContentType = empresa.FirmaContentType,
        FirmaNombre = empresa.FirmaNombre,
        SerieBoleta = empresa.SerieBoleta,
        SerieFactura = empresa.SerieFactura,
        SerieNotaCredito = empresa.SerieNotaCredito,
        SerieNotaCreditoFactura = empresa.SerieNotaCreditoFactura,
        SerieNotaCreditoBoleta = empresa.SerieNotaCreditoBoleta,
        SerieNotaPedido = empresa.SerieNotaPedido,
        SerieCotizacion = empresa.SerieCotizacion,
        Estado = empresa.Estado
    };
}
