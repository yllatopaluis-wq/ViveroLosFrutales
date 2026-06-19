using ViveroLosFrutales.Application.Common;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Application.Services;

public class DevolucionService(
    IDevolucionRepository devolucionRepository,
    IMovimientoCajaRepository movimientoCajaRepository,
    IEmpresaContext empresaContext)
{
    public Task<PagedResult<DevolucionListDto>> BuscarAsync(SearchRequest request, CancellationToken cancellationToken) =>
        devolucionRepository.BuscarAsync(empresaContext.EmpresaId, request, cancellationToken);

    public async Task<DevolucionDetalleDto> ObtenerDetalleAsync(int id, CancellationToken cancellationToken)
    {
        var devolucion = await ObtenerEntidadAsync(id, cancellationToken);
        return ToDetalleDto(devolucion);
    }

    public async Task<RegistrarDevolucionDto> ObtenerFormularioRegistroAsync(int id, CancellationToken cancellationToken)
    {
        var devolucion = await ObtenerEntidadAsync(id, cancellationToken);
        if (devolucion.EstadoDevolucion is EstadoDevolucion.DEVUELTO or EstadoDevolucion.ANULADO)
        {
            throw new InvalidOperationException("La devolucion no permite nuevos registros.");
        }

        return new RegistrarDevolucionDto
        {
            DevolucionId = devolucion.DevolucionId,
            TipoTercero = devolucion.TipoTercero,
            Tercero = Tercero(devolucion),
            DocumentoOrigen = Documento(devolucion),
            MontoPendiente = devolucion.MontoPendiente,
            MontoDevolver = devolucion.MontoPendiente,
            Fecha = DateTime.Today
        };
    }

    public async Task RegistrarDevolucionAsync(RegistrarDevolucionDto dto, CancellationToken cancellationToken)
    {
        if (dto.MontoDevolver <= 0)
        {
            throw new InvalidOperationException("El monto a devolver debe ser mayor a cero.");
        }

        if (string.IsNullOrWhiteSpace(dto.MedioDevolucion))
        {
            throw new InvalidOperationException("Ingrese el medio de devolucion.");
        }

        await devolucionRepository.EjecutarEnTransaccionAsync(async () =>
        {
            var devolucion = await ObtenerEntidadAsync(dto.DevolucionId, cancellationToken);
            ValidarTercero(devolucion);

            if (devolucion.EstadoDevolucion is not (EstadoDevolucion.PENDIENTE or EstadoDevolucion.PARCIAL))
            {
                throw new InvalidOperationException("La devolucion no permite nuevos registros.");
            }

            if (dto.MontoDevolver > devolucion.MontoPendiente)
            {
                throw new InvalidOperationException("El monto a devolver no puede superar el monto pendiente.");
            }

            await movimientoCajaRepository.GuardarAsync(new MovimientoCaja
            {
                EmpresaId = empresaContext.EmpresaId,
                ClienteId = devolucion.TipoTercero == TipoTerceroDevolucion.CLIENTE ? devolucion.ClienteId : null,
                ProveedorId = devolucion.TipoTercero == TipoTerceroDevolucion.PROVEEDOR ? devolucion.ProveedorId : null,
                TipoMovimiento = devolucion.TipoTercero == TipoTerceroDevolucion.CLIENTE
                    ? TipoMovimientoCaja.EGRESO
                    : TipoMovimientoCaja.INGRESO,
                Origen = devolucion.TipoTercero == TipoTerceroDevolucion.CLIENTE
                    ? OrigenMovimientoCaja.DEVOLUCION_CLIENTE
                    : OrigenMovimientoCaja.DEVOLUCION_PROVEEDOR,
                OrigenId = devolucion.DevolucionId,
                Fecha = dto.Fecha.Date,
                Monto = decimal.Round(dto.MontoDevolver, 2),
                MedioPago = dto.MedioDevolucion.Trim().ToUpperInvariant(),
                Descripcion = string.IsNullOrWhiteSpace(dto.Observacion)
                    ? $"Devolucion {devolucion.TipoTercero.ToString().ToLowerInvariant()} {Documento(devolucion)}"
                    : dto.Observacion.Trim(),
                UsuarioRegistro = empresaContext.UsuarioNombre
            }, cancellationToken);

            devolucion.MontoDevuelto = decimal.Round(devolucion.MontoDevuelto + dto.MontoDevolver, 2);
            devolucion.MontoPendiente = decimal.Round(devolucion.MontoOriginal - devolucion.MontoDevuelto, 2);
            if (devolucion.MontoPendiente < 0) devolucion.MontoPendiente = 0;
            devolucion.EstadoDevolucion = devolucion.MontoPendiente <= 0
                ? EstadoDevolucion.DEVUELTO
                : EstadoDevolucion.PARCIAL;
            devolucion.Observacion = dto.Observacion.Trim();
            devolucion.FechaModificacion = DateTime.UtcNow;
            devolucion.UsuarioModificacion = empresaContext.UsuarioNombre;
            await devolucionRepository.GuardarAsync(devolucion, cancellationToken);
        }, cancellationToken);
    }

    public Task AnularPendienteAsync(int id, string motivo, CancellationToken cancellationToken) =>
        throw new InvalidOperationException("No se puede anular una solicitud de devolucion. Registre la devolucion para mantener el sustento del dinero pendiente.");

    public async Task<Devolucion?> CrearDevolucionPorAnulacionNotaPedidoAsync(NotaPedido nota, decimal totalCobradoActivo, string motivo, CancellationToken cancellationToken)
    {
        if (totalCobradoActivo <= 0) return null;
        if (await devolucionRepository.ExisteActivaPorNotaPedidoAsync(empresaContext.EmpresaId, nota.NotaPedidoId, cancellationToken))
        {
            return null;
        }

        var devolucion = new Devolucion
        {
            EmpresaId = empresaContext.EmpresaId,
            TipoTercero = TipoTerceroDevolucion.CLIENTE,
            ClienteId = nota.ClienteId,
            Origen = OrigenDevolucion.ANULACION_NOTA_PEDIDO,
            NotaPedidoId = nota.NotaPedidoId,
            FechaGeneracion = DateTime.Today,
            MontoOriginal = decimal.Round(totalCobradoActivo, 2),
            MontoPendiente = decimal.Round(totalCobradoActivo, 2),
            MontoDevuelto = 0,
            EstadoDevolucion = EstadoDevolucion.PENDIENTE,
            MotivoGeneracion = motivo.Trim(),
            Observacion = "Generada por anulacion de nota de pedido con cobros historicos.",
            UsuarioRegistro = empresaContext.UsuarioNombre
        };
        ValidarTercero(devolucion);

        await devolucionRepository.GuardarAsync(devolucion, cancellationToken);
        return devolucion;
    }

    public async Task<Devolucion?> CrearDevolucionPorNotaCreditoAsync(Comprobante comprobanteOriginal, Comprobante notaCredito, decimal totalCobradoActivo, CancellationToken cancellationToken)
    {
        var nuevoTotalValido = decimal.Round(comprobanteOriginal.Total - notaCredito.Total, 2);
        if (nuevoTotalValido < 0) nuevoTotalValido = 0;
        var excesoCobrado = decimal.Round(totalCobradoActivo - nuevoTotalValido, 2);
        if (excesoCobrado <= 0) return null;

        if (await devolucionRepository.ExisteActivaPorNotaCreditoAsync(empresaContext.EmpresaId, notaCredito.ComprobanteId, cancellationToken))
        {
            return null;
        }

        var devolucion = new Devolucion
        {
            EmpresaId = empresaContext.EmpresaId,
            TipoTercero = TipoTerceroDevolucion.CLIENTE,
            ClienteId = comprobanteOriginal.ClienteId,
            Origen = OrigenDevolucion.NOTA_CREDITO,
            ComprobanteId = comprobanteOriginal.ComprobanteId,
            NotaCreditoId = notaCredito.ComprobanteId,
            FechaGeneracion = DateTime.Today,
            MontoOriginal = excesoCobrado,
            MontoPendiente = excesoCobrado,
            MontoDevuelto = 0,
            EstadoDevolucion = EstadoDevolucion.PENDIENTE,
            MotivoGeneracion = notaCredito.MotivoNotaCredito,
            Observacion = "Generada por exceso cobrado luego de emitir nota de credito.",
            UsuarioRegistro = empresaContext.UsuarioNombre
        };
        ValidarTercero(devolucion);

        await devolucionRepository.GuardarAsync(devolucion, cancellationToken);
        return devolucion;
    }

    public async Task<Devolucion?> CrearDevolucionPorAnulacionComprobanteAsync(Comprobante comprobante, decimal totalCobradoActivo, string motivo, CancellationToken cancellationToken)
    {
        if (totalCobradoActivo <= 0) return null;
        if (await devolucionRepository.ExisteActivaPorComprobanteAsync(empresaContext.EmpresaId, comprobante.ComprobanteId, cancellationToken))
        {
            return null;
        }

        var devolucion = new Devolucion
        {
            EmpresaId = empresaContext.EmpresaId,
            TipoTercero = TipoTerceroDevolucion.CLIENTE,
            ClienteId = comprobante.ClienteId,
            Origen = OrigenDevolucion.ANULACION_COMPROBANTE,
            ComprobanteId = comprobante.ComprobanteId,
            FechaGeneracion = DateTime.Today,
            MontoOriginal = decimal.Round(totalCobradoActivo, 2),
            MontoPendiente = decimal.Round(totalCobradoActivo, 2),
            MontoDevuelto = 0,
            EstadoDevolucion = EstadoDevolucion.PENDIENTE,
            MotivoGeneracion = motivo.Trim(),
            Observacion = "Generada por anulacion de comprobante con cobros historicos.",
            UsuarioRegistro = empresaContext.UsuarioNombre
        };
        ValidarTercero(devolucion);

        await devolucionRepository.GuardarAsync(devolucion, cancellationToken);
        return devolucion;
    }

    public async Task<Devolucion?> CrearDevolucionPorAnulacionCompraAsync(Compra compra, decimal totalPagadoActivo, string motivo, CancellationToken cancellationToken)
    {
        if (totalPagadoActivo <= 0) return null;
        if (await devolucionRepository.ExisteActivaPorCompraAsync(empresaContext.EmpresaId, compra.CompraId, cancellationToken))
        {
            return null;
        }

        var devolucion = new Devolucion
        {
            EmpresaId = empresaContext.EmpresaId,
            TipoTercero = TipoTerceroDevolucion.PROVEEDOR,
            ProveedorId = compra.ProveedorId,
            Origen = OrigenDevolucion.ANULACION_COMPRA,
            CompraId = compra.CompraId,
            FechaGeneracion = DateTime.Today,
            MontoOriginal = decimal.Round(totalPagadoActivo, 2),
            MontoPendiente = decimal.Round(totalPagadoActivo, 2),
            MontoDevuelto = 0,
            EstadoDevolucion = EstadoDevolucion.PENDIENTE,
            MotivoGeneracion = motivo.Trim(),
            Observacion = "Generada por anulacion de compra con pagos historicos.",
            UsuarioRegistro = empresaContext.UsuarioNombre
        };
        ValidarTercero(devolucion);

        await devolucionRepository.GuardarAsync(devolucion, cancellationToken);
        return devolucion;
    }

    private async Task<Devolucion> ObtenerEntidadAsync(int id, CancellationToken cancellationToken) =>
        await devolucionRepository.ObtenerAsync(empresaContext.EmpresaId, id, cancellationToken)
            ?? throw new InvalidOperationException("Devolucion no encontrada.");

    private static void ValidarTercero(Devolucion devolucion)
    {
        if (devolucion.TipoTercero == TipoTerceroDevolucion.CLIENTE && (devolucion.ClienteId is null || devolucion.ProveedorId is not null))
        {
            throw new InvalidOperationException("La devolucion de cliente requiere ClienteId y no debe tener ProveedorId.");
        }

        if (devolucion.TipoTercero == TipoTerceroDevolucion.PROVEEDOR && (devolucion.ProveedorId is null || devolucion.ClienteId is not null))
        {
            throw new InvalidOperationException("La devolucion de proveedor requiere ProveedorId y no debe tener ClienteId.");
        }
    }

    private static DevolucionDetalleDto ToDetalleDto(Devolucion devolucion) => new()
    {
        DevolucionId = devolucion.DevolucionId,
        FechaGeneracion = devolucion.FechaGeneracion,
        TipoTercero = devolucion.TipoTercero,
        Tercero = Tercero(devolucion),
        Origen = devolucion.Origen,
        OrigenDescripcion = OrigenDescripcion(devolucion.Origen),
        DocumentoOrigen = Documento(devolucion),
        MontoOriginal = devolucion.MontoOriginal,
        MontoDevuelto = devolucion.MontoDevuelto,
        MontoPendiente = devolucion.MontoPendiente,
        Estado = devolucion.EstadoDevolucion,
        Observacion = devolucion.Observacion,
        MotivoGeneracion = devolucion.MotivoGeneracion
    };

    private static string Tercero(Devolucion devolucion) =>
        devolucion.TipoTercero == TipoTerceroDevolucion.CLIENTE
            ? devolucion.Cliente?.NombreCompleto ?? string.Empty
            : devolucion.Proveedor?.RazonSocial ?? string.Empty;

    private static string Documento(Devolucion devolucion)
    {
        if (devolucion.NotaPedido is not null) return $"{devolucion.NotaPedido.Serie}-{devolucion.NotaPedido.Correlativo:000000}";
        if (devolucion.NotaCredito is not null) return $"{devolucion.NotaCredito.Serie}-{devolucion.NotaCredito.Correlativo:000000}";
        if (devolucion.Comprobante is not null) return $"{devolucion.Comprobante.Serie}-{devolucion.Comprobante.Correlativo:000000}";
        if (devolucion.Compra is not null) return devolucion.Compra.Documento;
        return "-";
    }

    private static string OrigenDescripcion(OrigenDevolucion origen) => origen switch
    {
        OrigenDevolucion.ANULACION_NOTA_PEDIDO => "Anulacion Nota Pedido",
        OrigenDevolucion.NOTA_CREDITO => "Nota de Credito",
        OrigenDevolucion.ANULACION_COMPRA => "Anulacion Compra",
        OrigenDevolucion.ANULACION_COMPROBANTE => "Anulacion Comprobante",
        _ => origen.ToString()
    };
}
