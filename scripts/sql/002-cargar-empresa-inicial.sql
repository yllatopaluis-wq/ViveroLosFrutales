IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');
GO

/*
    Empresas iniciales de Vivero Los Frutales.

    Ejecutar despues de 001-create-database.sql y antes de las cargas 003 y 004.
    El script es idempotente: identifica cada empresa por su RUC, actualiza sus datos
    principales si ya existe y la inserta si no existe.
*/

USE ViveroLosFrutalesDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID(N'erp.Empresa', N'U') IS NULL
BEGIN
    THROW 51000, 'No existe la tabla Empresa. Ejecute primero 001-create-database.sql.', 1;
END;
GO

DECLARE @Empresas TABLE (
    RUC nvarchar(11) NOT NULL PRIMARY KEY,
    RazonSocial nvarchar(200) NOT NULL,
    NombreComercial nvarchar(150) NOT NULL,
    Direccion nvarchar(250) NOT NULL,
    Telefono nvarchar(50) NOT NULL,
    Email nvarchar(150) NOT NULL,
    SerieBoleta nvarchar(20) NOT NULL,
    SerieFactura nvarchar(20) NOT NULL,
    SerieNotaCredito nvarchar(20) NOT NULL,
    SerieNotaCreditoFactura nvarchar(20) NOT NULL,
    SerieNotaCreditoBoleta nvarchar(20) NOT NULL,
    SerieNotaPedido nvarchar(20) NOT NULL,
    SerieCotizacion nvarchar(20) NOT NULL,
    RepresentanteLegalNombre nvarchar(200) NOT NULL,
    RepresentanteLegalDocumento nvarchar(20) NOT NULL,
    RepresentanteLegalCargo nvarchar(120) NOT NULL,
    FirmaContentType nvarchar(120) NOT NULL,
    FirmaNombre nvarchar(260) NOT NULL
);

INSERT INTO @Empresas (
    RUC,
    RazonSocial,
    NombreComercial,
    Direccion,
    Telefono,
    Email,
    SerieBoleta,
    SerieFactura,
    SerieNotaCredito,
    SerieNotaCreditoFactura,
    SerieNotaCreditoBoleta,
    SerieNotaPedido,
    SerieCotizacion,
    RepresentanteLegalNombre,
    RepresentanteLegalDocumento,
    RepresentanteLegalCargo,
    FirmaContentType,
    FirmaNombre
)
VALUES
    (
        N'20615619273',
        N'VIVERO LOS FRUTALES HUARAL SAC',
        N'VIVERO LOS FRUTALES HUARAL',
        N'CAL. CALLE MONTE CARMELO LT 17 HUARAL',
        N'990852727',
        N'viverolosfrutalesh@gmail.com',
        N'B001',
        N'F001',
        N'NC001',
        N'F101',
        N'B101',
        N'NP001',
        N'C001',
        N'',
        N'',
        N'',
        N'',
        N''
    ),
    (
        N'20615082997',
        N'VIVERO LOS FRUTALES LIMA SAC',
        N'VIVERO LOS FRUTALES LIMA',
        N'Av 10 de junio 1020 San Martin de Porres',
        N'',
        N'viverolosfrutalesh@gmail.com',
        N'B002',
        N'F002',
        N'NC002',
        N'F202',
        N'B202',
        N'NP002',
        N'C002',
        N'',
        N'',
        N'',
        N'',
        N''
    );

MERGE erp.Empresa WITH (HOLDLOCK) AS target
USING @Empresas AS source
ON target.RUC = source.RUC
WHEN MATCHED THEN
    UPDATE SET
        RazonSocial = source.RazonSocial,
        NombreComercial = source.NombreComercial,
        Direccion = source.Direccion,
        Telefono = source.Telefono,
        Email = source.Email,
        MonedaPredeterminada = N'PEN',
        SerieBoleta = source.SerieBoleta,
        SerieFactura = source.SerieFactura,
        SerieNotaCredito = source.SerieNotaCredito,
        SerieNotaCreditoFactura = source.SerieNotaCreditoFactura,
        SerieNotaCreditoBoleta = source.SerieNotaCreditoBoleta,
        SerieNotaPedido = source.SerieNotaPedido,
        SerieCotizacion = source.SerieCotizacion,
        RepresentanteLegalNombre = COALESCE(NULLIF(target.RepresentanteLegalNombre, N''), source.RepresentanteLegalNombre),
        RepresentanteLegalDocumento = COALESCE(NULLIF(target.RepresentanteLegalDocumento, N''), source.RepresentanteLegalDocumento),
        RepresentanteLegalCargo = COALESCE(NULLIF(target.RepresentanteLegalCargo, N''), source.RepresentanteLegalCargo),
        FirmaContentType = COALESCE(target.FirmaContentType, source.FirmaContentType),
        FirmaNombre = COALESCE(target.FirmaNombre, source.FirmaNombre),
        Estado = 1
WHEN NOT MATCHED BY TARGET THEN
    INSERT (
        RUC,
        RazonSocial,
        NombreComercial,
        Direccion,
        Telefono,
        Email,
        MonedaPredeterminada,
        UrlNubefact,
        TokenNubefact,
        LogoPath,
        LogoContenido,
        LogoContentType,
        LogoNombre,
        RepresentanteLegalNombre,
        RepresentanteLegalDocumento,
        RepresentanteLegalCargo,
        FirmaContenido,
        FirmaContentType,
        FirmaNombre,
        SerieBoleta,
        SerieFactura,
        SerieNotaCredito,
        SerieNotaCreditoFactura,
        SerieNotaCreditoBoleta,
        SerieNotaPedido,
        SerieCotizacion,
        Estado,
        FechaRegistro,
        UsuarioRegistro
    )
    VALUES (
        source.RUC,
        source.RazonSocial,
        source.NombreComercial,
        source.Direccion,
        source.Telefono,
        source.Email,
        N'PEN',
        N'',
        N'',
        N'',
        NULL,
        N'',
        N'',
        source.RepresentanteLegalNombre,
        source.RepresentanteLegalDocumento,
        source.RepresentanteLegalCargo,
        NULL,
        source.FirmaContentType,
        source.FirmaNombre,
        source.SerieBoleta,
        source.SerieFactura,
        source.SerieNotaCredito,
        source.SerieNotaCreditoFactura,
        source.SerieNotaCreditoBoleta,
        source.SerieNotaPedido,
        source.SerieCotizacion,
        1,
        SYSUTCDATETIME(),
        N'script-empresa-inicial'
    );

PRINT N'Empresas iniciales verificadas/registradas:';
SELECT RUC, RazonSocial, NombreComercial
FROM erp.Empresa
WHERE RUC IN (N'20615619273', N'20615082997')
ORDER BY RUC;
GO