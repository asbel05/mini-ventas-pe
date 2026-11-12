# Instrucciones para agentes

- Usa el [SDD](docs/SDD.md) y el [diagrama](docs/erp-ventas-modelo.drawio) como referencia. Conserva las tablas y relaciones acordadas.
- Completa y prueba el ERP con comprobantes simulados. Integra SUNAT al final.
- Usa un único proyecto ASP.NET Core MVC llamado `MiniVentas`, con Razor, Bootstrap 5.3 y JavaScript nativo. Deja las reglas de negocio en los procedimientos y restricciones de SQL Server.
- Estructura los `ViewModels/` en subcarpetas por módulo (`Productos/`, `Clientes/`, `Usuarios/`, `Ventas/`).
- En la interfaz de usuario: usa componentes estándar de Bootstrap 5.3, botones sólidos (`btn-primary`, `btn-secondary`, etc.; no `btn-outline`), badges limpios sin modifiers de relleno inline o `fs-6`, y no utilices `font-monospace`. En listados usa `card mb-4` para filtros (sin sombras) y tabla directa dentro de `.table-responsive` con `table table-hover table-bordered align-middle`.
- En autenticación y seguridad: centraliza claims en `Security/SesionUsuario.cs`, revalida sesiones en vivo con `Security/ValidacionCookie.cs` (`CookieAuthenticationEvents`), y no apliques `.Trim()` a las contraseñas.
- En ADO.NET: usa comandos parametrizados con tipos explícitos (`SqlDbType`, `ParameterDirection`, `Size`), gestiona violaciones de unicidad con `CodigosErrorSql` (2627/2601), guarda los scripts versionados en `database/` y califica todos los objetos con su esquema.
- Usa datos ficticios en las pruebas y configura los secretos fuera del repositorio.
