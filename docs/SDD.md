# Diseño del sistema

El diseño implementa las reglas comerciales, de facturación electrónica simulada y control de cobros para el mercado peruano.

## Arquitectura

`MiniVentas` es un único proyecto ASP.NET Core MVC con Razor y autenticación por cookie con revalidación reactiva.

- `Controllers/`, `Views/` y `ViewModels/` organizan la interacción con el usuario. Los `ViewModels/` se estructuran en subcarpetas por módulo (`Productos/`, `Clientes/`, `Usuarios/`, `Ventas/`).
- `Security/` centraliza la creación de claims (`SesionUsuario`) y la revalidación reactiva de la sesión (`ValidacionCookie` heredando de `CookieAuthenticationEvents`), actualizando roles, perfil de vendedor y revocando accesos en tiempo real ante bajas de cuenta.
- `Data/` contiene los DAO con ADO.NET y `Microsoft.Data.SqlClient`. Ejecuta procedimientos con parámetros tipados, gestiona códigos SQL centralizados (`CodigosErrorSql`) y mapea sus resultados.
- `Services/` coordina flujos como el registro de la venta y la emisión simulada. El cliente de SUNAT se incorpora al final en `Integrations/Sunat/`.

MVC valida los formularios y obtiene la identidad de la sesión. SQL Server aplica las reglas de negocio y controla las transacciones. C# se encarga de las contraseñas (sin aplicar `Trim()`), los archivos y las llamadas externas, que se ejecutan fuera de la transacción.

### Frontend

- MVC renderiza HTML en el servidor con vistas Razor tipadas por ViewModel. Las vistas comparten un layout y reutilizan vistas parciales para los bloques comunes.
- Bootstrap 5.3.8 aporta los componentes y el diseño adaptable con botones sólidos (`btn-*`), badges limpios sin modificación de padding inline y sin tipografías monoespaciadas forzadas. Los archivos estáticos se sirven desde `wwwroot/`.
- Los formularios se envían a acciones MVC. JavaScript nativo maneja las interacciones en el navegador y usa `fetch` para consultas que necesitan responder sin recargar la página. SQL Server calcula los importes definitivos.

## Base de datos

La base `MiniVentas` usa cuatro esquemas con `dbo` como propietario. El [diagrama editable](erp-ventas-modelo.drawio) define las tablas y relaciones.

| Esquema | Tablas |
| --- | --- |
| `seguridad` | `ROL`, `USUARIO` |
| `ventas` | `CLIENTE`, `VENDEDOR`, `PRODUCTO`, `PEDIDO`, `PEDIDO_DETALLE`, `PEDIDO_ESTADO`, `PAGO`, `MEDIO_PAGO`, `PAGO_ESTADO` |
| `facturacion` | `SERIE_COMPROBANTE`, `COMPROBANTE`, `COMPROBANTE_ESTADO` |
| `despacho` | `TRANSPORTISTA`, `ENVIO`, `ENVIO_ESTADO`, `GUIA_REMISION` |

## Implementación

SQL Server Developer corre en Docker con un volumen local. La aplicación usa el SDK de .NET 10. Los scripts numerados en `database/` crean los esquemas, las tablas, los catálogos, los tipos de tabla y los procedimientos. La cuenta de la aplicación tiene permisos de ejecución sobre sus procedimientos. La cadena de conexión y las credenciales se configuran con secretos de usuario o variables de entorno.

1. Crear el esquema de datos según el diagrama, TVPs y catálogos (`01` al `04`). *(Completado)*
2. Implementar acceso, roles, revalidación de sesiones, usuarios y catálogos (**Productos** y **Clientes** con Catálogo 06 SUNAT). *(Completado)*
3. Implementar ventas (POS), emisión simulada, cobros y despachos. Verificar el flujo completo, incluidos los pagos mixtos, los reintentos, la reversión ante fallos y las ventas concurrentes de una misma serie. *(En progreso)*
4. Integrar SUNAT una vez que el sistema completo funcione con emisión simulada.
