IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'MiniVentas ')
BEGIN
    CREATE DATABASE MiniVentas COLLATE Modern_Spanish_CI_AS;
END
GO

USE MiniVentas;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'seguridad')
    EXEC('CREATE SCHEMA seguridad AUTHORIZATION dbo;');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'ventas')
    EXEC('CREATE SCHEMA ventas AUTHORIZATION dbo;');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'facturacion')
    EXEC('CREATE SCHEMA facturacion AUTHORIZATION dbo;');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'despacho')
    EXEC('CREATE SCHEMA despacho AUTHORIZATION dbo;');
GO