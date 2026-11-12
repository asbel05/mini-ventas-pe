# ERP de ventas (`mini-ventas-pe`)

Sistema comercial para registrar pedidos, emitir boletas y facturas simuladas, cobrar con varios medios de pago y gestionar despachos con guías de remisión.

**Stack** · .NET 10, ASP.NET Core MVC con Razor, Bootstrap 5.3.8, JavaScript nativo, ADO.NET (`Microsoft.Data.SqlClient`) y SQL Server Developer en Docker.

**Estado actual** · Base de datos, autenticación por cookies y módulos de catálogos (**Productos** y **Clientes** con Catálogo 06 SUNAT) implementados. Punto de venta (POS), facturación y despacho en progreso.

- [Diseño del sistema](docs/SDD.md)
- [Diagrama editable](docs/erp-ventas-modelo.drawio)

---

## Cómo levantar el entorno local

### 1. Iniciar SQL Server en Docker
Desde la raíz del repositorio:

```bash
docker compose up -d
```

Esto levanta el contenedor `mini-ventas-sql` en el puerto `1433` con collation `Modern_Spanish_CI_AS`.

### 2. Ejecutar los scripts de base de datos
Ejecuta los scripts de la carpeta `database/` en orden numérico (`01` al `07`):

**En PowerShell:**
```powershell
Get-ChildItem database\*.sql | Sort-Object Name | ForEach-Object {
    Get-Content $_.FullName -Raw | docker exec -i mini-ventas-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "MiniVentas123!@" -C
}
```

**En Bash:**
```bash
for f in database/*.sql; do
    docker exec -i mini-ventas-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "MiniVentas123!@" -C < "$f"
done
```

### 3. Configurar la cadena de conexión (`ConnectionStrings__CadenaSql`)
La aplicación lee la cadena `ConnectionStrings:CadenaSql`. Puedes definirla como variable de entorno o en `src/MiniVentas/appsettings.Development.json`:

**Opción A — Variable de entorno (PowerShell):**
```powershell
$env:ConnectionStrings__CadenaSql="Server=localhost,1433;Database=MiniVentas;User Id=sa;Password=MiniVentas123!@;TrustServerCertificate=True;Encrypt=False;"
```

**Opción B — `src/MiniVentas/appsettings.Development.json`:**
```json
{
  "ConnectionStrings": {
    "CadenaSql": "Server=localhost,1433;Database=MiniVentas;User Id=sa;Password=MiniVentas123!@;TrustServerCertificate=True;Encrypt=False;"
  }
}
```

### 4. Ejecutar la aplicación
```bash
dotnet run --project src/MiniVentas
```

Al iniciar por primera vez, el sistema crea automáticamente el usuario administrador inicial si la tabla de usuarios está vacía:
- **Usuario:** `admin`
- **Contraseña:** `admin123`
