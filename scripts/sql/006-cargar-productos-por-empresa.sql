IF SCHEMA_ID(N'erp') IS NULL
    EXEC(N'CREATE SCHEMA erp');
GO

/*
    Carga de productos por empresa desde catalogo Nubefact: 20615082997-CATALOGO (2).xlsx

    Objetivo:
    - Cargar el mismo catalogo de productos para cada empresa activa registrada en erp.Empresa.
    - Mantener productos y categorias separados por EmpresaId.
    - Evitar duplicados por empresa usando EmpresaId + Nombre normalizado.
    - Si el producto ya existe para una empresa, actualiza categoria, unidad, precios, stock, IGV y detraccion.

    Uso:
    - Por defecto carga para las empresas Vivero Los Frutales Huaral y Lima.
    - Para cargar solo empresas especificas, agregue RUC en @RucsFiltro antes de la carga.

    RUC considerados:
        - 20615619273: Vivero Los Frutales Huaral
        - 20615082997: Vivero Los Frutales Lima
*/

USE ViveroLosFrutalesDB;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

DECLARE @RucsFiltro TABLE (RUC nvarchar(20) NOT NULL PRIMARY KEY);
-- Empresas destino para esta carga inicial.
INSERT INTO @RucsFiltro (RUC) VALUES (N'20615619273'), (N'20615082997');

DECLARE @Empresas TABLE (
    EmpresaId int NOT NULL PRIMARY KEY,
    RUC nvarchar(20) NOT NULL,
    RazonSocial nvarchar(250) NOT NULL
);

INSERT INTO @Empresas (EmpresaId, RUC, RazonSocial)
SELECT e.EmpresaId, e.RUC, e.RazonSocial
FROM erp.Empresa e
WHERE e.Estado = 1
  AND (
      NOT EXISTS (SELECT 1 FROM @RucsFiltro)
      OR EXISTS (SELECT 1 FROM @RucsFiltro f WHERE f.RUC = e.RUC)
  );

IF NOT EXISTS (SELECT 1 FROM @Empresas)
BEGIN
    THROW 51000, 'No existen empresas activas para cargar productos. Registre una empresa activa o revise @RucsFiltro.', 1;
END;

DECLARE @Catalogo TABLE (
    CodigoInterno nvarchar(50) NULL,
    Nombre nvarchar(200) NOT NULL,
    UnidadMedida nvarchar(20) NOT NULL,
    Categoria nvarchar(100) NOT NULL,
    VentaValorUnitarioSinIgv decimal(18,4) NULL,
    VentaPrecioUnitarioConIgv decimal(18,4) NULL,
    TipoAfectacionIgv int NOT NULL
);

INSERT INTO @Catalogo (
    CodigoInterno,
    Nombre,
    UnidadMedida,
    Categoria,
    VentaValorUnitarioSinIgv,
    VentaPrecioUnitarioConIgv,
    TipoAfectacionIgv
)
VALUES
    (N'99985', N'Ruda', N'NIU', N'Ornamentales', 2.542, 3, 10),
    (N'99986', N'Arandano Ventura', N'NIU', N'Frutales', 10.169, 12, 10),
    (N'172', N'PLANTA DE DAMASCO INJERTO', N'NIU', N'Frutales', 6, 6, 20),
    (N'99998', N'TURBA', N'SA', N'Materiales', 190, NULL, 20),
    (N'99991', N'UVINA INJERTA', N'NIU', N'Frutales', 6, NULL, 20),
    (N'99996', N'HIDROCLAY', N'KGM', N'Materiales', 9.3, NULL, 20),
    (N'99997', N'PERLITA', N'KGM', N'Materiales', 13.5, NULL, 20),
    (N'99995', N'UVA SWETE GLOBO ROSADO INJERTO', N'NIU', N'Frutales', 6, NULL, 20),
    (N'99989', N'CUFEA (LLUVIA MEXICAN)', N'NIU', N'Ornamentales', NULL, 4, 10),
    (N'99993', N'UVA MALVET INJERTO', N'NIU', N'Frutales', 6, NULL, 20),
    (N'99994', N'UVA TEMPRANITO INJERTO', N'NIU', N'Frutales', 6, NULL, 20),
    (N'99988', N'PINO ROMERON', N'NIU', N'Ornamentales', NULL, 18, 10),
    (N'99992', N'UVA CHARDONE INJERTO', N'NIU', N'Frutales', 6, NULL, 20),
    (N'99990', N'UVA NEGRA CRIOLLA INJERTA', N'NIU', N'Frutales', 6, NULL, 20),
    (N'323', N'PLANTA DE  PIMIENTA NEGRA', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'89', N'PLANTA DE  BUGAMBILIA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'48', N'PLANTA DE  CORAZON DE JESUS', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'98', N'PLANTA DE  GERANIO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'97', N'PLANTA DE  PLATANO DE SEDA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'82', N'HUMUS EN SACO 20KG', N'NIU', N'Abonos', 20, NULL, 20),
    (N'271', N'HUMUS X 1KG', N'NIU', N'Abonos', 20, NULL, 20),
    (N'99', N'PLANTA DE  MARACUYA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'95', N'PLANTA DE  FRESA SAN ANDREA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'94', N'PLANTA DE  PAPAYA HIBRIDA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'93', N'PLANTA DE  PAPAYA CRIOLLA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'92', N'PLANTA DE  PITAHAYA FUCSIA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'91', N'PLANTA DE  PITAHAYA AMARILLA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'90', N'PLANTA DE  PITAHAYA ROJO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'83', N'PLANTA DE  ARANDANO BILOXI', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'80', N'PLANTA DE  LITCHI', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'65', N'PLANTA DE  SAKURA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'63', N'PLANTA DE  GUAYABA AMARILLA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'53', N'PLANTA DE  ZARZAMORA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'50', N'PLANTA DE  PLATANO ISLA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'434', N'PLANTA DE  CALA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'433', N'PLANTA DE  YACON B.G', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'432', N'PLANTA DE  YACON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'431', N'SERVICIO DE EMBALAJE PTN', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'428', N'BANDEJA', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'427', N'PRODUCTO ACEITE MINERAL', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'426', N'PRODUCTOS INSEPTICIDAS', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'425', N'PRODUCTO FOLIARES', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'422', N'PLANTA DE  GUANABANA PATRON BLS 13X20', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'421', N'PLANTA DE  PALMERA BEQUIA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'420', N'CAUCHO', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'419', N'PLANTA DE  CAIMITO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'418', N'PLANTA DE  ORTENCIA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'417', N'PLANTA DE  SACO DE VIRUTA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'416', N'PLANTA DE  PEPEROMIA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'415', N'PLANTA DE  MANGO CIRUELO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'414', N'PLANTA DE  ANONA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'413', N'PLANTA DE  COCONA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'412', N'PLANTA DE  NONI', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'411', N'PLANTA DE  CACAO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'410', N'PLANTA DE  JAKARANDA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'409', N'PLANTA DE  MOLLE CRIOLLO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'408', N'PLANTA DE  SAPOTE', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'407', N'PLANTA DE  FIBRA DE COCO KG', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'405', N'PLANTA DE  LOGAN', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'403', N'CARRIZO', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'395', N'PLANTA DE NISPERO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'394', N'PLANTA DE  TRUENO DE VENUS - CUFEA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'393', N'PLANTA DE  ALFOMBRA ROSA EN MACETA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'392', N'PLANTA DE  ALFOMBRA ROSA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'391', N'PLANTA DE  VERDOLAGA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'390', N'PLANTA DE  CIRUELO SANTA ROSA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'389', N'PLANTA DE  DAMASCO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'388', N'PLANTA DE  CIRUELO REYNA CLAUDIA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'387', N'PLANTA DE  CUNA DE MOISES', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'386', N'PLANTA DE  ACALIFA ROJO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'385', N'PLANTA DE  ACALIFA VERDE', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'384', N'PLANTA DE  MIOPORO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'38', N'PLANTA DE  OKINAWA PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'377', N'PLANTA DE  PINO CHILENO GRANDE', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'376', N'PLANTA DE  PINO CHILENO MEDIANO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'374', N'PLANTA DE  NARDO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'373', N'CUCHILLA CUTER', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'372', N'PLANTA DE  MORA PATRON (BG)', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'371', N'PLANTA DE  GUAYABA PATRON (BG)', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'370', N'PLANTA DE  OKINAWA PATRON (BG)', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'368', N'PLANTA DE  ARANDANO BILOXI (BG)', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'365', N'PLANTA DE  MANGOSTINO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'361', N'PLANTA DE  CHAVELITA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'358', N'PLANTA DE  URCUMANO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'356', N'PLANTA DE  GUINDA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'355', N'PLANTA DE  POMAROSA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'353', N'PLANTA DE  COCO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'351', N'PLANTA DE  SAUCO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'334', N'PLANTA DE  VERBENA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'333', N'PLANTA DE  NISPERO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'329', N'PLANTA DE  CARDENAL', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'328', N'PLANTA DE  PALMERA DACTILERA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'327', N'PLANTA DE  PALMERA ABANICO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'326', N'PLANTA DE  LENGUA DE SUEGRA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'325', N'PLANTA DE  FRESA SABRINA X BANDEJA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'324', N'PLANTA DE  FRESA SAN ANDREA X BANDEJA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'322', N'PLANTA DE  FLOR DE JAMAICA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'320', N'PLANTA DE  LAUREL CHINO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'319', N'PLANTA DE  CLAVELES', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'318', N'PLANTA DE  COQUETA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'317', N'PLANTA DE  COPA DE ORO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'316', N'PLANTA DE  PINO AZUL', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'315', N'PLANTA DE  DUMB CANE', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'309', N'PLANTA DE  PEPINO DULCE', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'308', N'PLANTA DE  GARDENIA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'307', N'PLANTA DE  LIRIO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'306', N'PLANTA DE  ARANDANO EMERAL', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'302', N'PLANTA DE  PALMERA SIKA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'301', N'PLANTA DE  PINO CHILENO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'300', N'PLANTA DE  PINO VELA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'299', N'PLANTA DE  HELECHOS ALEMAN', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'298', N'PLANTA DE  CORONA DE CRISTO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'297', N'PLANTA DE  MOLLE', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'296', N'PLANTA DE  CORONA DE CRISTO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'295', N'PLANTA DE  MARGARITA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'293', N'PLANTA DE  MANDARINA CITRUMELO PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'292', N'PLANTA DE  LIMA RAMPUR', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'290', N'PLANTA DE  CIRUELA DE PALO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'289', N'PLANTA DE  HIGO BLANCO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'288', N'PLANTA DE  GUAYABA VERDE', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'282', N'PLANTA DE  TOMATE DE ARBOL', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'28', N'CINTA EMBALAJE NEGRA', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'277', N'PLANTA DE  HELECRO CATARATA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'276', N'PLANTA DE  PLATANO MORADO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'275', N'TIERRA PARA MACETAS', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'272', N'PLANTA DE  CEDRON', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'270', N'PLANTA DE  PAPAYA HIBRIDA (BG)', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'27', N'PLANTA DE  NUEVA GUINEA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'265', N'JABA DE PLASTICO', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'264', N'PLANTA DE  GALAN DE NOCHE', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'263', N'PLANTA DE  LLAMA DOLAR', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'261', N'PLANTA DE  CUCARDA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'251', N'PLANTA DE  CONSTILLA DE ADAN', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'247', N'PLANTA DE  MANGO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'246', N'PLANTA DE  MANGO CRIOLLO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'245', N'PLANTA DE  LIMON RUGOSO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'244', N'HERBICIDA AGRICOLA', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'243', N'PAQUETE DE BOLSA 8X16', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'242', N'PLANTA DE  CASUARINAS', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'239', N'PARAFILM X METRO', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'238', N'PARAFILM X CAJA', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'237', N'CUCHILLA TRAMONTINA', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'236', N'PLANTA DE  TRICOLOR', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'235', N'PLANTA DE  MEIJO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'234', N'PLANTA DE  OLIVO (BG)', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'232', N'PLANTA DE  GUAYABA ROSA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'231', N'PLANTA DE  MAMEY', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'230', N'CORDEL AMARRA RASHEL', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'23', N'PLANTA DE  MACADAMIA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'229', N'PLANTA DE  AGUAYMANTO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'228', N'PLANTA DE  PALMERA CATARATA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'227', N'PLANTA DE  PALMERA BOTELLA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'226', N'PLANTA DE  CIPRES LIMON', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'225', N'PLANTA DE  LAUREL AROMATICO', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'223', N'PLANTA DE  YACA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'222', N'PLANTA DE  NOGAL', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'218', N'PLANTA DE  TUMBO PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'217', N'PAQUETE DE BOLSA 13X20 (50 UND)', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'216', N'PLANTA DE  BUGAMBILIA (ARBOL)', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'215', N'PLANTA DE  CEREZA ESPAÑOLA (CRIOLLA)', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'214', N'PLANTA DE  JADE', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'213', N'PLANTA DE  MONEDA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'212', N'PLANTA DE  NUEZ', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'211', N'PLANTA DE  PLATANO BISCOCHITO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'209', N'PLANTA DE  VIOLETA DE LOS ALPES', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'206', N'PLANTA DE  PINO RADIATA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'205', N'PLANTA DE  EUCALIPTO CRIOLLO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'204', N'PLANTA DE  EUCALIPTO ALCANFORADO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'201', N'PLANTA DE  TUJA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'199', N'TIERRA PREPARADA X KILO', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'198', N'PLANTA DE  MANDARINA CLEOPATRA PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'197', N'PLANTA DE  PALTA PATRON ETINGER', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'194', N'PLANTA DE  HIGO NEGRO (BG)', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'190', N'PLANTA DE  GRANADA WONDERFULL (BG)', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'182', N'PLANTA DE  LANTANA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'181', N'PLANTA DE  PONCIANA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'178', N'PLANTA DE  VIRUTA DEL PINO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'177', N'PLANTA DE  LAUREL', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'176', N'PLANTA DE  ANTURIO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'175', N'PLANTA DE  CAFE', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'174', N'PLANTA DE  CIRUELO FRAILE (CANSA BOCA)', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'165', N'PLANTA DE  HELECHO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'164', N'PLANTA DE  PINO CIPRES', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'163', N'BOLSA DE CHUP X PAQUETE', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'162', N'PLANTA DE  GRANADA PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'161', N'PLANTA DE  UVA PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'160', N'PLANTA DE  LUCUMA PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'159', N'PLANTA DE  GUANABANA PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'158', N'PLANTA DE  CHIRIMOYA PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'157', N'BOMBACHITA X METRO', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'156', N'MALLA RACHEL X METRO', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'155', N'PLANTA DE  CHIMBILILLO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'154', N'PLANTA DE  PACAE', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'153', N'PLANTA DE  DURANTE AMARILLA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'152', N'PLANTA DE  DURANTE ROJA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'151', N'PLANTA DE  ACHIOTE', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'150', N'PLANTA DE  AJI LIMO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'149', N'PLANTA DE  GALONCHEA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'148', N'PLANTA DE  ROSA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'146', N'PLANTA DE  CAQUI PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'145', N'PAJILLA DE ARROZ', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'144', N'PLANTA DE  PALTA ANTILLANA PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'143', N'PLANTA DE  PALTA ZUTANO PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'142', N'PLANTA DE  PALTA TOPA TOPA PATRON', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'141', N'PLANTA DE  JUJUBE', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'140', N'PLANTA DE  STEVIA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'139', N'PLANTA DE  CHIFLERA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'135', N'PLANTA DE  HIGO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'134', N'PLANTA DE  HIGO TORO SENTADO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'132', N'PLANTA DE  KIWI', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'131', N'PLANTA DE  CARAMBOLA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'130', N'PLANTA DE  TAMARINDO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'13', N'PLANTA DE  SANDIA SIN PASAS', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'128', N'PLANTA DE  CEREZA MORADA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'127', N'PLANTA DE  CEREZA ROJA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'123', N'PLANTA DE  MEMBRILLO LUCUMO', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'122', N'PLANTA DE  MORA MEJORADA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'121', N'PLANTA DE  MORA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'120', N'PLANTA DE  FRAMBUESA', N'NIU', N'Frutales', NULL, NULL, 10),
    (N'119', N'PLANTA DE  CANELA', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'118', N'PLANTA DE  CLAVO DE OLOR', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'117', N'PLANTA DE  MUÑA', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'116', N'PLANTA DE  LAVANDA', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'115', N'PLANTA DE  HIERVABUENA', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'114', N'PLANTA DE  ROMERO', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'113', N'PLANTA DE  MENTA', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'112', N'PLANTA DE  OREGANO', N'NIU', N'Aromaticas', NULL, NULL, 10),
    (N'111', N'PLANTA DE  FICO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'105', N'PLANTA DE  CROTO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'104', N'PLANTA DE  PALMERA HAWAIANA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'103', N'PLANTA DE  SUCULENTA', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'102', N'PLANTA DE  HUARANGUILLO', N'NIU', N'Ornamentales', NULL, NULL, 10),
    (N'100', N'PAQUETE DE BOLSA 7X14', N'NIU', N'Materiales', NULL, NULL, 10),
    (N'96', N'PLANTA DE  UVA SUPERIO INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'9', N'PLANTA DE  MANDARINA SATSUMA OKITSU INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'88', N'PLANTA DE  TANGELO INJERTO', N'NIU', N'Frutales', 6, 6, 20),
    (N'87', N'PLANTA DE  UVA QUEBRANTA INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'86', N'PLANTA DE  UVA RED GLOBE INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'85', N'PLANTA DE  UVA ITALIA INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'84', N'PLANTA DE  UVA SWET GLOBE INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'81', N'PLANTA DE  CAQUI AMARILLO INJERTA', N'NIU', N'Frutales', 35, 35, 20),
    (N'8', N'PLANTA DE  PALTA FUERTE INJERTA EN ZUTANO', N'NIU', N'Frutales', 12, 12, 20),
    (N'79', N'PLANTA DE  GRANADILLA COLOMBIANA INJERTA', N'NIU', N'Frutales', 5, 5, 20),
    (N'78', N'PLANTA DE  NISPERO JAPONES INJERTA (BG)', N'NIU', N'Frutales', 23, 23, 20),
    (N'77', N'PLANTA DE  LIMA DULCE INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'76', N'PLANTA DE  PECANA MAHA INJERTA', N'NIU', N'Frutales', 70, 70, 20),
    (N'75', N'PLANTA DE  MANDARINA TANGO INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'74', N'PLANTA DE  MANZANA DELICIA INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'73', N'PLANTA DE  TORONGA AMARILLA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'72', N'PLANTA DE  MANGO KENT INJERTA (BG)', N'NIU', N'Frutales', 35, 35, 20),
    (N'71', N'PLANTA DE  MANGO EDWART INJERTA (BG)', N'NIU', N'Frutales', 35, 35, 20),
    (N'70', N'PLANTA DE  LUCUMA VERAMIX INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'7', N'PLANTA DE  CHIRIMOYA CUMBE INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'69', N'PLANTA DE  LUCUMA SEDA INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'68', N'PLANTA DE  LUCUMA ROQUE INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'67', N'PLANTA DE  MANDARINA W.MURCOT INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'66', N'PLANTA DE  PALTA FUERTE INJERTA EN TOPA TOPA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'64', N'PLANTA DE  NARANJA POWER INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'62', N'PLANTA DE  KIN KAN INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'61', N'PLANTA DE  GUANABANA WORD INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'60', N'PLANTA DE  MANGO CAFRO INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'6', N'PLANTA DE  MANZANA DELICIA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'59', N'PLANTA DE  LUCUMA SILIX INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'58', N'PLANTA DE  MANDARINA CLEMENTINA EN CLEOPATRA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'57', N'PLANTA DE  MANGO EDWART INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'56', N'PLANTA DE  PALTA NAVAL AZUL INJERTA EN ZUTANO', N'NIU', N'Frutales', 12, 12, 20),
    (N'55', N'PLANTA DE  PERA DE AGUA INJERTA', N'NIU', N'Frutales', 5, 5, 20),
    (N'54', N'PLANTA DE  GUINDON INJERTA', N'NIU', N'Frutales', 5, 5, 20),
    (N'52', N'PLANTA DE  UVA BORGOÑA NEGRA INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'51', N'PLANTA DE  PALTA HASS INJERTA EN ANTILLANA', N'NIU', N'Frutales', 35, 35, 20),
    (N'5', N'PLANTA DE  LUCUMA SEDA INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'47', N'PLANTA DE  PALTA FUERTE INJERTA EN ZUTANO (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'46', N'PLANTA DE  PALTA HASS INJERTA EN ZUTANO (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'45', N'PLANTA DE  NARANJA HUANDO INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'44', N'PLANTA DE  LUCUMA ROQUE INJERTA (BG)', N'NIU', N'Frutales', 24, 24, 20),
    (N'43', N'PLANTA DE  LUCUMA VERAMIX INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'429', N'PLANTA DE  MANZANA CHILENA INJERTA', N'NIU', N'Frutales', 15, 15, 20),
    (N'424', N'PLANTA DE  GUANABANA CRIOLLA INJERTA BLS GRANDE', N'NIU', N'Frutales', 20, 20, 20),
    (N'423', N'PLANTA DE  GUANABANA CRIOLLA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'42', N'PLANTA DE  CHIRIMOYA CUMBE INJERTA (B. G)', N'NIU', N'Frutales', 25, 25, 20),
    (N'41', N'PLANTA DE  GUANABANA WORD INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'404', N'PLANTA DE  MACADAMIA INJERTA', N'NIU', N'Frutales', 60, 60, 20),
    (N'402', N'PLANTA DE  MANGO KEITT INJERTO', N'NIU', N'Frutales', 8, 8, 20),
    (N'401', N'PLANTA DE  MANGO HAIDEN INJERTA (B.G)', N'NIU', N'Frutales', 25, 25, 20),
    (N'400', N'PLANTA DE  MANZANA GOLDEN INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'40', N'PLANTA DE  GUINDON INJERTA (BG)', N'NIU', N'Frutales', 24, 24, 20),
    (N'4', N'PLANTA DE  PALTA HASS INJERTA EN ZUTANO', N'NIU', N'Frutales', 12, 12, 20),
    (N'399', N'PLANTA DE  PERA DE AGUA INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'398', N'PLANTA DE  MANZANA WINTER INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'397', N'PLANTA DE  PALTA CARMEN HASS INJERTA EN ZUTANO', N'NIU', N'Frutales', 13, 13, 20),
    (N'396', N'PLANTA DE  LIMON REAL', N'NIU', N'Frutales', 6, 6, 20),
    (N'39', N'PLANTA DE  GRANADA WONDELFUL INJERTA', N'NIU', N'Frutales', 4, 4, 20),
    (N'383', N'PLANTA DE  UVA RED GLOBE INJERTA (B.G)', N'NIU', N'Frutales', 20, 20, 20),
    (N'382', N'PLANTA DE  UVA QUEBRANTA INJERTA (B.G)', N'NIU', N'Frutales', 20, 20, 20),
    (N'381', N'PLANTA DE  UVA ITALIA INJERTA (B.G)', N'NIU', N'Frutales', 20, 20, 20),
    (N'380', N'PLANTA DE  	UVA BORGOÑA NEGRA INJERTA (BG)', N'NIU', N'Frutales', 20, 20, 20),
    (N'378', N'PLANTA DE  LUCUMA BELTRAN 2', N'NIU', N'Frutales', 6, 6, 20),
    (N'375', N'PLANTA DE  DURAZNO ACONCAGUA', N'NIU', N'Frutales', 7, 7, 20),
    (N'37', N'PLANTA DE  TUMBO INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'369', N'PLANTA DE  NECTARINA INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'366', N'PLANTA DE  MANGO PAPAYA', N'NIU', N'Frutales', 8, 8, 20),
    (N'363', N'PLANTA DE  UVA AUTUMN CRISP', N'NIU', N'Frutales', 8, 8, 20),
    (N'362', N'PLANTA DE  LUCUMA PALO INJERTA', N'NIU', N'Frutales', 8.5, 8.5, 20),
    (N'36', N'PLANTA DE  PALTA NAVAL AZUL INJERTA EN TOPA TOPA', N'NIU', N'Frutales', 10, 10, 20),
    (N'357', N'PLANTA DE  MAMEY  INJERTO', N'NIU', N'Frutales', 23, 23, 20),
    (N'352', N'PLANTA DE  PALTA NAVA AZUL INJERTO EN ANTILLANO', N'NIU', N'Frutales', 20, 20, 20),
    (N'350', N'PLANTA DE  LUCUMA MARIA BELEN INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'35', N'PLANTA DE  MANZANA ISRAEL INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'349', N'PLANTA DE  UVA PALESTINA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'348', N'PLANTA DE  NARANJA HUANDO INJERTA BLS 10X15', N'NIU', N'Frutales', 12, 12, 20),
    (N'347', N'PLANTA DE  LIMON SUTIL INJERTA BLS 13X15', N'NIU', N'Frutales', 20, 20, 20),
    (N'346', N'PLANTA DE  MANDARINA ORRI INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'345', N'PLANTA DE  NARANJA NAVEL CARA CARA INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'344', N'PLANTA DE  LUCUMA SEDA INJERTO (BLS 13X15)', N'NIU', N'Frutales', 18, 18, 20),
    (N'343', N'PLANTA DE  LUCUMA BELTRAN 4 INJERTA  (BLS 13X15)', N'NIU', N'Frutales', 18, 18, 20),
    (N'342', N'PLANTA DE  DURAZNO ABRIDOR NARANJA INJERTO', N'NIU', N'Frutales', 7, 7, 20),
    (N'341', N'PLANTA DE  DURAZNO ABRIDOR AMARILLO INJERTO', N'NIU', N'Frutales', 7, 7, 20),
    (N'340', N'PLANTA DE  DURAZNO HUAYCO CREMA INJERTO', N'NIU', N'Frutales', 7, 7, 20),
    (N'34', N'PLANTA DE  LUCUMA SILIX INJERTA (BG)', N'NIU', N'Frutales', 24, 24, 20),
    (N'338', N'PLANTA DE  YACA INJERTA', N'NIU', N'Frutales', 70, 70, 20),
    (N'337', N'PLANTA DE  LITCHI INJERTA', N'NIU', N'Frutales', 200, 200, 20),
    (N'336', N'PLANTA DE  NARANJA VALENCIA INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'335', N'PLANTA DE  LIMON PERSA INJERTO', N'NIU', N'Frutales', 10, 10, 20),
    (N'332', N'PLANTA DE  LUCUMA VERAMIX INJERTAS 2', N'NIU', N'Frutales', 7, 7, 20),
    (N'331', N'PLANTA DE  MANZANA ISRAEL INJERTA - BLS 8X16', N'NIU', N'Frutales', 10, 10, 20),
    (N'330', N'PLANTA DE  MANZANA CANA (SANTA ROSA) INJERTA - BLS 8X16', N'NIU', N'Frutales', 10, 10, 20),
    (N'33', N'PLANTA DE  PALTA FUERTE INJERTA EN ATILLANA', N'NIU', N'Frutales', 35, 35, 20),
    (N'321', N'PLANTA DE  ALBARICOQUE INJERTO', N'NIU', N'Frutales', 7, 7, 20),
    (N'32', N'PLANTA DE  MANGO KENT INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'314', N'PLANTA DE  NARANJA POWER INJERTA B.G', N'NIU', N'Frutales', 25, 25, 20),
    (N'313', N'PLANTA DE  PERA ITALIANA INJERTA B.G', N'NIU', N'Frutales', 25, 25, 20),
    (N'312', N'PLANTA DE  PERA CHILENA INJERTA B.G.', N'NIU', N'Frutales', 25, 25, 20),
    (N'311', N'PLANTA DE  LIMON DULCE INJERTO B.G', N'NIU', N'Frutales', 25, 25, 20),
    (N'310', N'PLANTA DE  TORONJA POMELO INJERTO B.G.', N'NIU', N'Frutales', 25, 25, 20),
    (N'31', N'PLANTA DE  MANGO CAFRO INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'305', N'PLANTA DE  MANGO ATAULFO INJERTO', N'NIU', N'Frutales', 7, 7, 20),
    (N'304', N'PLANTA DE  MANGO HADEN INJERTO', N'NIU', N'Frutales', 7, 7, 20),
    (N'303', N'PLANTA DE  MANZANA PACHACAMAC INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'30', N'PLANTA DE  DURAZNO ORO AZTECA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'3', N'PLANTA DE  MANZANA CANA (SANTA ROSA) INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'294', N'PLANTA DE  MANZANA GOLDEN INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'291', N'PLANTA DE  MANDARINA INVICTA INJERTA', N'NIU', N'Frutales', 8, 8, 20),
    (N'29', N'PLANTA DE  HIGO TORO SENTADO INJERTA', N'NIU', N'Frutales', 15, 15, 20),
    (N'287', N'PLANTA DE  OLIVO CEVILLANO VERDE INJERTA', N'NIU', N'Frutales', 12, 12, 20),
    (N'286', N'PLANTA DE  GRANADA MOLLAR INJERTA', N'NIU', N'Frutales', 8, 8, 20),
    (N'285', N'PLANTA DE  MANZANA WINTER', N'NIU', N'Frutales', 7, 7, 20),
    (N'284', N'PLANTA DE  MANZANA FUJI INJERTO', N'NIU', N'Frutales', 7, 7, 20),
    (N'283', N'PLANTA DE  PERA MOTA INJERTA', N'NIU', N'Frutales', 10, 10, 20),
    (N'274', N'PLANTA DE  MANDARINA PRIMOSOLE INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'273', N'PLANTA DE  PALTA VILLACAMPA INJERTA EN ZUTANO', N'NIU', N'Frutales', 11, 11, 20),
    (N'269', N'PLANTA DE  UVA CENTENIA INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'268', N'PLANTA DE  UVA COTON CANDY INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'267', N'PLANTA DE  PALTA SUPER FUERTE INJERTA EN ZUTANO', N'NIU', N'Frutales', 12, 12, 20),
    (N'266', N'PLANTA DE  CIRUELO SANTA ROSA', N'NIU', N'Frutales', 6, 6, 20),
    (N'260', N'PLANTA DE  PECANA MAHA INJERTA 1.2 MT', N'NIU', N'Frutales', 40, 40, 20),
    (N'26', N'PLANTA DE  PALTA FUERTE INJERTA EN TOPA TOPA', N'NIU', N'Frutales', 10, 10, 20),
    (N'259', N'PLANTA DE  PECANA MAHA INJERTO 8OCM', N'NIU', N'Frutales', 25, 25, 20),
    (N'258', N'PLANTA DE  TANGELO INJERTO (B.G)', N'NIU', N'Frutales', 25, 25, 20),
    (N'257', N'PLANTA DE  OLIVO INJERTO (B.G)', N'NIU', N'Frutales', 30, 30, 20),
    (N'256', N'PLANTA DE  MANGO PLATANO INJERTA (B.G)', N'NIU', N'Frutales', 25, 25, 20),
    (N'255', N'PLANTA DE  MANGO PLATANO INJERTO', N'NIU', N'Frutales', 7, 7, 20),
    (N'254', N'PLANTA DE  LIMA DULCE INJERTA (B.G)', N'NIU', N'Frutales', 25, 25, 20),
    (N'253', N'PLANTA DE  CAQUI ROJO BRILLANTE INJERTO (B.G)', N'NIU', N'Frutales', 60, 60, 20),
    (N'252', N'PLANTA DE  MANDARINA MALVACEA INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'250', N'PLANTA DE  ALMENDRA INJERTA (BG)', N'NIU', N'Frutales', 30, 30, 20),
    (N'25', N'PLANTA DE  NISPERO JAPONES INJERTO', N'NIU', N'Frutales', 6, 6, 20),
    (N'249', N'PLANTA DE  BLANQUILLO INJERTO (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'24', N'PLANTA DE  LUCUMA BELTRAN 4 INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'221', N'PLANTA DE  LUCUMA BELTRAN 4 INJERTO (BLS 8X16)', N'NIU', N'Frutales', 12, 12, 20),
    (N'220', N'PLANTA DE  LUCUMA SEDA INJERTO (BLS 8X16)', N'NIU', N'Frutales', 12, 12, 20),
    (N'22', N'PLANTA DE  MANDARINA RIO DE ORO INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'219', N'PLANTA DE  NARANJA HUANDO INJERTA  (BL 8X16)', N'NIU', N'Frutales', 10, 10, 20),
    (N'210', N'PLANTA DE  MANDARINA MORIA INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'21', N'PLANTA DE  BLANQUILLO MEJORADO INJERTO', N'NIU', N'Frutales', 6, 6, 20),
    (N'208', N'PLANTA DE  MANDARINA TEMPRANERA MIOWASI', N'NIU', N'Frutales', 7, 7, 20),
    (N'207', N'PLANTA DE  MANDARINA SATSUMA OWARI INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'203', N'PLANTA DE  PERA ITALIANA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'202', N'PLANTA DE  UVA BORGOÑA BLANCA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'200', N'PLANTA DE  LIMON DULCE INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'20', N'PLANTA DE  DURAZNO HUAYCO ROJO INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'2', N'PLANTA DE  MANZANA ISRAEL INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'196', N'PLANTA DE  MANDARINA SATSUMA INJERTO (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'195', N'PLANTA DE  KIN KAN INJERTO (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'192', N'PLANTA DE  PALTA NAVAL AZUL INJERTA EN ZUTANO (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'191', N'PLANTA DE  PALTA NAVAL AZUL INJERTA EN TOPA TOPA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'19', N'PLANTA DE  MANZANA CANA (SANTA ROSA) INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'189', N'PLANTA DE  CIRUELO INJERTO (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'188', N'PLANTA DE  DAMASCO INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'187', N'PLANTA DE  PALTA HASS MEJORADA INJERTA EN ZUTANO', N'NIU', N'Frutales', 20, 20, 20),
    (N'186', N'PLANTA DE  PALTA HASS MEJORADA INJERTA EN ANTILLANO', N'NIU', N'Frutales', 35, 35, 20),
    (N'185', N'PLANTA DE  PALTA VILLACAMPA INJERTA EN TOPA TOPA', N'NIU', N'Frutales', 10, 10, 20),
    (N'184', N'PLANTA DE  NECTARIN INJERTA', N'NIU', N'Frutales', 8, 8, 20),
    (N'183', N'PLANTA DE  ALMENDRA INJERTA', N'NIU', N'Frutales', 9, 9, 20),
    (N'180', N'PLANTA DE  UVA VARIADAS INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'18', N'PLANTA DE  LIMON SUTIL INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'179', N'PLANTA DE  PALTA HASS MEJORADA EN TOPA TOPA INJERTA', N'NIU', N'Frutales', 17, 17, 20),
    (N'173', N'PLANTA DE  CIRUELO CHILENO INJERTO', N'NIU', N'Frutales', 7, 7, 20),
    (N'17', N'PLANTA DE  MANDARINA RIO DE ORO INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'16', N'PLANTA DE  LUCUMA BELTRAN 4 INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'15', N'PLANTA DE  LIMON SUTIL INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'14', N'PLANTA DE  NARANJA HUANDO INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'138', N'PLANTA DE  CAQUI ROJO BRILLANTE INJERTO', N'NIU', N'Frutales', 40, 40, 20),
    (N'137', N'PLANTA DE  PERA PACAR INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'136', N'PLANTA DE  PALTA ZUTANO INJERTA EN ZUTANO', N'NIU', N'Frutales', 12, 12, 20),
    (N'133', N'PLANTA DE  PALTA LINDA INJERTA EN TOPA TOPA', N'NIU', N'Frutales', 10, 10, 20),
    (N'126', N'PLANTA DE  CIRUELO AMARILLO REINACLAUDIA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'125', N'PLANTA DE  TORONJA ROJA (POMELO) INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'124', N'PLANTA DE  MANZANA DE AGUA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'12', N'PLANTA DE  LIMON TAHITI INJERTA', N'NIU', N'Frutales', 8, 8, 20),
    (N'11', N'PLANTA DE  LIMON TAHITI INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'109', N'PLANTA DE  MANDARINA W.MURCOT INJERTA', N'NIU', N'Frutales', 7, 7, 20),
    (N'108', N'PLANTA DE  DURAZNO HUAYCO ROJO INJERTA (BG)', N'NIU', N'Frutales', 25, 25, 20),
    (N'107', N'PLANTA DE  PERA CHILENA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'106', N'PLANTA DE  PALTA HASS INJERTA EN TOPA TOPA', N'NIU', N'Frutales', 10, 10, 20),
    (N'101', N'PLANTA DE  CIRUELO SANTA ROSA INJERTA', N'NIU', N'Frutales', 6, 6, 20),
    (N'10', N'PLANTA DE  OLIVO INJERTA', N'NIU', N'Frutales', 10, 10, 20),
    (N'1', N'PLANTA DE  SAKURA INJERTA', N'NIU', N'Frutales', 35, 35, 20),
    (N'99999', N'Semilla de Pouteria lucuma', N'KGM', N'Semillas', 10, NULL, 40);

BEGIN TRANSACTION;

INSERT INTO erp.Categoria (EmpresaId, Nombre, Descripcion, Estado, FechaRegistro, UsuarioRegistro)
SELECT DISTINCT e.EmpresaId, c.Categoria, N'', 1, SYSUTCDATETIME(), N'catalogo-productos-empresa'
FROM @Empresas e
CROSS JOIN @Catalogo c
WHERE NOT EXISTS (
    SELECT 1
    FROM erp.Categoria ca
    WHERE ca.EmpresaId = e.EmpresaId
      AND UPPER(LTRIM(RTRIM(ca.Nombre))) = UPPER(LTRIM(RTRIM(c.Categoria)))
);

;WITH Base AS (
    SELECT
        Nombre = LEFT(LTRIM(RTRIM(Nombre)), 200),
        NombreClave = UPPER(LTRIM(RTRIM(Nombre))),
        UnidadMedida = LEFT(LTRIM(RTRIM(UnidadMedida)), 20),
        Categoria = LEFT(LTRIM(RTRIM(Categoria)), 100),
        VentaValorUnitarioSinIgv,
        VentaPrecioUnitarioConIgv,
        TipoAfectacionIgv,
        AfectoIgv = CONVERT(bit, CASE WHEN TipoAfectacionIgv = 10 THEN 1 ELSE 0 END),
        Prioridad = ROW_NUMBER() OVER (
            PARTITION BY UPPER(LTRIM(RTRIM(Nombre)))
            ORDER BY
                CASE WHEN VentaValorUnitarioSinIgv IS NOT NULL OR VentaPrecioUnitarioConIgv IS NOT NULL THEN 0 ELSE 1 END,
                CodigoInterno
        )
    FROM @Catalogo
    WHERE TipoAfectacionIgv IN (10, 20, 40)
), Productos AS (
    SELECT
        Nombre,
        NombreClave,
        Categoria,
        UnidadMedida,
        AfectoIgv,
        TieneDetraccion = CONVERT(bit, CASE WHEN TipoAfectacionIgv = 20 THEN 1 ELSE 0 END),
        PorcentajeDetraccion = CONVERT(decimal(5,2), CASE WHEN TipoAfectacionIgv = 20 THEN 1.50 ELSE 0 END),
        Stock = CONVERT(decimal(18,2), 10000),
        PrecioVentaSinIgv = CONVERT(decimal(18,2),
            CASE
                WHEN TipoAfectacionIgv = 10 THEN COALESCE(VentaValorUnitarioSinIgv, ROUND(VentaPrecioUnitarioConIgv / 1.18, 2), 0)
                ELSE COALESCE(VentaValorUnitarioSinIgv, VentaPrecioUnitarioConIgv, 0)
            END),
        PrecioVentaConIgv = CONVERT(decimal(18,2),
            CASE
                WHEN TipoAfectacionIgv = 10 THEN COALESCE(VentaPrecioUnitarioConIgv, ROUND(VentaValorUnitarioSinIgv * 1.18, 2), 0)
                ELSE COALESCE(VentaPrecioUnitarioConIgv, VentaValorUnitarioSinIgv, 0)
            END)
    FROM Base
    WHERE Prioridad = 1
), ProductosEmpresa AS (
    SELECT
        e.EmpresaId,
        p.Nombre,
        p.NombreClave,
        p.Categoria,
        p.UnidadMedida,
        p.AfectoIgv,
        p.TieneDetraccion,
        p.PorcentajeDetraccion,
        p.Stock,
        p.PrecioVentaSinIgv,
        p.PrecioVentaConIgv
    FROM @Empresas e
    CROSS JOIN Productos p
)
MERGE erp.Producto WITH (HOLDLOCK) AS target
USING ProductosEmpresa AS source
ON target.EmpresaId = source.EmpresaId
   AND UPPER(LTRIM(RTRIM(target.Nombre))) = source.NombreClave
WHEN MATCHED THEN
    UPDATE SET
        Categoria = source.Categoria,
        UnidadMedida = source.UnidadMedida,
        AfectoIgv = source.AfectoIgv,
        Stock = source.Stock,
        PrecioVentaSinIgv = source.PrecioVentaSinIgv,
        PrecioVentaConIgv = source.PrecioVentaConIgv,
        TieneDetraccion = source.TieneDetraccion,
        PorcentajeDetraccion = source.PorcentajeDetraccion,
        Estado = 1
WHEN NOT MATCHED BY TARGET THEN
    INSERT (
        EmpresaId,
        Categoria,
        Nombre,
        UnidadMedida,
        Stock,
        AfectoIgv,
        PrecioVentaSinIgv,
        PrecioVentaConIgv,
        TieneDetraccion,
        PorcentajeDetraccion,
        FechaRegistro,
        UsuarioRegistro,
        Estado
    )
    VALUES (
        source.EmpresaId,
        source.Categoria,
        source.Nombre,
        source.UnidadMedida,
        source.Stock,
        source.AfectoIgv,
        source.PrecioVentaSinIgv,
        source.PrecioVentaConIgv,
        source.TieneDetraccion,
        source.PorcentajeDetraccion,
        SYSUTCDATETIME(),
        N'catalogo-productos-empresa',
        1
    );

DECLARE @ProductosPorEmpresa int = (
    SELECT COUNT(*)
    FROM (
        SELECT UPPER(LTRIM(RTRIM(Nombre))) AS NombreClave
        FROM @Catalogo
        WHERE TipoAfectacionIgv IN (10, 20, 40)
        GROUP BY UPPER(LTRIM(RTRIM(Nombre)))
    ) x
);
DECLARE @EmpresasCargadas int = (SELECT COUNT(*) FROM @Empresas);

COMMIT TRANSACTION;

PRINT CONCAT(N'Empresas procesadas: ', @EmpresasCargadas);
PRINT CONCAT(N'Productos base por empresa: ', @ProductosPorEmpresa);
PRINT CONCAT(N'Productos esperados empresa x catalogo: ', @EmpresasCargadas * @ProductosPorEmpresa);
GO



