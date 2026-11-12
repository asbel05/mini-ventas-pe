USE MiniVentas;
GO

CREATE OR ALTER PROCEDURE ventas.sp_ListarProductos
    @busqueda NVARCHAR(120) = NULL,
    @solo_activos BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @busqueda = NULLIF(TRIM(@busqueda), N'');

    DECLARE @patron_sku VARCHAR(122) = CASE WHEN @busqueda IS NOT NULL THEN '%' + CAST(@busqueda AS VARCHAR(120)) + '%' END;
    DECLARE @patron_nombre NVARCHAR(122) = CASE WHEN @busqueda IS NOT NULL THEN N'%' + @busqueda + N'%' END;

    SELECT
        producto_id,
        sku,
        nombre,
        precio_venta,
        activo
    FROM ventas.PRODUCTO
    WHERE (@solo_activos IS NULL OR activo = @solo_activos)
      AND (@busqueda IS NULL OR sku LIKE @patron_sku OR nombre LIKE @patron_nombre)
    ORDER BY producto_id DESC;
END
GO

CREATE OR ALTER PROCEDURE ventas.sp_ObtenerProductoPorId
    @producto_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        producto_id,
        sku,
        nombre,
        precio_venta,
        activo
    FROM ventas.PRODUCTO
    WHERE producto_id = @producto_id;
END
GO

CREATE OR ALTER PROCEDURE ventas.sp_CrearProducto
    @sku VARCHAR(32),
    @nombre NVARCHAR(120),
    @precio_venta DECIMAL(12,2),
    @activo BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    SET @sku = UPPER(TRIM(@sku));
    SET @nombre = TRIM(@nombre);

    INSERT INTO ventas.PRODUCTO (sku, nombre, precio_venta, activo)
    VALUES (@sku, @nombre, @precio_venta, @activo);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS producto_id;
END
GO

CREATE OR ALTER PROCEDURE ventas.sp_ActualizarProducto
    @producto_id INT,
    @sku VARCHAR(32),
    @nombre NVARCHAR(120),
    @precio_venta DECIMAL(12,2),
    @activo BIT
AS
BEGIN
    SET NOCOUNT ON;
    SET @sku = UPPER(TRIM(@sku));
    SET @nombre = TRIM(@nombre);
    UPDATE ventas.PRODUCTO
    SET sku = @sku,
        nombre = @nombre,
        precio_venta = @precio_venta,
        activo = @activo
    WHERE producto_id = @producto_id;
    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END
GO

CREATE OR ALTER PROCEDURE ventas.sp_CambiarEstadoProducto
    @producto_id INT,
    @activo BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ventas.PRODUCTO
    SET activo = @activo
    WHERE producto_id = @producto_id;
    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END
GO