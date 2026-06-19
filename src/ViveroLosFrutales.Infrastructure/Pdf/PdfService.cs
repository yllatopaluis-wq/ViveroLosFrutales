using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ViveroLosFrutales.Application.Interfaces;
using ViveroLosFrutales.Domain.Entities;
using ViveroLosFrutales.Domain.Enums;

namespace ViveroLosFrutales.Infrastructure.Pdf;

public class PdfService(IWebHostEnvironment environment, IOptions<PdfOptions> optionsAccessor) : IPdfService
{
    private readonly IWebHostEnvironment _environment = environment;
    private readonly PdfOptions _options = optionsAccessor.Value;

    public Task<string> GenerarCotizacionAsync(Cotizacion cotizacion, CancellationToken cancellationToken)
    {
        var comprobanteShape = new Comprobante
        {
            EmpresaId = cotizacion.EmpresaId,
            Empresa = cotizacion.Empresa,
            Cliente = cotizacion.Cliente,
            ClienteId = cotizacion.ClienteId,
            TipoComprobante = TipoComprobante.COT,
            Serie = cotizacion.Serie,
            Correlativo = cotizacion.Correlativo,
            FechaEmision = cotizacion.FechaEmision,
            Direccion = cotizacion.Direccion,
            FormaPago = cotizacion.FormaPago,
            EmpresaRazonSocial = cotizacion.EmpresaRazonSocial,
            EmpresaNombreComercial = cotizacion.EmpresaNombreComercial,
            EmpresaRuc = cotizacion.EmpresaRuc,
            EmpresaDireccion = cotizacion.EmpresaDireccion,
            EmpresaTelefono = cotizacion.EmpresaTelefono,
            EmpresaEmail = cotizacion.EmpresaEmail,
            CondicionesVenta = cotizacion.CondicionesVenta,
            CaracteristicasTecnicas = cotizacion.CaracteristicasTecnicas,
            SubTotal = cotizacion.SubTotal,
            Igv = cotizacion.Igv,
            Total = cotizacion.Total
        };

        foreach (var detalle in cotizacion.Detalles)
        {
            comprobanteShape.Detalles.Add(new ComprobanteDetalle
            {
                ProductoId = detalle.ProductoId,
                Producto = detalle.Producto,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                Importe = detalle.Importe,
                ImporteIgv = detalle.ImporteIgv
            });
        }

        return GenerarComprobanteLocalAsync(comprobanteShape, cancellationToken);
    }

    public Task<string> GenerarComprobanteLocalAsync(Comprobante comprobante, CancellationToken cancellationToken)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var configuredBasePath = _options.BasePath;
        var basePath = string.IsNullOrWhiteSpace(configuredBasePath)
            ? Path.Combine(_environment.WebRootPath, "documentos")
            : (Path.IsPathRooted(configuredBasePath)
                ? configuredBasePath
                : Path.Combine(_environment.WebRootPath, configuredBasePath));

        var directory = comprobante.TipoComprobante == Domain.Enums.TipoComprobante.COT
            ? Path.Combine(basePath, comprobante.FechaEmision.Year.ToString())
            : Path.Combine(basePath, comprobante.EmpresaId.ToString(), comprobante.TipoComprobante.ToString());

        var fileName = GetFileName(comprobante);
        var absolutePath = Path.GetFullPath(Path.Combine(directory, fileName));

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        if (comprobante.TipoComprobante == TipoComprobante.NPE)
        {
            GenerateNotaPedidoPdf(comprobante, absolutePath);
            return Task.FromResult(ToPublicPath(absolutePath));
        }

        var logoBytes = LoadLogoBytes(comprobante);
        var empresaRazonSocial = !string.IsNullOrWhiteSpace(comprobante.EmpresaRazonSocial)
            ? comprobante.EmpresaRazonSocial
            : comprobante.Empresa?.RazonSocial ?? "Empresa emisora";
        var empresaNombreComercial = !string.IsNullOrWhiteSpace(comprobante.EmpresaNombreComercial)
            ? comprobante.EmpresaNombreComercial
            : comprobante.Empresa?.NombreComercial ?? string.Empty;
        var empresaRuc = !string.IsNullOrWhiteSpace(comprobante.EmpresaRuc)
            ? comprobante.EmpresaRuc
            : comprobante.Empresa?.RUC ?? string.Empty;
        var empresaDireccion = !string.IsNullOrWhiteSpace(comprobante.EmpresaDireccion)
            ? comprobante.EmpresaDireccion
            : comprobante.Empresa?.Direccion ?? string.Empty;
        var empresaTelefono = !string.IsNullOrWhiteSpace(comprobante.EmpresaTelefono)
            ? comprobante.EmpresaTelefono
            : comprobante.Empresa?.Telefono ?? string.Empty;
        var empresaEmail = !string.IsNullOrWhiteSpace(comprobante.EmpresaEmail)
            ? comprobante.EmpresaEmail
            : comprobante.Empresa?.Email ?? string.Empty;
        var serie = comprobante.TipoComprobante == Domain.Enums.TipoComprobante.COT
            ? comprobante.Empresa?.SerieCotizacion ?? comprobante.Serie
            : comprobante.Serie;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(28);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(header =>
                {
                    header.Item().Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            if (logoBytes != null)
                            {
                                column.Item().Height(70).Width(140).Image(logoBytes).FitArea();
                            }

                            if (!string.IsNullOrWhiteSpace(empresaNombreComercial))
                            {
                                column.Item().Text(empresaNombreComercial).FontSize(13).Bold();
                            }

                            column.Item().Text(empresaRazonSocial);

                            if (!string.IsNullOrWhiteSpace(empresaRuc))
                            {
                                column.Item().Text($"RUC: {empresaRuc}");
                            }

                            if (!string.IsNullOrWhiteSpace(empresaDireccion))
                                column.Item().Text($"Dirección: {empresaDireccion}");

                            if (!string.IsNullOrWhiteSpace(empresaTelefono))
                                column.Item().Text($"Teléfono: {empresaTelefono}");

                            if (!string.IsNullOrWhiteSpace(empresaEmail))
                                column.Item().Text($"Correo: {empresaEmail}");
                        });

                        row.ConstantItem(220).Column(box =>
                        {
                            box.Item().Padding(10).Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten3).Column(inner =>
                            {
                                inner.Item().Text("COTIZACIÓN").FontSize(16).Bold().AlignCenter();
                                inner.Item().Text($"{serie}-{comprobante.Correlativo:000000}").FontSize(14).SemiBold().AlignCenter();
                                inner.Item().PaddingTop(8).Text($"Fecha: {comprobante.FechaEmision:dd/MM/yyyy}").AlignCenter();
                            });
                        });
                    });
                });

                page.Content().Column(column =>
                {
                    column.Item().PaddingVertical(10).Row(row =>
                    {
                        row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(clientBox =>
                        {
                            clientBox.Item().Text("DATOS DEL CLIENTE").Bold().FontSize(11);
                            clientBox.Item().PaddingTop(5).Text($"Atención: {comprobante.Cliente?.NombreCompleto ?? string.Empty}");
                            clientBox.Item().Text($"RUC: {comprobante.Cliente?.NumeroDocumento ?? string.Empty}");
                            clientBox.Item().Text($"Dirección: {comprobante.Direccion}");
                        });
                    });

                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn(4);
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(50);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Text("ITEM").Bold().AlignCenter();
                            header.Cell().Background(Colors.Grey.Lighten3).Text("PRODUCTO").Bold();
                            header.Cell().Background(Colors.Grey.Lighten3).Text("CANTIDAD").Bold().AlignCenter();
                            header.Cell().Background(Colors.Grey.Lighten3).Text("UNIDAD").Bold().AlignCenter();
                            header.Cell().Background(Colors.Grey.Lighten3).Text("PRECIO UNITARIO (S/)").Bold().AlignRight();
                            header.Cell().Background(Colors.Grey.Lighten3).Text("TOTAL").Bold().AlignRight();
                        });

                        var detalles = comprobante.Detalles.ToList();
                        for (var index = 0; index < Math.Max(detalles.Count, 7); index++)
                        {
                            if (index < detalles.Count)
                            {
                                var detalle = detalles[index];
                                table.Cell().AlignCenter().Text((index + 1).ToString());
                                table.Cell().Text(detalle.Producto?.Nombre ?? detalle.ProductoId.ToString());
                                table.Cell().AlignRight().Text(detalle.Cantidad.ToString("N2"));
                                table.Cell().AlignCenter().Text(FormatearUnidadMedida(detalle.Producto?.UnidadMedida));
                                table.Cell().AlignRight().Text($"{detalle.PrecioUnitario:N2}");
                                table.Cell().AlignRight().Text($"{(detalle.Importe + detalle.ImporteIgv):N2}");
                            }
                            else
                            {
                                table.Cell().Text(" ");
                                table.Cell().Text(" ");
                                table.Cell().Text(" ");
                                table.Cell().Text(" ");
                                table.Cell().Text(" ");
                                table.Cell().Text(" ");
                            }
                        }
                    });

                    column.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem();
                        row.ConstantItem(220).Column(summary =>
                        {
                            summary.Item().AlignRight().Text($"Subtotal: S/ {comprobante.SubTotal:N2}");
                            summary.Item().AlignRight().Text($"IGV: S/ {comprobante.Igv:N2}");
                            summary.Item().AlignRight().Text($"TOTAL: S/ {comprobante.Total:N2}").Bold().FontSize(12);
                        });
                    });

                    column.Item().PaddingTop(15).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(conditions =>
                    {
                        conditions.Item().Text("CONDICIÓN DE VENTA").Bold().FontSize(11);
                        AddMultilineBulletText(conditions, comprobante.CondicionesVenta,
                            "Plazo de entrega: Entrega programada a los 6 meses después de aceptación de cotización.",
                            "Lugar de entrega: En las instalaciones del Vivero (Huaral)",
                            "Forma de pago: Contado.",
                            "Garantía: 1 meses",
                            "Medios de pago: En efectivo, depósito a cuenta corriente");
                    });

                    column.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(features =>
                    {
                        features.Item().Text("CARACTERÍSTICAS TÉCNICAS").Bold().FontSize(11);
                        AddMultilineBulletText(features, comprobante.CaracteristicasTecnicas,
                            "Semillas vegetativas seleccionadas de campos certificados con control fitosanitario.",
                            "Edad: Plantas de 5 meses.",
                            "Tamaño de 50 – 70 cm",
                            "Bolsas medidas de 7 x 12",
                            "Peso aprox. por planta 3 kilos");
                    });
                });
            });
        }).GeneratePdf(absolutePath);

        return Task.FromResult(ToPublicPath(absolutePath));
    }

    private void GenerateNotaPedidoPdf(Comprobante comprobante, string absolutePath)
    {
        var empresaRazonSocial = FirstNotEmpty(comprobante.EmpresaRazonSocial, comprobante.Empresa?.RazonSocial, "Empresa emisora");
        var empresaNombreComercial = FirstNotEmpty(comprobante.EmpresaNombreComercial, comprobante.Empresa?.NombreComercial, empresaRazonSocial);
        var empresaRuc = FirstNotEmpty(comprobante.EmpresaRuc, comprobante.Empresa?.RUC, string.Empty);
        var empresaDireccion = FirstNotEmpty(comprobante.EmpresaDireccion, comprobante.Empresa?.Direccion, string.Empty);
        var empresaTelefono = FirstNotEmpty(comprobante.EmpresaTelefono, comprobante.Empresa?.Telefono, string.Empty);
        var empresaEmail = FirstNotEmpty(comprobante.EmpresaEmail, comprobante.Empresa?.Email, string.Empty);
        var clienteDocumentoLabel = comprobante.Cliente?.TipoDocumento == TipoDocumentoCliente.RUC ? "RUC" : "DOCUMENTO";
        var clienteDocumento = comprobante.Cliente?.NumeroDocumento ?? string.Empty;
        var clienteNombre = comprobante.Cliente?.NombreCompleto ?? string.Empty;
        var moneda = comprobante.Empresa?.MonedaPredeterminada ?? "SOLES";
        var direccionCliente = FirstNotEmpty(comprobante.Direccion, comprobante.Cliente?.Direccion, string.Empty);
        var exonerado = comprobante.Detalles
            .Where(x => x.ImporteIgv == 0)
            .Sum(x => x.PrecioUnitario * x.Cantidad);
        var gravado = comprobante.Detalles
            .Where(x => x.ImporteIgv != 0)
            .Sum(x => x.Importe);

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(26);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                page.Content().Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().PaddingTop(4).Column(left =>
                        {
                            left.Item().Text(empresaNombreComercial).FontSize(22).Bold();
                            left.Item().Text(empresaRazonSocial.ToUpperInvariant()).FontSize(8);
                            if (!string.IsNullOrWhiteSpace(empresaDireccion))
                            {
                                left.Item().Text(empresaDireccion.ToUpperInvariant()).FontSize(8);
                            }
                            if (!string.IsNullOrWhiteSpace(empresaTelefono))
                            {
                                left.Item().Text($"CELULAR : {empresaTelefono}").FontSize(8);
                            }
                            if (!string.IsNullOrWhiteSpace(empresaEmail))
                            {
                                left.Item().Text($"EMAIL : {empresaEmail.ToUpperInvariant()}").FontSize(8);
                            }
                        });

                        row.ConstantItem(250).Height(76).Border(1).PaddingTop(6).Column(box =>
                        {
                            box.Item().Text($"RUC: {empresaRuc}").FontSize(13).Bold().AlignCenter();
                            box.Item().Text("NOTA DE PEDIDO").FontSize(13).Bold().AlignCenter();
                            box.Item().Text($"{comprobante.Serie}-{comprobante.Correlativo:00000000}").FontSize(13).Bold().AlignCenter();
                        });
                    });

                    column.Item().PaddingTop(14).Row(row =>
                    {
                        row.RelativeItem().Column(client =>
                        {
                            client.Item().Row(r =>
                            {
                                r.ConstantItem(100).Text(clienteDocumentoLabel).Bold();
                                r.ConstantItem(10).Text(":");
                                r.RelativeItem().Text(clienteDocumento);
                            });
                            client.Item().Row(r =>
                            {
                                r.ConstantItem(100).Text("RAZON SOCIAL").Bold();
                                r.ConstantItem(10).Text(":");
                                r.RelativeItem().Text(clienteNombre.ToUpperInvariant());
                            });
                            client.Item().Row(r =>
                            {
                                r.ConstantItem(100).Text("DIRECCION").Bold();
                                r.ConstantItem(10).Text(":");
                                r.RelativeItem().Text(direccionCliente.ToUpperInvariant());
                            });
                        });

                        row.ConstantItem(265).Column(meta =>
                        {
                            AddMetaLine(meta, "FECHA EMISION", comprobante.FechaEmision.ToString("dd/MM/yyyy"));
                            AddMetaLine(meta, "FECHA VENCIMIENTO", comprobante.FechaEmision.ToString("dd/MM/yyyy"));
                            AddMetaLine(meta, "MONEDA", moneda.ToUpperInvariant());
                            AddMetaLine(meta, "FORMA PAGO", comprobante.FormaPago.ToString().ToUpperInvariant());
                        });
                    });

                    column.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(38);
                            columns.ConstantColumn(58);
                            columns.ConstantColumn(56);
                            columns.RelativeColumn();
                            columns.ConstantColumn(78);
                            columns.ConstantColumn(88);
                        });

                        AddNotaPedidoHeader(table, "ITEM");
                        AddNotaPedidoHeader(table, "CANTIDAD");
                        AddNotaPedidoHeader(table, "U/M");
                        AddNotaPedidoHeader(table, "PRODUCTO");
                        AddNotaPedidoHeader(table, "P/U");
                        AddNotaPedidoHeader(table, "IMPORTE");

                        var index = 1;
                        foreach (var detalle in comprobante.Detalles)
                        {
                            var importeLinea = decimal.Round(detalle.PrecioUnitario * detalle.Cantidad, 2);
                            AddNotaPedidoCell(table, index.ToString(), "center");
                            AddNotaPedidoCell(table, detalle.Cantidad.ToString("N0"), "center");
                            AddNotaPedidoCell(table, FormatearUnidadMedida(detalle.Producto?.UnidadMedida), "center");
                            AddNotaPedidoCell(table, (detalle.Producto?.Nombre ?? detalle.ProductoId.ToString()).ToUpperInvariant(), fontSize: 7);
                            AddNotaPedidoCell(table, $"S/. {detalle.PrecioUnitario:N2}", "right");
                            AddNotaPedidoCell(table, $"S/. {importeLinea:N2}", "right");
                            index++;
                        }
                    });

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().PaddingTop(5).Text($"SON: {MoneyToSpanishWords(comprobante.Total)} SOLES.").Bold().FontSize(8);
                        row.ConstantItem(262).Table(summary =>
                        {
                            summary.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(126);
                            });

                            AddSummaryRow(summary, "EXONERADO:", $"S/. {exonerado:N2}");
                            AddSummaryRow(summary, "GRAVADO:", $"S/. {gravado:N2}");
                            AddSummaryRow(summary, "IGV:", $"S/. {comprobante.Igv:N2}");
                            AddSummaryRow(summary, "TOTAL:", $"S/. {comprobante.Total:N2}", true);
                        });
                    });

                    column.Item().PaddingTop(16).Text("NOTA:").Bold();
                    column.Item().PaddingTop(14).Row(row =>
                    {
                        row.ConstantItem(48).Text("LUGAR:").FontSize(8);
                        row.RelativeItem().Text("AV LOS NATURALES CDRA 11").FontSize(8);
                    });
                    column.Item().PaddingTop(10).Row(row =>
                    {
                        row.ConstantItem(66).Text("REFERENCIA:").FontSize(8);
                        row.RelativeItem().Text("CAMINO AL CEMENTERIO LOS NATURALES, PASANDO LA PERRERA MUNICIPAL LOS NATURALES").FontSize(8);
                    });
                    column.Item().PaddingTop(14).Text("CANJEAR POR BOLETA O FACTURA").FontSize(8);
                });
            });
        }).GeneratePdf(absolutePath);
    }

    private static void AddMultilineBulletText(ColumnDescriptor container, string content, params string[] defaultLines)
    {
        var lines = string.IsNullOrWhiteSpace(content)
            ? defaultLines
            : content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            container.Item().PaddingTop(2).Text($"• {line.Trim()}").FontSize(10);
        }
    }

    private byte[]? LoadLogoBytes(Comprobante? comprobante = null)
    {
        var logoPath = !string.IsNullOrWhiteSpace(comprobante?.Empresa?.LogoPath)
            ? comprobante.Empresa.LogoPath
            : _options.LogoPath;
        if (string.IsNullOrWhiteSpace(logoPath))
        {
            var defaultLogoPath = Path.Combine(_environment.WebRootPath, "images", "logo.png");
            if (File.Exists(defaultLogoPath))
            {
                return File.ReadAllBytes(defaultLogoPath);
            }

            return null;
        }

        var path = Path.IsPathRooted(logoPath)
            ? logoPath
            : Path.Combine(_environment.WebRootPath, logoPath);

        return File.Exists(path) ? File.ReadAllBytes(path) : null;
    }

    private string ToPublicPath(string absolutePath)
    {
        if (absolutePath.StartsWith(Path.GetFullPath(_environment.WebRootPath), StringComparison.OrdinalIgnoreCase))
        {
            var relativePath = Path.GetRelativePath(_environment.WebRootPath, absolutePath);
            return "/" + relativePath.Replace("\\", "/");
        }

        return absolutePath;
    }

    private static string FirstNotEmpty(params string?[] values) =>
        values.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim() ?? string.Empty;

    private static void AddMetaLine(ColumnDescriptor column, string label, string value)
    {
        column.Item().Row(row =>
        {
            row.ConstantItem(120).Text(label).Bold();
            row.ConstantItem(10).Text(":");
            row.RelativeItem().Text(value);
        });
    }

    private static void AddNotaPedidoHeader(TableDescriptor table, string text) =>
        table.Cell().Border(1).Background(Colors.Grey.Lighten2).Padding(2).Text(text).Bold().AlignCenter();

    private static void AddNotaPedidoCell(TableDescriptor table, string text, string alignment = "left", int? fontSize = null)
    {
        var descriptor = table.Cell().Border(1).Padding(2);
        var textDescriptor = descriptor.Text(text);
        if (fontSize is not null) textDescriptor.FontSize(fontSize.Value);
        if (alignment == "center") textDescriptor.AlignCenter();
        if (alignment == "right") textDescriptor.AlignRight();
    }

    private static string FormatearUnidadMedida(string? unidad)
    {
        return unidad?.Trim().ToUpperInvariant() switch
        {
            null or "" => "UNIDAD",
            "NIU" or "UND" or "UNIDAD" or "UNIDADES" => "UNIDAD",
            "KGM" => "KG",
            "MTR" => "METRO",
            "LTR" => "LITRO",
            var valor => valor
        };
    }

    private static void AddSummaryRow(TableDescriptor table, string label, string value, bool bold = false)
    {
        var labelCell = table.Cell().Border(1).Padding(3).Text(label).AlignRight();
        var valueCell = table.Cell().Border(1).Padding(3).Text(value).AlignRight();
        if (bold)
        {
            labelCell.Bold();
            valueCell.Bold();
        }
        else
        {
            labelCell.SemiBold();
            valueCell.SemiBold();
        }
    }

    private static string MoneyToSpanishWords(decimal value)
    {
        var integer = (long)Math.Truncate(value);
        var cents = (int)Math.Round((value - integer) * 100, 0);
        return $"{NumberToSpanishWords(integer).ToUpperInvariant()} CON {cents:00}/100";
    }

    private static string NumberToSpanishWords(long value)
    {
        if (value == 0) return "cero";
        if (value < 0) return "menos " + NumberToSpanishWords(Math.Abs(value));

        if (value >= 1_000_000)
        {
            var millions = value / 1_000_000;
            var remainder = value % 1_000_000;
            var prefix = millions == 1 ? "un millon" : $"{NumberToSpanishWords(millions)} millones";
            return remainder == 0 ? prefix : $"{prefix} {NumberToSpanishWords(remainder)}";
        }

        if (value >= 1000)
        {
            var thousands = value / 1000;
            var remainder = value % 1000;
            var prefix = thousands == 1 ? "mil" : $"{NumberToSpanishWords(thousands)} mil";
            return remainder == 0 ? prefix : $"{prefix} {NumberToSpanishWords(remainder)}";
        }

        if (value >= 100)
        {
            var hundreds = value / 100;
            var remainder = value % 100;
            var names = new[] { "", "ciento", "doscientos", "trescientos", "cuatrocientos", "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos" };
            if (value == 100) return "cien";
            return remainder == 0 ? names[hundreds] : $"{names[hundreds]} {NumberToSpanishWords(remainder)}";
        }

        var units = new[]
        {
            "", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "diez",
            "once", "doce", "trece", "catorce", "quince", "dieciseis", "diecisiete", "dieciocho", "diecinueve"
        };
        if (value < 20) return units[value];

        if (value < 30)
        {
            return value == 20 ? "veinte" : "veinti" + units[value - 20];
        }

        var tens = new[] { "", "", "", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa" };
        var ten = value / 10;
        var unit = value % 10;
        return unit == 0 ? tens[ten] : $"{tens[ten]} y {units[unit]}";
    }

    private static string GetFileName(Comprobante comprobante)
    {
        if (comprobante.TipoComprobante == Domain.Enums.TipoComprobante.COT)
        {
            var document = comprobante.Cliente?.NumeroDocumento?.Trim() ?? "SIN_DOCUMENTO";
            var customer = comprobante.Cliente?.NombreCompleto?.Trim() ?? "SIN_NOMBRE";
            return NormalizeFileName($"{comprobante.TipoComprobante}{comprobante.FechaEmision:yyyyMMdd}{comprobante.Correlativo:00} - {document} {customer}.pdf");
        }

        return NormalizeFileName($"{comprobante.Serie}-{comprobante.Correlativo:000000}.pdf");
    }

    private static string NormalizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Concat(fileName.Select(c => invalidChars.Contains(c) ? '_' : c));
    }
}
