USE ViveroLosFrutalesDB;
GO

/*
    Usuario administrador inicial para salida a produccion.

    Credenciales iniciales:
        Usuario: admin
        Password: Admin1234

    Importante:
    - Cambiar la contrasena desde el sistema despues del primer ingreso.
    - El usuario queda asociado a las empresas indicadas en @RucsEmpresa para que pueda iniciar sesion.
    - El rol de permisos de negocio es erp.Rol RolId = 1 (Administrador).
    - Tambien se registra el rol Identity en erp.AspNetRoles y la relacion en erp.AspNetUserRoles.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');
GO

IF OBJECT_ID(N'erp.AspNetUsers', N'U') IS NULL
    THROW 51000, 'No existe erp.AspNetUsers. Ejecute primero 001-create-database.sql.', 1;
IF OBJECT_ID(N'erp.AspNetRoles', N'U') IS NULL
    THROW 51000, 'No existe erp.AspNetRoles. Ejecute primero 001-create-database.sql.', 1;
IF OBJECT_ID(N'erp.AspNetUserRoles', N'U') IS NULL
    THROW 51000, 'No existe erp.AspNetUserRoles. Ejecute primero 001-create-database.sql.', 1;
IF OBJECT_ID(N'erp.UsuarioEmpresa', N'U') IS NULL
    THROW 51000, 'No existe erp.UsuarioEmpresa. Ejecute primero 001-create-database.sql.', 1;
IF OBJECT_ID(N'erp.Empresa', N'U') IS NULL
    THROW 51000, 'No existe erp.Empresa. Ejecute primero 001-create-database.sql y 002-cargar-empresa-inicial.sql.', 1;
IF OBJECT_ID(N'erp.Rol', N'U') IS NULL
    THROW 51000, 'No existe erp.Rol. Ejecute primero 001-create-database.sql.', 1;
GO

DECLARE @UsuarioId nvarchar(450) = N'00000000-0000-0000-0000-000000000001';
DECLARE @Usuario nvarchar(256) = N'admin';
DECLARE @UsuarioNormalizado nvarchar(256) = N'ADMIN';
DECLARE @Email nvarchar(256) = N'admin@viverolosfrutales.local';
DECLARE @EmailNormalizado nvarchar(256) = N'ADMIN@VIVEROLOSFRUTALES.LOCAL';
DECLARE @Nombres nvarchar(max) = N'Administrador';
DECLARE @Apellidos nvarchar(max) = N'Sistema';
DECLARE @RolNegocioId int = 1;
DECLARE @RolIdentityId nvarchar(450) = N'00000000-0000-0000-0000-000000000101';
DECLARE @RolIdentityNombre nvarchar(256) = N'Administrador';
DECLARE @RolIdentityNormalizado nvarchar(256) = N'ADMINISTRADOR';
DECLARE @PasswordHash nvarchar(max) = N'AQAAAAEAACcQAAAAEO01KW8P0dJYuE9FonNoatDmJ5HL9QGK7F8EtFQxNMHnKydGJy6yN+lL1/fZiuBVgA==';

DECLARE @RucsEmpresa TABLE (RUC nvarchar(11) NOT NULL PRIMARY KEY);
INSERT INTO @RucsEmpresa (RUC)
VALUES (N'20615082997'), (N'20615619273');

BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM erp.Rol WHERE RolId = @RolNegocioId)
BEGIN
    SET IDENTITY_INSERT erp.Rol ON;
    INSERT INTO erp.Rol (RolId, Nombre, Descripcion, Estado)
    VALUES (@RolNegocioId, N'Administrador', N'Acceso total', 1);
    SET IDENTITY_INSERT erp.Rol OFF;
END;

-- Asegurar permisos completos para el rol administrador interno.
INSERT INTO erp.RolPermiso (RolId, PermisoId)
SELECT @RolNegocioId, p.PermisoId
FROM erp.Permiso p
WHERE NOT EXISTS (
    SELECT 1
    FROM erp.RolPermiso rp
    WHERE rp.RolId = @RolNegocioId
      AND rp.PermisoId = p.PermisoId
);

IF NOT EXISTS (SELECT 1 FROM erp.AspNetRoles WHERE NormalizedName = @RolIdentityNormalizado)
BEGIN
    INSERT INTO erp.AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@RolIdentityId, @RolIdentityNombre, @RolIdentityNormalizado, CONVERT(nvarchar(36), NEWID()));
END
ELSE
BEGIN
    SELECT TOP (1) @RolIdentityId = Id
    FROM erp.AspNetRoles
    WHERE NormalizedName = @RolIdentityNormalizado
    ORDER BY Id;

    UPDATE erp.AspNetRoles
    SET Name = @RolIdentityNombre,
        NormalizedName = @RolIdentityNormalizado
    WHERE Id = @RolIdentityId;
END;

IF EXISTS (
    SELECT 1
    FROM erp.AspNetUsers
    WHERE NormalizedUserName = @UsuarioNormalizado
      AND Id <> @UsuarioId
)
BEGIN
    SELECT TOP (1) @UsuarioId = Id
    FROM erp.AspNetUsers
    WHERE NormalizedUserName = @UsuarioNormalizado
    ORDER BY Id;
END;

IF NOT EXISTS (SELECT 1 FROM erp.AspNetUsers WHERE Id = @UsuarioId)
BEGIN
    INSERT INTO erp.AspNetUsers (
        Id,
        Nombres,
        Apellidos,
        RolId,
        Activo,
        UserName,
        NormalizedUserName,
        Email,
        NormalizedEmail,
        EmailConfirmed,
        PasswordHash,
        SecurityStamp,
        ConcurrencyStamp,
        PhoneNumber,
        PhoneNumberConfirmed,
        TwoFactorEnabled,
        LockoutEnd,
        LockoutEnabled,
        AccessFailedCount
    )
    VALUES (
        @UsuarioId,
        @Nombres,
        @Apellidos,
        @RolNegocioId,
        1,
        @Usuario,
        @UsuarioNormalizado,
        @Email,
        @EmailNormalizado,
        1,
        @PasswordHash,
        CONVERT(nvarchar(36), NEWID()),
        CONVERT(nvarchar(36), NEWID()),
        NULL,
        0,
        0,
        NULL,
        1,
        0
    );
END
ELSE
BEGIN
    UPDATE erp.AspNetUsers
    SET Nombres = @Nombres,
        Apellidos = @Apellidos,
        RolId = @RolNegocioId,
        Activo = 1,
        UserName = @Usuario,
        NormalizedUserName = @UsuarioNormalizado,
        Email = @Email,
        NormalizedEmail = @EmailNormalizado,
        EmailConfirmed = 1,
        PasswordHash = @PasswordHash,
        SecurityStamp = CONVERT(nvarchar(36), NEWID()),
        ConcurrencyStamp = CONVERT(nvarchar(36), NEWID()),
        LockoutEnabled = 1,
        AccessFailedCount = 0
    WHERE Id = @UsuarioId;
END;

IF NOT EXISTS (
    SELECT 1
    FROM erp.AspNetUserRoles
    WHERE UserId = @UsuarioId
      AND RoleId = @RolIdentityId
)
BEGIN
    INSERT INTO erp.AspNetUserRoles (UserId, RoleId)
    VALUES (@UsuarioId, @RolIdentityId);
END;

IF NOT EXISTS (
    SELECT 1
    FROM erp.Empresa e
    INNER JOIN @RucsEmpresa r ON r.RUC = e.RUC
    WHERE e.Estado = 1
)
BEGIN
    THROW 51001, 'No existen empresas activas para asociar el usuario administrador. Ejecute 002-cargar-empresa-inicial.sql o revise @RucsEmpresa.', 1;
END;

INSERT INTO erp.UsuarioEmpresa (UsuarioId, EmpresaId)
SELECT @UsuarioId, e.EmpresaId
FROM erp.Empresa e
INNER JOIN @RucsEmpresa r ON r.RUC = e.RUC
WHERE e.Estado = 1
  AND NOT EXISTS (
      SELECT 1
      FROM erp.UsuarioEmpresa ue
      WHERE ue.UsuarioId = @UsuarioId
        AND ue.EmpresaId = e.EmpresaId
  );

COMMIT TRANSACTION;

PRINT N'Usuario administrador verificado/registrado.';
SELECT u.UserName, u.Email, u.Activo, u.RolId
FROM erp.AspNetUsers u
WHERE u.Id = @UsuarioId;

SELECT e.EmpresaId, e.RUC, e.RazonSocial, e.NombreComercial
FROM erp.UsuarioEmpresa ue
INNER JOIN erp.Empresa e ON e.EmpresaId = ue.EmpresaId
WHERE ue.UsuarioId = @UsuarioId
ORDER BY e.RUC;
GO

