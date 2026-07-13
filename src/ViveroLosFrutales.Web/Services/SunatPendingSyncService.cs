using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Enums;
using ViveroLosFrutales.Infrastructure.Data;

namespace ViveroLosFrutales.Web.Services;

public class SunatPendingSyncService(IServiceScopeFactory scopeFactory, ILogger<SunatPendingSyncService> logger)
{
    private readonly ConcurrentDictionary<int, byte> running = new();

    public void Start(int empresaId)
    {
        if (!running.TryAdd(empresaId, 0))
        {
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await SyncAsync(empresaId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al sincronizar comprobantes pendientes SUNAT de empresa {EmpresaId}.", empresaId);
            }
            finally
            {
                running.TryRemove(empresaId, out _);
            }
        });
    }

    private async Task SyncAsync(int empresaId)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var nubefactService = scope.ServiceProvider.GetRequiredService<INubefactService>();

        var comprobantes = await db.Comprobantes
            .Include(x => x.Empresa)
            .Include(x => x.Cliente)
            .Include(x => x.Detalles)
            .ThenInclude(x => x.Producto)
            .Where(x => x.EmpresaId == empresaId
                && x.Estado == EstadoRegistro.Activo
                && x.EstadoSunat == EstadoSunat.Pendiente
                && (x.TipoComprobante == TipoComprobante.BOL || x.TipoComprobante == TipoComprobante.FAC)
                && !string.IsNullOrEmpty(x.NubefactRespuesta))
            .OrderBy(x => x.FechaRegistro)
            .Take(10)
            .ToListAsync();

        foreach (var comprobante in comprobantes)
        {
            var respuesta = await nubefactService.ConsultarEstadoAsync(comprobante, CancellationToken.None);
            comprobante.EstadoSunat = respuesta.EstadoSunat;
            comprobante.PdfUrl = string.IsNullOrWhiteSpace(respuesta.PdfUrl) ? comprobante.PdfUrl : respuesta.PdfUrl;
            comprobante.XmlUrl = string.IsNullOrWhiteSpace(respuesta.XmlUrl) ? comprobante.XmlUrl : respuesta.XmlUrl;
            comprobante.NubefactHash = string.IsNullOrWhiteSpace(respuesta.Hash) ? comprobante.NubefactHash : respuesta.Hash;
            comprobante.NubefactRespuesta = respuesta.RespuestaCompleta;
            await db.SaveChangesAsync();
        }
    }
}
