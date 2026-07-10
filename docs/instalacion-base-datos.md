# Instalación de la base de datos

Para una instalación nueva ejecutar los scripts SQL en este orden, desde SQL Server Management Studio o una herramienta equivalente conectada al servidor destino:

```text
001-create-database.sql
002-cargar-empresa-inicial.sql
003-cargar-categorias-financieras.sql
004-cargar-productos-catalogo.sql
005-cargar-clientes-entidades.sql
006-cargar-productos-por-empresa.sql
007-cargar-usuario-admin.sql
```

## Orden y propósito

- `001-create-database.sql`: crea el esquema `erp`, tablas, índices, relaciones, roles internos, permisos, moneda y catálogos base.
- `002-cargar-empresa-inicial.sql`: registra o actualiza las empresas iniciales Vivero Los Frutales Lima y Huaral.
- `003-cargar-categorias-financieras.sql`: carga categorías iniciales de gastos e ingresos por empresa.
- `004-cargar-productos-catalogo.sql`: carga el catálogo de productos desde Nubefact para las empresas configuradas.
- `005-cargar-clientes-entidades.sql`: carga clientes globales en `erp.Cliente`.
- `006-cargar-productos-por-empresa.sql`: carga/verifica productos por empresa con el mismo catálogo base.
- `007-cargar-usuario-admin.sql`: crea/verifica el usuario administrador inicial, su rol Identity, permisos y asociación a empresas.

## Empresas iniciales

El script `002` trabaja con estos RUC:

```text
20615082997 - VIVERO LOS FRUTALES LIMA SAC
20615619273 - VIVERO LOS FRUTALES HUARAL SAC
```

Los scripts de productos y usuario administrador filtran esas empresas por RUC. Si se agrega una empresa nueva, se debe registrar con datos fiscales reales y luego ajustar los filtros de los scripts de carga que correspondan.

## Usuario inicial

El script `007` deja disponible el acceso inicial:

```text
Usuario: admin
Password: Admin1234
```

El usuario queda activo, con rol interno `Administrador`, rol Identity `Administrador` y filas en `erp.UsuarioEmpresa` para las empresas iniciales activas.

Cambiar la contraseña después del primer ingreso.

## Validaciones rápidas

Después de ejecutar los scripts puede validarse:

```sql
SELECT EmpresaId, RUC, RazonSocial, NombreComercial, Estado
FROM erp.Empresa
ORDER BY EmpresaId;

SELECT UserName, Email, Activo, RolId
FROM erp.AspNetUsers
WHERE NormalizedUserName = N'ADMIN';

SELECT u.UserName, e.RUC, e.RazonSocial
FROM erp.UsuarioEmpresa ue
INNER JOIN erp.AspNetUsers u ON u.Id = ue.UsuarioId
INNER JOIN erp.Empresa e ON e.EmpresaId = ue.EmpresaId
WHERE u.NormalizedUserName = N'ADMIN'
ORDER BY e.RUC;
```

Todos los scripts de carga son idempotentes: se pueden volver a ejecutar para verificar o completar información sin duplicar registros clave.

## Actualizacion de bases existentes

Para bases ya instaladas, ademas de los scripts iniciales, aplicar los parches idempotentes que correspondan a la version publicada. Para el snapshot historico del cliente ejecutar:

```text
015-add-snapshot-cliente-comprobante.sql
016-add-snapshot-cliente-cotizacion-nota-pedido.sql
```

Estos scripts agregan columnas nullable y no modifican datos historicos existentes. `ClienteTipoDocumento` debe quedar como `int NULL` en `erp.Comprobante`, `erp.Cotizacion` y `erp.NotaPedido`; si una base previa lo tenia como `nvarchar`, el script `016` lo convierte a `int` para evitar errores de casteo al ver o imprimir documentos.
