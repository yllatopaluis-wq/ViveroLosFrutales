/*
Diagnostico de ingresos por cuenta financiera BCP.

La pantalla Caja y Bancos toma los importes desde erp.MovimientoCaja:
- Ingresos: movimientos activos con TipoMovimiento = 1 (INGRESO)
- Egresos: movimientos activos con TipoMovimiento = 2 (EGRESO)
- Saldo: SaldoInicial + ingresos acumulados - egresos acumulados

Ajustar @EmpresaId, @CuentaTexto y fechas antes de ejecutar si corresponde.
Dejar @FechaDesde y @FechaHasta en NULL para ver todo el acumulado.
*/

DECLARE @EmpresaId int = 1;
DECLARE @CuentaTexto nvarchar(120) = N'BCP';
DECLARE @NumeroCuenta nvarchar(80) = N'1917305260004';
DECLARE @FechaDesde date = NULL;
DECLARE @FechaHasta date = NULL;

DECLARE @FechaHastaExclusiva date = CASE
    WHEN @FechaHasta IS NULL THEN NULL
    ELSE DATEADD(day, 1, @FechaHasta)
END;

IF OBJECT_ID('tempdb..#CuentaObjetivo') IS NOT NULL DROP TABLE #CuentaObjetivo;
IF OBJECT_ID('tempdb..#MovimientosPeriodo') IS NOT NULL DROP TABLE #MovimientosPeriodo;
IF OBJECT_ID('tempdb..#MovimientosAcumulados') IS NOT NULL DROP TABLE #MovimientosAcumulados;

SELECT
    c.CuentaFinancieraId,
    c.EmpresaId,
    c.Nombre,
    c.Tipo,
    c.Banco,
    c.NumeroCuenta,
    c.Moneda,
    c.SaldoInicial,
    c.FechaSaldoInicial,
    c.Activo
INTO #CuentaObjetivo
FROM erp.CuentaFinanciera c
WHERE c.EmpresaId = @EmpresaId
  AND (
      c.Nombre LIKE N'%' + @CuentaTexto + N'%'
      OR c.Banco LIKE N'%' + @CuentaTexto + N'%'
      OR c.NumeroCuenta = @NumeroCuenta
  );

IF NOT EXISTS (SELECT 1 FROM #CuentaObjetivo)
BEGIN
    SELECT
        Mensaje = 'No se encontro una cuenta financiera con esos parametros. Revise @EmpresaId, @CuentaTexto o @NumeroCuenta.',
        @EmpresaId AS EmpresaId,
        @CuentaTexto AS CuentaTexto,
        @NumeroCuenta AS NumeroCuenta;

    SELECT
        c.CuentaFinancieraId,
        c.EmpresaId,
        c.Nombre,
        Tipo = CASE c.Tipo WHEN 1 THEN 'Caja' WHEN 2 THEN 'Banco' WHEN 3 THEN 'Billetera' ELSE CONVERT(varchar(20), c.Tipo) END,
        c.Banco,
        c.NumeroCuenta,
        c.Moneda,
        c.SaldoInicial,
        c.Activo
    FROM erp.CuentaFinanciera c
    WHERE c.EmpresaId = @EmpresaId
    ORDER BY c.Tipo, c.Nombre;

    RETURN;
END;

SELECT m.*
INTO #MovimientosPeriodo
FROM erp.MovimientoCaja m
INNER JOIN #CuentaObjetivo c
    ON c.CuentaFinancieraId = m.CuentaFinancieraId
WHERE m.EmpresaId = @EmpresaId
  AND m.Estado = 1
  AND (@FechaDesde IS NULL OR m.Fecha >= @FechaDesde)
  AND (@FechaHastaExclusiva IS NULL OR m.Fecha < @FechaHastaExclusiva);

SELECT m.*
INTO #MovimientosAcumulados
FROM erp.MovimientoCaja m
INNER JOIN #CuentaObjetivo c
    ON c.CuentaFinancieraId = m.CuentaFinancieraId
WHERE m.EmpresaId = @EmpresaId
  AND m.Estado = 1
  AND (@FechaHastaExclusiva IS NULL OR m.Fecha < @FechaHastaExclusiva);

SELECT
    c.CuentaFinancieraId,
    c.Nombre,
    Tipo = CASE c.Tipo WHEN 1 THEN 'Caja' WHEN 2 THEN 'Banco' WHEN 3 THEN 'Billetera' ELSE CONVERT(varchar(20), c.Tipo) END,
    c.Banco,
    c.NumeroCuenta,
    c.Moneda,
    c.SaldoInicial,
    IngresosPeriodo = COALESCE(SUM(CASE WHEN p.TipoMovimiento = 1 THEN p.Monto END), 0),
    EgresosPeriodo = COALESCE(SUM(CASE WHEN p.TipoMovimiento = 2 THEN p.Monto END), 0),
    IngresosAcumulados = (
        SELECT COALESCE(SUM(a.Monto), 0)
        FROM #MovimientosAcumulados a
        WHERE a.CuentaFinancieraId = c.CuentaFinancieraId
          AND a.TipoMovimiento = 1
    ),
    EgresosAcumulados = (
        SELECT COALESCE(SUM(a.Monto), 0)
        FROM #MovimientosAcumulados a
        WHERE a.CuentaFinancieraId = c.CuentaFinancieraId
          AND a.TipoMovimiento = 2
    ),
    SaldoPantalla = c.SaldoInicial
        + (
            SELECT COALESCE(SUM(a.Monto), 0)
            FROM #MovimientosAcumulados a
            WHERE a.CuentaFinancieraId = c.CuentaFinancieraId
              AND a.TipoMovimiento = 1
        )
        - (
            SELECT COALESCE(SUM(a.Monto), 0)
            FROM #MovimientosAcumulados a
            WHERE a.CuentaFinancieraId = c.CuentaFinancieraId
              AND a.TipoMovimiento = 2
        )
FROM #CuentaObjetivo c
LEFT JOIN #MovimientosPeriodo p
    ON p.CuentaFinancieraId = c.CuentaFinancieraId
GROUP BY
    c.CuentaFinancieraId,
    c.Nombre,
    c.Tipo,
    c.Banco,
    c.NumeroCuenta,
    c.Moneda,
    c.SaldoInicial,
    c.FechaSaldoInicial,
    c.Activo
ORDER BY c.Nombre;

SELECT
    TipoMovimiento = CASE m.TipoMovimiento WHEN 1 THEN 'INGRESO' WHEN 2 THEN 'EGRESO' ELSE CONVERT(varchar(20), m.TipoMovimiento) END,
    Origen = CASE m.Origen
        WHEN 1 THEN 'COBRO_CLIENTE'
        WHEN 2 THEN 'PAGO_PROVEEDOR'
        WHEN 3 THEN 'Billetera'
        WHEN 4 THEN 'OTRO'
        WHEN 5 THEN 'DEVOLUCION_CLIENTE'
        WHEN 6 THEN 'INGRESO_MANUAL'
        WHEN 7 THEN 'DEVOLUCION_PROVEEDOR'
        WHEN 8 THEN 'TRANSFERENCIA'
        ELSE CONVERT(varchar(20), m.Origen)
    END,
    Cantidad = COUNT(*),
    Total = SUM(m.Monto)
FROM #MovimientosPeriodo m
GROUP BY m.TipoMovimiento, m.Origen
ORDER BY m.TipoMovimiento, m.Origen;

SELECT
    m.MovimientoCajaId,
    m.Fecha,
    TipoMovimiento = CASE m.TipoMovimiento WHEN 1 THEN 'INGRESO' WHEN 2 THEN 'EGRESO' ELSE CONVERT(varchar(20), m.TipoMovimiento) END,
    Origen = CASE m.Origen
        WHEN 1 THEN 'COBRO_CLIENTE'
        WHEN 2 THEN 'PAGO_PROVEEDOR'
        WHEN 3 THEN 'Billetera'
        WHEN 4 THEN 'OTRO'
        WHEN 5 THEN 'DEVOLUCION_CLIENTE'
        WHEN 6 THEN 'INGRESO_MANUAL'
        WHEN 7 THEN 'DEVOLUCION_PROVEEDOR'
        WHEN 8 THEN 'TRANSFERENCIA'
        ELSE CONVERT(varchar(20), m.Origen)
    END,
    m.OrigenId,
    m.Monto,
    m.MedioPago,
    m.Descripcion,
    m.ClienteId,
    m.ProveedorId
FROM #MovimientosPeriodo m
WHERE m.TipoMovimiento = 1
ORDER BY m.Fecha, m.MovimientoCajaId;
