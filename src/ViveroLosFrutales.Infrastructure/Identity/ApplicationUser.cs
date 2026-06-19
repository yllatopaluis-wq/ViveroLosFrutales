using Microsoft.AspNetCore.Identity;

namespace ViveroLosFrutales.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public int RolId { get; set; }
    public bool Activo { get; set; } = true;
}
