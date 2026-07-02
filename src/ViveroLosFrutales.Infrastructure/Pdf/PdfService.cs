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

        if (comprobante.TipoComprobante == TipoComprobante.COT)
        {
            GenerateCotizacionPdf(comprobante, absolutePath);
            return Task.FromResult(ToPublicPath(absolutePath));
        }

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
                                column.Item().Text(empresaNombreComercial.ToUpperInvariant()).FontSize(13).Bold();
                            }

                            column.Item().Text(empresaRazonSocial.ToUpperInvariant()).FontSize(8);

                            if (!string.IsNullOrWhiteSpace(empresaRuc))
                            {
                                column.Item().Text($"RUC: {empresaRuc}").FontSize(8);
                            }

                            if (!string.IsNullOrWhiteSpace(empresaDireccion))
                                column.Item().Text($"DIRECCION: {empresaDireccion.ToUpperInvariant()}").FontSize(8);

                            if (!string.IsNullOrWhiteSpace(empresaTelefono))
                                column.Item().Text($"CELULAR : {empresaTelefono}").FontSize(8);

                            if (!string.IsNullOrWhiteSpace(empresaEmail))
                                column.Item().Text($"EMAIL : {empresaEmail.ToUpperInvariant()}").FontSize(8);
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

                    column.Item().PaddingTop(12).Table(table =>
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

    private void GenerateCotizacionPdf(Comprobante comprobante, string absolutePath)
    {
        var logoBytes = LoadLogoBytes(comprobante);
        var empresaRazonSocial = FirstNotEmpty(comprobante.EmpresaRazonSocial, comprobante.Empresa?.RazonSocial, "Empresa emisora");
        var empresaNombreComercial = FirstNotEmpty(comprobante.EmpresaNombreComercial, comprobante.Empresa?.NombreComercial, empresaRazonSocial);
        var empresaRuc = FirstNotEmpty(comprobante.EmpresaRuc, comprobante.Empresa?.RUC, string.Empty);
        var empresaDireccion = FirstNotEmpty(comprobante.EmpresaDireccion, comprobante.Empresa?.Direccion, string.Empty);
        var empresaTelefono = FirstNotEmpty(comprobante.EmpresaTelefono, comprobante.Empresa?.Telefono, string.Empty);
        var empresaEmail = FirstNotEmpty(comprobante.EmpresaEmail, comprobante.Empresa?.Email, string.Empty);
        var firmaBytes = comprobante.Empresa?.FirmaContenido is { Length: > 0 } firma ? firma : null;
        var serie = FirstNotEmpty(comprobante.Serie, comprobante.Empresa?.SerieCotizacion);
        var clienteDocumentoLabel = comprobante.Cliente?.TipoDocumento == TipoDocumentoCliente.RUC ? "RUC" : "Documento";
        var clienteDocumento = comprobante.Cliente?.NumeroDocumento ?? string.Empty;
        var clienteNombre = comprobante.Cliente?.NombreCompleto ?? string.Empty;
        var clienteDireccion = FirstNotEmpty(comprobante.Direccion, comprobante.Cliente?.Direccion, string.Empty);
        var detalles = comprobante.Detalles.ToList();

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(28);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                page.Content().Column(column =>
                {
                    column.Item().ShowEntire().Row(row =>
                    {
                        row.ConstantItem(96).Height(86).AlignMiddle().AlignCenter().Element(logo =>
                        {
                            if (logoBytes is not null)
                            {
                                logo.Image(logoBytes).FitArea();
                            }
                            else
                            {
                                logo.Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().AlignMiddle().Text("LOGO").FontSize(10).Bold();
                            }
                        });

                        row.RelativeItem().PaddingLeft(12).PaddingTop(4).Column(company =>
                        {
                            company.Item().Text(empresaNombreComercial.ToUpperInvariant()).FontSize(14).Bold();
                            company.Item().Text(empresaRazonSocial).FontSize(9).SemiBold();
                            AddCotizacionInfoLine(company, "RUC", empresaRuc);
                            AddCotizacionInfoLine(company, "Direccion", empresaDireccion);
                            AddCotizacionInfoLine(company, "Celular", empresaTelefono);
                            AddCotizacionInfoLine(company, "Email", empresaEmail);
                        });

                        row.ConstantItem(180).Height(100).Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(10).Column(box =>
                        {
                            box.Item().Text("COTIZACION").FontSize(19).Bold().AlignCenter();
                            box.Item().Text($"{serie}-{comprobante.Correlativo:000000}").FontSize(14).SemiBold().AlignCenter();
                            box.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            box.Item().Text($"Fecha: {comprobante.FechaEmision:dd/MM/yyyy}").FontSize(8);
                            box.Item().Text("Vigencia: 15 dias calendario").FontSize(8);
                        });
                    });

                    column.Item().PaddingTop(14).Border(1).BorderColor(Colors.Grey.Lighten1).Column(clientBox =>
                    {
                        AddCotizacionSectionTitle(clientBox, "DATOS DEL CLIENTE");
                        clientBox.Item().Padding(9).Column(lines =>
                        {
                            AddCotizacionLabelValue(lines, "Atencion", clienteNombre.ToUpperInvariant());
                            AddCotizacionLabelValue(lines, clienteDocumentoLabel, clienteDocumento);
                            AddCotizacionLabelValue(lines, "Direccion", clienteDireccion.ToUpperInvariant());
                        });
                    });

                    column.Item().PaddingTop(12).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(44);
                            columns.RelativeColumn(4.6f);
                            columns.ConstantColumn(78);
                            columns.ConstantColumn(72);
                            columns.ConstantColumn(78);
                            columns.ConstantColumn(78);
                        });

                        AddCotizacionHeaderCell(table, "ITEM");
                        AddCotizacionHeaderCell(table, "PRODUCTO");
                        AddCotizacionHeaderCell(table, "CANTIDAD");
                        AddCotizacionHeaderCell(table, "UNIDAD");
                        AddCotizacionHeaderCell(table, "PRECIO\nUNITARIO");
                        AddCotizacionHeaderCell(table, "TOTAL");

                        for (var index = 0; index < Math.Max(detalles.Count, 6); index++)
                        {
                            if (index < detalles.Count)
                            {
                                var detalle = detalles[index];
                                var totalLinea = decimal.Round(detalle.Importe + detalle.ImporteIgv, 2);
                                AddCotizacionCell(table, (index + 1).ToString(), "center");
                                AddCotizacionCell(table, (detalle.Producto?.Nombre ?? detalle.ProductoId.ToString()).ToUpperInvariant(), fontSize: 6);
                                AddCotizacionCell(table, detalle.Cantidad.ToString("N2"), "center");
                                AddCotizacionCell(table, FormatearUnidadMedida(detalle.Producto?.UnidadMedida), "center");
                                AddCotizacionCell(table, detalle.PrecioUnitario.ToString("N2"), "right");
                                AddCotizacionCell(table, totalLinea.ToString("N2"), "right");
                            }
                            else
                            {
                                AddCotizacionCell(table, " ", "center");
                                AddCotizacionCell(table, " ");
                                AddCotizacionCell(table, " ", "center");
                                AddCotizacionCell(table, " ", "center");
                                AddCotizacionCell(table, " ", "right");
                                AddCotizacionCell(table, " ", "right");
                            }
                        }
                    });

                    column.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem();
                        row.ConstantItem(220).Column(summary =>
                        {
                            AddCotizacionSummaryRow(summary, "Subtotal", $"S/ {comprobante.SubTotal:N2}");
                            AddCotizacionSummaryRow(summary, "IGV 0%", $"S/ {comprobante.Igv:N2}");
                            summary.Item().Background(Colors.Grey.Darken2).PaddingHorizontal(10).PaddingVertical(6).Row(total =>
                            {
                                total.RelativeItem().Text("TOTAL").FontSize(10).Bold().FontColor(Colors.White);
                                total.ConstantItem(92).Text($"S/ {comprobante.Total:N2}").FontSize(10).Bold().FontColor(Colors.White).AlignRight();
                            });
                        });
                    });

                    column.Item().PaddingTop(12).Row(row =>
                    {
                        row.RelativeItem().PaddingRight(7).Border(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(8).Column(conditions =>
                        {
                            AddCotizacionSectionTitle(conditions, "CONDICION DE VENTA");
                            AddCotizacionBulletText(conditions, comprobante.CondicionesVenta,
                                "Plazo de entrega: A los 10 dias de realizado el pago.",
                                "Lugar de entrega: En las instalaciones del vivero.",
                                $"Forma de pago: {comprobante.FormaPago}.",
                                "Garantia: 1 mes.",
                                "Medio de pago: En efectivo, deposito a cuenta corriente o pago con tarjeta.");
                        });

                        row.RelativeItem().PaddingLeft(7).Border(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(8).Column(features =>
                        {
                            AddCotizacionSectionTitle(features, "CARACTERISTICAS TECNICAS");
                            AddCotizacionBulletText(features, comprobante.CaracteristicasTecnicas,
                                "Semillas seleccionadas de campos certificados con control fitosanitario.",
                                "Edad: Plantas de 14 meses.",
                                "Tamaño de 40 - 50 cm.",
                                "Bolsa: medidas de 7 x 14.",
                                "Peso aprox. por planta: 4 kilos.");
                        });
                    });

                    column.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten4).Padding(8).Row(row =>
                    {
                        AddBenefitBox(row, CotizacionIconShield(), "BENEFICIOS DE\nNUESTRAS PLANTAS", "", featured: true);
                        AddBenefitBox(row, CotizacionIconMedal(), "Mayor rendimiento", "y desarrollo");
                        AddBenefitBox(row, CotizacionIconAdvisor(), "Asesoria tecnica", "");
                        AddBenefitBox(row, CotizacionIconShield(), "Garantia de", "satisfaccion", hasDivider: false);
                    });

                    column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                    column.Item().PaddingTop(12).Row(row =>
                    {
                        row.RelativeItem(1.35f).PaddingRight(12).Column(about =>
                        {
                            about.Item().Text("SOBRE NOSOTROS").FontSize(8).Bold();
                            about.Item().PaddingTop(5).Text("Somos un vivero especializado en la produccion de plantas frutales de alta calidad, comprometidos con la agricultura sostenible y el exito de nuestros clientes.").FontSize(7).LineHeight(1.25f);
                        });

                        row.RelativeItem(0.95f).PaddingHorizontal(8).BorderLeft(1).BorderColor(Colors.Grey.Lighten2).PaddingLeft(10).Column(contact =>
                        {
                            contact.Item().Text("CONTACTANOS").FontSize(8).Bold();
                            AddCotizacionContactLine(contact, empresaTelefono);
                            AddCotizacionContactLine(contact, empresaEmail);
                            AddCotizacionContactLine(contact, empresaNombreComercial);
                            AddCotizacionContactLine(contact, empresaDireccion);
                        });

                        row.ConstantItem(170).Height(86).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).AlignBottom().Column(signature =>
                        {
                            signature.Item().Height(48).AlignCenter().AlignMiddle().Element(firma =>
                            {
                                if (firmaBytes is not null)
                                {
                                    firma.Image(firmaBytes).FitArea();
                                }
                                else
                                {
                                    firma.Text(" ");
                                }
                            });
                            signature.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken1);
                            signature.Item().PaddingTop(4).Text("Firma y Sello").FontSize(7).AlignCenter();
                        });
                    });
                });

                page.Footer().Height(18).Background(Colors.Grey.Darken2).AlignCenter().AlignMiddle()
                    .Text("Gracias por confiar en nosotros!").FontSize(8).FontColor(Colors.White).Italic();
            });
        }).GeneratePdf(absolutePath);
    }
    private void GenerateNotaPedidoPdf(Comprobante comprobante, string absolutePath)
    {
        var logoBytes = LoadLogoBytes(comprobante);
        var empresaRazonSocial = FirstNotEmpty(comprobante.EmpresaRazonSocial, comprobante.Empresa?.RazonSocial, "Empresa emisora");
        var empresaNombreComercial = FirstNotEmpty(comprobante.EmpresaNombreComercial, comprobante.Empresa?.NombreComercial, empresaRazonSocial);
        var empresaRuc = FirstNotEmpty(comprobante.EmpresaRuc, comprobante.Empresa?.RUC, string.Empty);
        var empresaDireccion = FirstNotEmpty(comprobante.EmpresaDireccion, comprobante.Empresa?.Direccion, string.Empty);
        var empresaTelefono = FirstNotEmpty(comprobante.EmpresaTelefono, comprobante.Empresa?.Telefono, string.Empty);
        var empresaEmail = FirstNotEmpty(comprobante.EmpresaEmail, comprobante.Empresa?.Email, string.Empty);
        var clienteDocumentoLabel = comprobante.Cliente?.TipoDocumento == TipoDocumentoCliente.RUC ? "RUC" : "DOCUMENTO";
        var clienteDocumento = comprobante.Cliente?.NumeroDocumento ?? string.Empty;
        var clienteNombre = comprobante.Cliente?.NombreCompleto ?? string.Empty;
        var moneda = FormatearMoneda(comprobante.Empresa?.MonedaPredeterminada);
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
                page.Margin(18);
                page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));

                page.Content().Column(column =>
                {
                    column.Item().ShowEntire().Row(row =>
                    {
                        row.ConstantItem(86).Height(78).AlignMiddle().AlignCenter().Element(logo =>
                        {
                            if (logoBytes is not null)
                            {
                                logo.Image(logoBytes).FitArea();
                            }
                            else
                            {
                                logo.Border(1).BorderColor(Colors.Grey.Lighten2).AlignCenter().AlignMiddle().Text("LOGO").FontSize(9).Bold();
                            }
                        });

                        row.RelativeItem().PaddingLeft(12).PaddingTop(3).Column(company =>
                        {
                            company.Item().Text(empresaNombreComercial.ToUpperInvariant()).FontSize(14).Bold();
                            company.Item().PaddingTop(3).Text(empresaRazonSocial.ToUpperInvariant()).FontSize(8).SemiBold();
                            AddNotaPedidoCompanyLine(company, NotaPedidoIconLocation(), empresaDireccion.ToUpperInvariant());
                            AddNotaPedidoCompanyLine(company, NotaPedidoIconPhone(), empresaTelefono);
                            AddNotaPedidoCompanyLine(company, NotaPedidoIconMail(), empresaEmail.ToUpperInvariant());
                        });

                        row.ConstantItem(195).Border(1).BorderColor(Colors.Grey.Lighten1).Padding(6).Column(box =>
                        {
                            box.Item().Background(Colors.Grey.Darken2).PaddingVertical(5).Text("NOTA DE PEDIDO").FontSize(12).Bold().FontColor(Colors.White).AlignCenter();
                            box.Item().PaddingTop(7).Text($"{comprobante.Serie}-{comprobante.Correlativo:00000000}").FontSize(13).Bold().AlignCenter();
                            box.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            box.Item().Text($"RUC: {empresaRuc}").FontSize(8).Bold().AlignCenter();
                        });
                    });

                    column.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8).Row(row =>
                    {
                        row.RelativeItem().PaddingRight(12).Column(client =>
                        {
                            AddNotaPedidoLabelValue(client, clienteDocumentoLabel, clienteDocumento);
                            AddNotaPedidoLabelValue(client, "RAZON SOCIAL", clienteNombre.ToUpperInvariant());
                            AddNotaPedidoLabelValue(client, "DIRECCION", direccionCliente.ToUpperInvariant());
                        });

                        row.ConstantItem(1).Height(64).Background(Colors.Grey.Lighten2);

                        row.RelativeItem().PaddingLeft(14).Column(meta =>
                        {
                            AddNotaPedidoLabelValue(meta, "FECHA EMISION", comprobante.FechaEmision.ToString("dd/MM/yyyy"));
                            AddNotaPedidoLabelValue(meta, "FECHA VENCIMIENTO", comprobante.FechaEmision.ToString("dd/MM/yyyy"));
                            AddNotaPedidoLabelValue(meta, "MONEDA", moneda.ToUpperInvariant());
                            AddNotaPedidoLabelValue(meta, "FORMA PAGO", comprobante.FormaPago.ToString().ToUpperInvariant());
                        });
                    });

                    column.Item().PaddingTop(12).Table(table =>
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
                        AddNotaPedidoHeader(table, "UNIDAD\nMEDIDA");
                        AddNotaPedidoHeader(table, "PRODUCTO");
                        AddNotaPedidoHeader(table, "PRECIO\nUNITARIO");
                        AddNotaPedidoHeader(table, "IMPORTE");

                        var detalles = comprobante.Detalles.ToList();
                        for (var index = 0; index < Math.Max(detalles.Count, 10); index++)
                        {
                            if (index < detalles.Count)
                            {
                                var detalle = detalles[index];
                                var importeLinea = decimal.Round(detalle.PrecioUnitario * detalle.Cantidad, 2);
                                AddNotaPedidoCell(table, (index + 1).ToString(), "center");
                                AddNotaPedidoCell(table, detalle.Cantidad.ToString("N0"), "center");
                                AddNotaPedidoCell(table, FormatearUnidadMedida(detalle.Producto?.UnidadMedida), "center");
                                AddNotaPedidoCell(table, (detalle.Producto?.Nombre ?? detalle.ProductoId.ToString()).ToUpperInvariant(), fontSize: 7);
                                AddNotaPedidoCell(table, $"S/. {detalle.PrecioUnitario:N2}", "right");
                                AddNotaPedidoCell(table, $"S/. {importeLinea:N2}", "right");
                            }
                            else
                            {
                                AddNotaPedidoCell(table, " ", "center");
                                AddNotaPedidoCell(table, " ", "center");
                                AddNotaPedidoCell(table, " ", "center");
                                AddNotaPedidoCell(table, " ");
                                AddNotaPedidoCell(table, " ", "right");
                                AddNotaPedidoCell(table, " ", "right");
                            }
                        }
                    });

                    column.Item().ShowEntire().Row(row =>
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

                    column.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(9).Row(row =>
                    {
                        row.ConstantItem(22).Height(22).Svg(NotaPedidoIconNote());
                        row.RelativeItem().PaddingLeft(10).Column(note =>
                        {
                            note.Item().Text("NOTA:").FontSize(8).Bold();
                            note.Item().PaddingTop(4).Row(line =>
                            {
                                line.ConstantItem(70).Text("LUGAR:").FontSize(7).Bold();
                                line.RelativeItem().Text("AV LOS NATURALES CDRA 11").FontSize(7);
                            });
                            note.Item().PaddingTop(4).Row(line =>
                            {
                                line.ConstantItem(70).Text("REFERENCIA:").FontSize(7).Bold();
                                line.RelativeItem().Text("CAMINO AL CEMENTERIO LOS NATURALES, PASANDO LA PERRERA MUNICIPAL LOS NATURALES").FontSize(7);
                            });
                            note.Item().PaddingTop(8).Text("CANJEAR POR BOLETA O FACTURA").FontSize(7);
                        });
                    });
                });
            });
        }).GeneratePdf(absolutePath);
    }

    private static void AddCotizacionInfoLine(ColumnDescriptor column, string label, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            column.Item().Text($"{label}: {value}").FontSize(8);
        }
    }

    private static void AddCotizacionSectionTitle(ColumnDescriptor column, string title) =>
        column.Item().Background(Colors.Grey.Lighten3).PaddingHorizontal(9).PaddingVertical(6).Text(title).FontSize(8).Bold();

    private static void AddCotizacionLabelValue(ColumnDescriptor column, string label, string value)
    {
        column.Item().PaddingBottom(5).Row(row =>
        {
            row.ConstantItem(82).Text($"{label}:").FontSize(8).Bold();
            row.RelativeItem().Text(string.IsNullOrWhiteSpace(value) ? "-" : value).FontSize(8);
        });
    }

    private static void AddCotizacionHeaderCell(TableDescriptor table, string text) =>
        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten3).Padding(6).AlignMiddle().Text(text).FontSize(7).Bold().AlignCenter();

    private static void AddCotizacionCell(TableDescriptor table, string text, string alignment = "left", int? fontSize = null)
    {
        var descriptor = table.Cell().MinHeight(29).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).AlignMiddle();
        var textDescriptor = descriptor.Text(text).FontSize(fontSize ?? 8);
        if (alignment == "center") textDescriptor.AlignCenter();
        if (alignment == "right") textDescriptor.AlignRight();
    }

    private static void AddCotizacionSummaryRow(ColumnDescriptor column, string label, string value)
    {
        column.Item().PaddingHorizontal(10).PaddingVertical(2).Row(row =>
        {
            row.RelativeItem().Text(label).FontSize(8);
            row.ConstantItem(92).Text(value).FontSize(8).AlignRight();
        });
    }

    private static void AddCotizacionBulletText(ColumnDescriptor column, string content, params string[] defaultLines)
    {
        var lines = string.IsNullOrWhiteSpace(content)
            ? defaultLines
            : content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            column.Item().PaddingHorizontal(9).PaddingTop(3).Text($"- {line.Trim()}").FontSize(7);
        }
    }

    private static void AddBenefitBox(RowDescriptor row, string iconSvg, string title, string subtitle, bool featured = false, bool hasDivider = true)
    {
        var item = featured ? row.ConstantItem(170) : row.RelativeItem();
        var container = hasDivider
            ? item.PaddingRight(8).BorderRight(1).BorderColor(Colors.Grey.Lighten1).PaddingLeft(4)
            : item.PaddingLeft(4);

        container.Row(content =>
        {
            content.ConstantItem(featured ? 42 : 32).Height(featured ? 42 : 32).AlignMiddle().Svg(iconSvg);
            content.RelativeItem().PaddingLeft(featured ? 10 : 7).AlignMiddle().Column(text =>
            {
                text.Item().Text(title).FontSize(featured ? 8 : 7).Bold();
                if (!string.IsNullOrWhiteSpace(subtitle))
                {
                    text.Item().Text(subtitle).FontSize(6);
                }
            });
        });
    }

    private static string CotizacionIconShield() =>
        """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64">
          <path d="M32 5 L54 14 V29 C54 43 45 54 32 59 C19 54 10 43 10 29 V14 Z" fill="none" stroke="#5f6368" stroke-width="5" stroke-linejoin="round"/>
          <path d="M22 32 L29 39 L43 23" fill="none" stroke="#5f6368" stroke-width="5" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
        """;

    private static string CotizacionIconMedal() =>
        """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64">
          <circle cx="32" cy="24" r="15" fill="none" stroke="#5f6368" stroke-width="4"/>
          <path d="M25 39 L20 57 L32 50 L44 57 L39 39" fill="none" stroke="#5f6368" stroke-width="4" stroke-linejoin="round"/>
          <path d="M32 15 L35 21 L42 22 L37 27 L38 34 L32 31 L26 34 L27 27 L22 22 L29 21 Z" fill="none" stroke="#5f6368" stroke-width="3" stroke-linejoin="round"/>
        </svg>
        """;

    private static string CotizacionIconAdvisor() =>
        """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64">
          <circle cx="32" cy="20" r="11" fill="none" stroke="#5f6368" stroke-width="4"/>
          <path d="M18 56 V48 C18 39 24 34 32 34 C40 34 46 39 46 48 V56" fill="none" stroke="#5f6368" stroke-width="4" stroke-linecap="round"/>
          <path d="M20 21 C21 12 27 8 32 8 C37 8 43 12 44 21" fill="none" stroke="#5f6368" stroke-width="4" stroke-linecap="round"/>
          <path d="M47 24 V30 H41" fill="none" stroke="#5f6368" stroke-width="4" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M24 44 L32 52 L40 44" fill="none" stroke="#5f6368" stroke-width="3" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
        """;

    private static void AddCotizacionContactLine(ColumnDescriptor column, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            column.Item().PaddingTop(3).Text(value).FontSize(7);
        }
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
        if (comprobante?.Empresa?.LogoContenido is { Length: > 0 })
        {
            return comprobante.Empresa.LogoContenido;
        }

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

    private static string FormatContact(params string?[] values)
    {
        var contact = values
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return contact.Length == 0 ? "-" : string.Join(" / ", contact);
    }

    private static void AddMetaLine(ColumnDescriptor column, string label, string value)
    {
        column.Item().Row(row =>
        {
            row.ConstantItem(120).Text(label).Bold();
            row.ConstantItem(10).Text(":");
            row.RelativeItem().Text(value);
        });
    }

    private static void AddNotaPedidoLabelValue(ColumnDescriptor column, string label, string value)
    {
        column.Item().PaddingBottom(5).Row(row =>
        {
            row.ConstantItem(102).Text(label).FontSize(8).Bold();
            row.ConstantItem(10).Text(":").FontSize(8);
            row.RelativeItem().Text(string.IsNullOrWhiteSpace(value) ? "-" : value).FontSize(8);
        });
    }

    private static void AddNotaPedidoCompanyLine(ColumnDescriptor column, string iconSvg, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        column.Item().PaddingTop(4).Row(row =>
        {
            row.ConstantItem(12).Height(12).Svg(iconSvg);
            row.RelativeItem().PaddingLeft(7).Text(value).FontSize(8);
        });
    }

    private static string NotaPedidoIconNote() =>
        """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
          <path d="M6 3 H16 L20 7 V21 H6 Z" fill="none" stroke="#5f6368" stroke-width="2" stroke-linejoin="round"/>
          <path d="M16 3 V8 H20" fill="none" stroke="#5f6368" stroke-width="2" stroke-linejoin="round"/>
          <path d="M9 12 H16 M9 16 H14" fill="none" stroke="#5f6368" stroke-width="2" stroke-linecap="round"/>
          <path d="M4 7 H6 M4 11 H6 M4 15 H6" fill="none" stroke="#5f6368" stroke-width="2" stroke-linecap="round"/>
        </svg>
        """;
    private static string NotaPedidoIconLocation() =>
        """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
          <path d="M12 22 C12 22 5 15 5 9 A7 7 0 0 1 19 9 C19 15 12 22 12 22 Z" fill="#5f6368"/>
          <circle cx="12" cy="9" r="2.6" fill="#ffffff"/>
        </svg>
        """;

    private static string NotaPedidoIconPhone() =>
        """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
          <path d="M7 3 L10 8 L8 10 C9.5 13 11 14.5 14 16 L16 14 L21 17 C20 20 18 21 15 21 C9 21 3 15 3 9 C3 6 4 4 7 3 Z" fill="#5f6368"/>
        </svg>
        """;

    private static string NotaPedidoIconMail() =>
        """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
          <rect x="3" y="5" width="18" height="14" rx="1.5" fill="none" stroke="#5f6368" stroke-width="2"/>
          <path d="M4 7 L12 13 L20 7" fill="none" stroke="#5f6368" stroke-width="2" stroke-linejoin="round"/>
        </svg>
        """;
    private static void AddNotaPedidoHeader(TableDescriptor table, string text) =>
        table.Cell()
            .Border(0.5f)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(Colors.Grey.Lighten3)
            .PaddingHorizontal(3)
            .PaddingVertical(2)
            .AlignMiddle()
            .Text(text)
            .FontSize(7)
            .Bold()
            .AlignCenter();

    private static void AddNotaPedidoCell(TableDescriptor table, string text, string alignment = "left", int? fontSize = null)
    {
        var descriptor = table.Cell()
            .MinHeight(16)
            .Border(0.5f)
            .BorderColor(Colors.Grey.Lighten2)
            .Background(Colors.White)
            .PaddingHorizontal(4)
            .PaddingVertical(2)
            .AlignMiddle();
        var textDescriptor = descriptor.Text(text).FontSize(fontSize ?? 8);
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

    private static string FormatearMoneda(string? moneda)
    {
        return moneda?.Trim().ToUpperInvariant() switch
        {
            null or "" => "SOLES",
            "PEN" or "SOL" or "SOLES" => "SOLES",
            "USD" or "DOLAR" or "DOLARES" => "DOLARES",
            var valor => valor
        };
    }

    private static void AddSummaryRow(TableDescriptor table, string label, string value, bool bold = false)
    {
        var labelCell = table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(label).AlignRight();
        var valueCell = table.Cell().Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(value).AlignRight();
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
