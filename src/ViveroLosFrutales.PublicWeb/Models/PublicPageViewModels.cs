using System.ComponentModel.DataAnnotations;
using ViveroLosFrutales.Application.DTOs;

namespace ViveroLosFrutales.PublicWeb.Models;

public class PublicPageViewModel
{
    public PublicEmpresaDto Empresa { get; set; } = new("Vivero Los Frutales", "Vivero Los Frutales", "", "Huaral, Lima, Peru", "", "");
    public IReadOnlyList<PublicProductoDto> Productos { get; set; } = Array.Empty<PublicProductoDto>();
}

public class ContactoViewModel
{
    public PublicEmpresaDto Empresa { get; set; } = new("Vivero Los Frutales", "Vivero Los Frutales", "", "Huaral, Lima, Peru", "", "");

    [Required(ErrorMessage = "Ingrese su nombre.")]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese un telefono o WhatsApp.")]
    [StringLength(30)]
    public string Telefono { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Ingrese un correo valido.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Cuéntenos qué necesita.")]
    [StringLength(1000)]
    public string Mensaje { get; set; } = string.Empty;
}
