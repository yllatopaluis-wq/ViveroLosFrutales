using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class ErrorAplicacionService(IErrorAplicacionRepository repository, IEmpresaContext empresaContext)
{
    public Task<PagedResult<ErrorAplicacionListDto>> BuscarAsync(ErrorAplicacionSearchDto request, CancellationToken cancellationToken) =>
        repository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public async Task<ErrorAplicacionDetalleDto> ObtenerAsync(int id, CancellationToken cancellationToken)
    {
        var error = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Registro de error no encontrado.");

        return new ErrorAplicacionDetalleDto
        {
            ErrorAplicacionId = error.ErrorAplicacionId,
            FechaUtc = error.FechaUtc,
            Empresa = error.Empresa?.RazonSocial ?? "Sin empresa seleccionada",
            Usuario = error.Usuario,
            Ruta = error.Ruta,
            MetodoHttp = error.MetodoHttp,
            TipoExcepcion = error.TipoExcepcion,
            Mensaje = error.Mensaje,
            Detalle = error.Detalle,
            Identificador = error.Identificador,
            Estado = error.Estado,
            FechaRevisionUtc = error.FechaRevisionUtc,
            UsuarioRevision = error.UsuarioRevision,
            ObservacionRevision = error.ObservacionRevision
        };
    }

    public async Task MarcarRevisadoAsync(int id, string observacion, CancellationToken cancellationToken)
    {
        var error = await repository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Registro de error no encontrado.");
        error.Estado = EstadoErrorAplicacion.REVISADO;
        error.FechaRevisionUtc = DateTime.UtcNow;
        error.UsuarioRevision = empresaContext.UsuarioNombre;
        error.ObservacionRevision = observacion?.Trim() ?? string.Empty;
        await repository.GuardarAsync(error, cancellationToken);
    }
}
