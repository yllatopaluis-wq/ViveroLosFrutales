using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViveroLosFrutales.Application.DTOs;
using ViveroLosFrutales.Application.Services;
using ViveroLosFrutales.Infrastructure.Identity;

namespace ViveroLosFrutales.Web.Controllers;

[AllowAnonymous]
public class AccountController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    UsuarioService usuarioService,
    EmpresaService empresaService) : Controller
{
    public async Task<IActionResult> Login(CancellationToken cancellationToken)
    {
        await CargarEmpresasLoginAsync(cancellationToken);
        return View(new LoginDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken)
    {
        if (dto.EmpresaId <= 0)
        {
            await CargarEmpresasLoginAsync(cancellationToken);
            ModelState.AddModelError(string.Empty, "Seleccione una empresa.");
            return View(dto);
        }

        var result = await signInManager.PasswordSignInAsync(dto.Usuario, dto.Password, dto.Recordarme, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            await CargarEmpresasLoginAsync(cancellationToken);
            ModelState.AddModelError(string.Empty, "Usuario o contrasena incorrectos.");
            return View(dto);
        }

        var user = await userManager.FindByNameAsync(dto.Usuario);
        if (user is null || !user.Activo)
        {
            await signInManager.SignOutAsync();
            await CargarEmpresasLoginAsync(cancellationToken);
            ModelState.AddModelError(string.Empty, "Usuario inactivo.");
            return View(dto);
        }

        var empresas = await usuarioService.ObtenerEmpresasAsync(user.Id, cancellationToken);
        var empresa = empresas.FirstOrDefault(x => x.EmpresaId == dto.EmpresaId);
        if (empresa is null)
        {
            await signInManager.SignOutAsync();
            await CargarEmpresasLoginAsync(cancellationToken);
            ModelState.AddModelError(string.Empty, "No tiene acceso a la empresa seleccionada.");
            return View(dto);
        }

        EstablecerEmpresaActiva(empresa);
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> SeleccionarEmpresa(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(nameof(Login));

        var empresas = await usuarioService.ObtenerEmpresasAsync(user.Id, cancellationToken);
        return View(new SeleccionarEmpresaDto { Empresas = empresas });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeleccionarEmpresa(SeleccionarEmpresaDto dto, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction(nameof(Login));

        var empresas = await usuarioService.ObtenerEmpresasAsync(user.Id, cancellationToken);
        if (!empresas.Any(x => x.EmpresaId == dto.EmpresaId))
        {
            ModelState.AddModelError(string.Empty, "No tiene acceso a la empresa seleccionada.");
            dto.Empresas = empresas;
            return View(dto);
        }

        var empresa = empresas.First(x => x.EmpresaId == dto.EmpresaId);
        EstablecerEmpresaActiva(empresa);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Remove("EmpresaId");
        HttpContext.Session.Remove("EmpresaNombre");
        await signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    private void EstablecerEmpresaActiva(UsuarioEmpresaDto empresa)
    {
        var nombre = string.IsNullOrWhiteSpace(empresa.NombreComercial)
            ? empresa.RazonSocial
            : empresa.NombreComercial;

        HttpContext.Session.SetInt32("EmpresaId", empresa.EmpresaId);
        HttpContext.Session.SetString("EmpresaNombre", nombre);
    }

    private async Task CargarEmpresasLoginAsync(CancellationToken cancellationToken)
    {
        ViewBag.Empresas = (await empresaService.BuscarAsync(new ViveroLosFrutales.Application.Common.SearchRequest { PageSize = 500 }, cancellationToken))
            .Items
            .Where(x => x.Estado == ViveroLosFrutales.Domain.Enums.EstadoRegistro.Activo)
            .ToList();
    }
}
