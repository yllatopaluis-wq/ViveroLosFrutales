using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;

namespace ViveroLosFrutales.Infrastructure.Data;

public class DocumentoConfiguracionRepository(ApplicationDbContext db) : IDocumentoConfiguracionRepository
{
    public async Task<FormularioConfiguracion?> ObtenerFormularioAsync(string tipoDocumento, int? empresaId, int? teamId, CancellationToken cancellationToken)
    {
        var query = db.FormularioConfiguraciones.AsNoTracking()
            .Include(x => x.Bloques)
            .Include(x => x.Campos)
            .Include(x => x.ProductoConfiguracion)
            .Where(x => x.Activo && x.TipoDocumento == tipoDocumento);

        return await Resolver(query, empresaId, teamId, x => x.Version, cancellationToken);
    }

    public async Task<CondicionComercialPlantilla?> ObtenerCondicionPlantillaAsync(string tipoDocumento, int? empresaId, int? teamId, CancellationToken cancellationToken)
    {
        var query = db.CondicionComercialPlantillas.AsNoTracking()
            .Include(x => x.Items)
            .Where(x => x.Activa && x.EsPredeterminada && x.TipoDocumento == tipoDocumento);

        return await Resolver(query, empresaId, teamId, x => x.CondicionComercialPlantillaId, cancellationToken);
    }

    public async Task<PlantillaDocumento?> ObtenerPlantillaDocumentoAsync(string tipoDocumento, int? empresaId, int? teamId, CancellationToken cancellationToken)
    {
        var query = db.PlantillaDocumentos.AsNoTracking()
            .Include(x => x.Bloques)
            .Where(x => x.Activa && x.EsPredeterminada && x.TipoDocumento == tipoDocumento);

        return await Resolver(query, empresaId, teamId, x => x.Version, cancellationToken);
    }

    public async Task<IReadOnlyList<CotizacionCondicionSnapshot>> ListarCondicionesSnapshotAsync(int cotizacionId, CancellationToken cancellationToken)
    {
        return await db.CotizacionCondicionSnapshots.AsNoTracking()
            .Where(x => x.CotizacionId == cotizacionId)
            .OrderBy(x => x.Orden)
            .ThenBy(x => x.CotizacionCondicionSnapshotId)
            .ToListAsync(cancellationToken);
    }

    public async Task GuardarConfiguracionDocumentoAsync(int empresaId, string usuario, DocumentoConfiguracionEditDto dto, CancellationToken cancellationToken)
    {
        var tipoDocumento = ViveroLosFrutales.Application.Services.FormularioConfiguracionService.NormalizarTipoDocumento(dto.TipoDocumento);
        var formulario = await db.FormularioConfiguraciones
            .Include(x => x.Bloques)
            .Include(x => x.Campos)
            .Include(x => x.ProductoConfiguracion)
            .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.TeamId == null && x.TipoDocumento == tipoDocumento && x.Activo, cancellationToken);

        if (formulario is null)
        {
            formulario = new FormularioConfiguracion
            {
                EmpresaId = empresaId,
                TipoDocumento = tipoDocumento,
                Nombre = string.IsNullOrWhiteSpace(dto.Nombre) ? ViveroLosFrutales.Application.Services.FormularioConfiguracionService.NombreDocumento(tipoDocumento) : dto.Nombre.Trim(),
                Version = 1,
                Activo = true,
                UsuarioRegistro = usuario
            };
            db.FormularioConfiguraciones.Add(formulario);
        }
        else
        {
            formulario.Nombre = string.IsNullOrWhiteSpace(dto.Nombre) ? formulario.Nombre : dto.Nombre.Trim();
            formulario.Version += 1;
            formulario.UsuarioRegistro = usuario;
        }

        db.FormularioBloqueConfiguraciones.RemoveRange(formulario.Bloques);
        db.FormularioCampoConfiguraciones.RemoveRange(formulario.Campos);
        formulario.Bloques.Clear();
        formulario.Campos.Clear();
        foreach (var bloque in dto.Bloques.OrderBy(x => x.Orden))
        {
            if (string.IsNullOrWhiteSpace(bloque.Bloque)) continue;
            formulario.Bloques.Add(new FormularioBloqueConfiguracion
            {
                Bloque = bloque.Bloque.Trim().ToUpperInvariant(),
                Titulo = bloque.Titulo?.Trim() ?? string.Empty,
                Visible = bloque.Visible,
                Orden = bloque.Orden,
                Colapsado = bloque.Colapsado
            });
        }

        foreach (var campo in dto.Campos.OrderBy(x => x.Bloque).ThenBy(x => x.Orden))
        {
            if (string.IsNullOrWhiteSpace(campo.Bloque) || string.IsNullOrWhiteSpace(campo.Campo)) continue;
            formulario.Campos.Add(new FormularioCampoConfiguracion
            {
                Bloque = campo.Bloque.Trim().ToUpperInvariant(),
                Campo = campo.Campo.Trim(),
                Etiqueta = campo.Etiqueta?.Trim() ?? string.Empty,
                Visible = campo.Visible,
                Obligatorio = campo.Obligatorio,
                SoloLectura = campo.SoloLectura,
                Orden = campo.Orden,
                Ancho = campo.Ancho?.Trim() ?? string.Empty,
                ValorDefecto = string.IsNullOrWhiteSpace(campo.ValorDefecto) ? null : campo.ValorDefecto.Trim()
            });
        }
        if (formulario.ProductoConfiguracion is null)
        {
            formulario.ProductoConfiguracion = new FormularioBloqueProductoConfiguracion();
        }

        formulario.ProductoConfiguracion.UnirProductosDuplicados = dto.ProductoComportamiento.UnirProductosDuplicados;
        formulario.ProductoConfiguracion.CantidadInicial = dto.ProductoComportamiento.CantidadInicial <= 0 ? 1m : dto.ProductoComportamiento.CantidadInicial;
        formulario.ProductoConfiguracion.PermitirEditarPrecio = dto.ProductoComportamiento.PermitirEditarPrecio;
        formulario.ProductoConfiguracion.PermitirDescuento = dto.ProductoComportamiento.PermitirDescuento;
        formulario.ProductoConfiguracion.MostrarStock = dto.ProductoComportamiento.MostrarStock;
        formulario.ProductoConfiguracion.BloquearSinStock = dto.ProductoComportamiento.BloquearSinStock;

        if (dto.Condiciones.Count > 0)
        {
            var plantilla = await db.CondicionComercialPlantillas
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.EmpresaId == empresaId && x.TeamId == null && x.TipoDocumento == tipoDocumento && x.Activa && x.EsPredeterminada, cancellationToken);

            if (plantilla is null)
            {
                plantilla = new CondicionComercialPlantilla
                {
                    EmpresaId = empresaId,
                    TipoDocumento = tipoDocumento,
                    Nombre = $"Condiciones {ViveroLosFrutales.Application.Services.FormularioConfiguracionService.NombreDocumento(tipoDocumento)}",
                    Activa = true,
                    EsPredeterminada = true,
                    UsuarioRegistro = usuario
                };
                db.CondicionComercialPlantillas.Add(plantilla);
            }
            else
            {
                plantilla.UsuarioRegistro = usuario;
            }

            db.CondicionComercialItems.RemoveRange(plantilla.Items);
            plantilla.Items.Clear();
            foreach (var condicion in dto.Condiciones.OrderBy(x => x.Orden))
            {
                if (string.IsNullOrWhiteSpace(condicion.Texto)) continue;
                plantilla.Items.Add(new CondicionComercialItem
                {
                    Etiqueta = condicion.Etiqueta?.Trim() ?? string.Empty,
                    Texto = condicion.Texto.Trim(),
                    Orden = condicion.Orden,
                    Visible = condicion.Visible
                });
            }
        }
        await db.SaveChangesAsync(cancellationToken);
    }
    private static async Task<T?> Resolver<T>(IQueryable<T> query, int? empresaId, int? teamId, Func<T, int> versionSelector, CancellationToken cancellationToken)
        where T : class
    {
        var items = await query.ToListAsync(cancellationToken);
        return items
            .Where(x => Matches(x, empresaId, teamId))
            .OrderBy(x => ScopeOrder(x, empresaId, teamId))
            .ThenByDescending(versionSelector)
            .FirstOrDefault();
    }

    private static bool Matches<T>(T item, int? empresaId, int? teamId)
    {
        var empresa = (int?)item!.GetType().GetProperty("EmpresaId")!.GetValue(item);
        var team = (int?)item.GetType().GetProperty("TeamId")!.GetValue(item);
        return (empresa == empresaId && team == teamId && teamId is not null)
            || (empresa == empresaId && team is null)
            || (empresa is null && team is null);
    }

    private static int ScopeOrder<T>(T item, int? empresaId, int? teamId)
    {
        var empresa = (int?)item!.GetType().GetProperty("EmpresaId")!.GetValue(item);
        var team = (int?)item.GetType().GetProperty("TeamId")!.GetValue(item);
        if (empresa == empresaId && team == teamId && teamId is not null) return 0;
        if (empresa == empresaId && team is null) return 1;
        return 2;
    }
}










