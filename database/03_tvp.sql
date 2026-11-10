USE MiniVentas;
GO

IF TYPE_ID(N'ventas.TVP_PedidoDetalle') IS NOT NULL
    DROP TYPE ventas.TVP_PedidoDetalle;
GO
CREATE TYPE ventas.TVP_PedidoDetalle AS TABLE (
    producto_id INT NOT NULL,
    descripcion NVARCHAR(160) NOT NULL,
    cantidad DECIMAL(12,3) NOT NULL,
    precio_unitario DECIMAL(12,2) NOT NULL
);
GO

IF TYPE_ID(N'ventas.TVP_Pago') IS NOT NULL
    DROP TYPE ventas.TVP_Pago;
GO
CREATE TYPE ventas.TVP_Pago AS TABLE (
    medio_pago_id TINYINT NOT NULL,
    monto DECIMAL(12,2) NOT NULL,
    referencia VARCHAR(100) NULL
);
GO