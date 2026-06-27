namespace ViveroLosFrutales.Application.DTOs;

public record DevolucionAlertaDto(
    int DevolucionId,
    DateTime Fecha,
    string Tercero,
    string Documento,
    decimal MontoPendiente,
    string Origen);

public class DevolucionAlertasDto
{
    public int TotalPendientes { get; set; }
    public IReadOnlyList<DevolucionAlertaDto> Recientes { get; set; } = Array.Empty<DevolucionAlertaDto>();
}
