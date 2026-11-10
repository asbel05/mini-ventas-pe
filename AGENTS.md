# Instrucciones para agentes

- Usa el [SDD](docs/SDD.md), el [diagrama](docs/erp-ventas-modelo.drawio) y las [reglas de venta](docs/reglas-venta.md) como referencia. Conserva las tablas y relaciones acordadas.
- Completa y prueba el ERP con comprobantes simulados. Integra SUNAT al final.
- Usa un único proyecto ASP.NET Core MVC llamado `ErpVentas`, con Razor, Bootstrap 5.3 y JavaScript nativo. Deja las reglas de negocio en los procedimientos y restricciones de SQL Server.
- Usa comandos parametrizados con tipos explícitos en ADO.NET. Guarda los scripts versionados en `database/` y califica los objetos con su esquema.
- Usa datos ficticios en las pruebas y configura los secretos fuera del repositorio.
