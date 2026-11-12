USE MiniVentas;
GO

IF EXISTS (SELECT 1 FROM seguridad.USUARIO WHERE rol_id = 1)
   AND NOT EXISTS (
       SELECT 1 FROM ventas.VENDEDOR v
       INNER JOIN seguridad.USUARIO u ON v.usuario_id = u.usuario_id
       WHERE u.rol_id = 1
   )
BEGIN
    INSERT INTO ventas.VENDEDOR (usuario_id, codigo_vendedor, zona_ventas)
    SELECT TOP 1 usuario_id, 'VEN-ADMIN', N'SEDE PRINCIPAL'
    FROM seguridad.USUARIO
    WHERE rol_id = 1;
END
GO

CREATE OR ALTER PROCEDURE seguridad.sp_ListarUsuarios
    @busqueda NVARCHAR(120) = NULL,
    @rol_id TINYINT = NULL,
    @solo_activos BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @busqueda = NULLIF(TRIM(@busqueda), N'');

    DECLARE @patron NVARCHAR(122) = CASE WHEN @busqueda IS NOT NULL THEN N'%' + @busqueda + N'%' END;

    SELECT
        u.usuario_id,
        u.nombre_usuario,
        u.nombre,
        u.activo,
        r.rol_id,
        r.codigo AS rol_codigo,
        r.nombre AS rol_nombre,
        v.vendedor_id,
        v.codigo_vendedor,
        v.zona_ventas
    FROM seguridad.USUARIO u
    INNER JOIN seguridad.ROL r ON u.rol_id = r.rol_id
    LEFT JOIN ventas.VENDEDOR v ON u.usuario_id = v.usuario_id
    WHERE (@rol_id IS NULL OR u.rol_id = @rol_id)
      AND (@solo_activos IS NULL OR u.activo = @solo_activos)
      AND (
          @busqueda IS NULL
          OR u.nombre_usuario LIKE @patron
          OR u.nombre LIKE @patron
          OR v.codigo_vendedor LIKE @patron
      )
    ORDER BY u.usuario_id DESC;
END
GO

CREATE OR ALTER PROCEDURE seguridad.sp_ObtenerUsuarioPorId
    @usuario_id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.usuario_id,
        u.nombre_usuario,
        u.nombre,
        u.activo,
        r.rol_id,
        r.codigo AS rol_codigo,
        r.nombre AS rol_nombre,
        v.vendedor_id,
        v.codigo_vendedor,
        v.zona_ventas
    FROM seguridad.USUARIO u
    INNER JOIN seguridad.ROL r ON u.rol_id = r.rol_id
    LEFT JOIN ventas.VENDEDOR v ON u.usuario_id = v.usuario_id
    WHERE u.usuario_id = @usuario_id;
END
GO

CREATE OR ALTER PROCEDURE seguridad.sp_CrearUsuario
    @nombre_usuario NVARCHAR(100),
    @password_hash VARCHAR(255),
    @rol_id TINYINT,
    @nombre NVARCHAR(120),
    @activo BIT = 1,
    @codigo_vendedor VARCHAR(24) = NULL,
    @zona_ventas NVARCHAR(80) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @nombre_usuario = LOWER(TRIM(@nombre_usuario));
    SET @nombre = TRIM(@nombre);
    SET @codigo_vendedor = UPPER(NULLIF(TRIM(@codigo_vendedor), ''));
    SET @zona_ventas = UPPER(NULLIF(TRIM(@zona_ventas), N''));

    IF @rol_id = 2 AND (@codigo_vendedor IS NULL OR @zona_ventas IS NULL)
        THROW 50001, 'El vendedor requiere código y zona de ventas.', 1;

    BEGIN TRAN;

    INSERT INTO seguridad.USUARIO (nombre_usuario, password_hash, rol_id, nombre, activo)
    VALUES (@nombre_usuario, @password_hash, @rol_id, @nombre, @activo);

    DECLARE @nuevo_usuario_id INT = CAST(SCOPE_IDENTITY() AS INT);

    IF @codigo_vendedor IS NOT NULL AND @zona_ventas IS NOT NULL
    BEGIN
        INSERT INTO ventas.VENDEDOR (usuario_id, codigo_vendedor, zona_ventas)
        VALUES (@nuevo_usuario_id, @codigo_vendedor, @zona_ventas);
    END

    COMMIT TRAN;

    SELECT @nuevo_usuario_id AS usuario_id;
END
GO

CREATE OR ALTER PROCEDURE seguridad.sp_ActualizarUsuario
    @usuario_id INT,
    @nombre_usuario NVARCHAR(100),
    @password_hash VARCHAR(255) = NULL,
    @rol_id TINYINT,
    @nombre NVARCHAR(120),
    @activo BIT,
    @codigo_vendedor VARCHAR(24) = NULL,
    @zona_ventas NVARCHAR(80) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @nombre_usuario = LOWER(TRIM(@nombre_usuario));
    SET @nombre = TRIM(@nombre);
    SET @password_hash = NULLIF(TRIM(@password_hash), '');
    SET @codigo_vendedor = UPPER(NULLIF(TRIM(@codigo_vendedor), ''));
    SET @zona_ventas = UPPER(NULLIF(TRIM(@zona_ventas), N''));

    IF @rol_id = 2 AND (@codigo_vendedor IS NULL OR @zona_ventas IS NULL)
        THROW 50001, 'El vendedor requiere código y zona de ventas.', 1;

    BEGIN TRAN;

    UPDATE seguridad.USUARIO
    SET nombre_usuario = @nombre_usuario,
        password_hash = COALESCE(@password_hash, password_hash),
        rol_id = @rol_id,
        nombre = @nombre,
        activo = @activo
    WHERE usuario_id = @usuario_id;

    IF @@ROWCOUNT = 0
    BEGIN
        ROLLBACK TRAN;
        SELECT CAST(0 AS BIT);
        RETURN;
    END

    IF @codigo_vendedor IS NOT NULL AND @zona_ventas IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM ventas.VENDEDOR WHERE usuario_id = @usuario_id)
        BEGIN
            UPDATE ventas.VENDEDOR
            SET codigo_vendedor = @codigo_vendedor,
                zona_ventas = @zona_ventas
            WHERE usuario_id = @usuario_id;
        END
        ELSE
        BEGIN
            INSERT INTO ventas.VENDEDOR (usuario_id, codigo_vendedor, zona_ventas)
            VALUES (@usuario_id, @codigo_vendedor, @zona_ventas);
        END
    END

    COMMIT TRAN;

    SELECT CAST(1 AS BIT);
END
GO

CREATE OR ALTER PROCEDURE seguridad.sp_CambiarEstadoUsuario
    @usuario_id INT,
    @activo BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE seguridad.USUARIO
    SET activo = @activo
    WHERE usuario_id = @usuario_id;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END
GO