USE MiniVentas;
GO

IF NOT EXISTS (SELECT 1 FROM ventas.CLIENTE WHERE tipo_documento = '0' AND numero_documento = '00000000')
BEGIN
    INSERT INTO ventas.CLIENTE (tipo_documento, numero_documento, nombre, origen_datos, consultado_en)
    VALUES ('0', '00000000', N'CLIENTES VARIOS', 'SISTEMA', NULL);
END
GO

CREATE OR ALTER PROCEDURE ventas.sp_ListarClientes
    @busqueda NVARCHAR(160) = NULL,
    @tipo_documento CHAR(1) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @busqueda = NULLIF(TRIM(@busqueda), N'');
    SET @tipo_documento = NULLIF(TRIM(@tipo_documento), '');

    DECLARE @patron_doc VARCHAR(122) = CASE WHEN @busqueda IS NOT NULL THEN '%' + CAST(@busqueda AS VARCHAR(120)) + '%' END;
    DECLARE @patron_nombre NVARCHAR(162) = CASE WHEN @busqueda IS NOT NULL THEN N'%' + @busqueda + N'%' END;

    SELECT
        cliente_id,
        tipo_documento,
        numero_documento,
        nombre,
        origen_datos,
        consultado_en
    FROM ventas.CLIENTE
    WHERE tipo_documento <> '0'
      AND (@tipo_documento IS NULL OR tipo_documento = @tipo_documento)
      AND (@busqueda IS NULL OR numero_documento LIKE @patron_doc OR nombre LIKE @patron_nombre)
    ORDER BY cliente_id DESC;
END
GO

CREATE OR ALTER PROCEDURE ventas.sp_ObtenerClientePorId
    @cliente_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        cliente_id,
        tipo_documento,
        numero_documento,
        nombre,
        origen_datos,
        consultado_en
    FROM ventas.CLIENTE
    WHERE cliente_id = @cliente_id
      AND tipo_documento <> '0';
END
GO

CREATE OR ALTER PROCEDURE ventas.sp_CrearCliente
    @tipo_documento CHAR(1),
    @numero_documento VARCHAR(12),
    @nombre NVARCHAR(160),
    @origen_datos VARCHAR(12) = 'MANUAL',
    @consultado_en DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @tipo_documento NOT IN ('1', '4', '6', '7')
        THROW 50001, 'El tipo de documento no es válido para el registro de clientes.', 1;

    SET @numero_documento = UPPER(TRIM(@numero_documento));
    SET @nombre = UPPER(TRIM(@nombre));

    INSERT INTO ventas.CLIENTE (tipo_documento, numero_documento, nombre, origen_datos, consultado_en)
    VALUES (@tipo_documento, @numero_documento, @nombre, @origen_datos, @consultado_en);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS cliente_id;
END
GO

CREATE OR ALTER PROCEDURE ventas.sp_ActualizarCliente
    @cliente_id INT,
    @tipo_documento CHAR(1),
    @numero_documento VARCHAR(12),
    @nombre NVARCHAR(160)
AS
BEGIN
    SET NOCOUNT ON;

    IF @tipo_documento NOT IN ('1', '4', '6', '7')
        THROW 50001, 'El tipo de documento no es válido para la edición de clientes.', 1;

    SET @numero_documento = UPPER(TRIM(@numero_documento));
    SET @nombre = UPPER(TRIM(@nombre));

    UPDATE ventas.CLIENTE
    SET tipo_documento = @tipo_documento,
        numero_documento = @numero_documento,
        nombre = @nombre
    WHERE cliente_id = @cliente_id
      AND tipo_documento <> '0';

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END
GO