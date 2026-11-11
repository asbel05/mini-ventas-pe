USE MiniVentas;
GO
CREATE OR ALTER PROCEDURE seguridad.sp_ObtenerUsuarioLogin
    @nombre_usuario NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        u.usuario_id,
        u.nombre_usuario,
        u.password_hash,
        u.nombre,
        u.activo,
        r.rol_id,
        r.codigo AS rol_codigo,
        r.nombre AS rol_nombre,
        v.vendedor_id
    FROM seguridad.USUARIO u
    INNER JOIN seguridad.ROL r ON u.rol_id = r.rol_id
    LEFT JOIN ventas.VENDEDOR v ON u.usuario_id = v.usuario_id
    WHERE u.nombre_usuario = @nombre_usuario;
END
GO