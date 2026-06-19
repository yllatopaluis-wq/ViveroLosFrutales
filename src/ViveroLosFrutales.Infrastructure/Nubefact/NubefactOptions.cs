namespace ViveroLosFrutales.Infrastructure.Nubefact;

public class NubefactOptions
{
    public decimal MontoMinimoDetraccion { get; set; } = 700m;
    public int TipoDetraccionGravada { get; set; } = 35;
    public int TipoDetraccionExonerada { get; set; } = 33;
    public int MedioPagoDetraccion { get; set; } = 1;
    public string FormatoPdf { get; set; } = "A4";
}
