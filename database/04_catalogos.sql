USE MiniVentas;
GO

-- Roles
IF NOT EXISTS (SELECT 1 FROM seguridad.ROL WHERE rol_id = 1)
    INSERT INTO seguridad.ROL (rol_id, codigo, nombre) VALUES (1, 'ADMIN', 'Administrador');

IF NOT EXISTS (SELECT 1 FROM seguridad.ROL WHERE rol_id = 2)
    INSERT INTO seguridad.ROL (rol_id, codigo, nombre) VALUES (2, 'VENDEDOR', 'Vendedor');
GO

-- Estados de Pedido
IF NOT EXISTS (SELECT 1 FROM ventas.PEDIDO_ESTADO WHERE pedido_estado_id = 1)
    INSERT INTO ventas.PEDIDO_ESTADO (pedido_estado_id, codigo, nombre) VALUES (1, 'CONFIRMADO', 'Confirmado');

IF NOT EXISTS (SELECT 1 FROM ventas.PEDIDO_ESTADO WHERE pedido_estado_id = 2)
    INSERT INTO ventas.PEDIDO_ESTADO (pedido_estado_id, codigo, nombre) VALUES (2, 'ANULADO', 'Anulado');
GO

-- Estados de Pago
IF NOT EXISTS (SELECT 1 FROM ventas.PAGO_ESTADO WHERE pago_estado_id = 1)
    INSERT INTO ventas.PAGO_ESTADO (pago_estado_id, codigo, nombre) VALUES (1, 'PENDIENTE', 'Pendiente');

IF NOT EXISTS (SELECT 1 FROM ventas.PAGO_ESTADO WHERE pago_estado_id = 2)
    INSERT INTO ventas.PAGO_ESTADO (pago_estado_id, codigo, nombre) VALUES (2, 'CONFIRMADO', 'Confirmado');

IF NOT EXISTS (SELECT 1 FROM ventas.PAGO_ESTADO WHERE pago_estado_id = 3)
    INSERT INTO ventas.PAGO_ESTADO (pago_estado_id, codigo, nombre) VALUES (3, 'ANULADO', 'Anulado');
GO

-- Medios de Pago
IF NOT EXISTS (SELECT 1 FROM ventas.MEDIO_PAGO WHERE medio_pago_id = 1)
    INSERT INTO ventas.MEDIO_PAGO (medio_pago_id, codigo, nombre, activo) VALUES (1, 'EFECTIVO', 'Efectivo', 1);

IF NOT EXISTS (SELECT 1 FROM ventas.MEDIO_PAGO WHERE medio_pago_id = 2)
    INSERT INTO ventas.MEDIO_PAGO (medio_pago_id, codigo, nombre, activo) VALUES (2, 'YAPE', 'Yape / Plin', 1);

IF NOT EXISTS (SELECT 1 FROM ventas.MEDIO_PAGO WHERE medio_pago_id = 3)
    INSERT INTO ventas.MEDIO_PAGO (medio_pago_id, codigo, nombre, activo) VALUES (3, 'TRANSFERENCIA', 'Transferencia Bancaria', 1);
GO

-- Estados de Comprobante
IF NOT EXISTS (SELECT 1 FROM facturacion.COMPROBANTE_ESTADO WHERE comprobante_estado_id = 1)
    INSERT INTO facturacion.COMPROBANTE_ESTADO (comprobante_estado_id, codigo, nombre) VALUES (1, 'PENDIENTE', 'Pendiente');

IF NOT EXISTS (SELECT 1 FROM facturacion.COMPROBANTE_ESTADO WHERE comprobante_estado_id = 2)
    INSERT INTO facturacion.COMPROBANTE_ESTADO (comprobante_estado_id, codigo, nombre) VALUES (2, 'EMITIDO', 'Emitido');

IF NOT EXISTS (SELECT 1 FROM facturacion.COMPROBANTE_ESTADO WHERE comprobante_estado_id = 3)
    INSERT INTO facturacion.COMPROBANTE_ESTADO (comprobante_estado_id, codigo, nombre) VALUES (3, 'RECHAZADO', 'Rechazado');
GO

-- Series Iniciales de Facturación Electrónica
IF NOT EXISTS (SELECT 1 FROM facturacion.SERIE_COMPROBANTE WHERE tipo = 'BOLETA' AND serie = 'B001')
    INSERT INTO facturacion.SERIE_COMPROBANTE (tipo, serie, ultimo_correlativo, activo) VALUES ('BOLETA', 'B001', 0, 1);

IF NOT EXISTS (SELECT 1 FROM facturacion.SERIE_COMPROBANTE WHERE tipo = 'FACTURA' AND serie = 'F001')
    INSERT INTO facturacion.SERIE_COMPROBANTE (tipo, serie, ultimo_correlativo, activo) VALUES ('FACTURA', 'F001', 0, 1);
GO

-- Estados de Envío (Despacho)
IF NOT EXISTS (SELECT 1 FROM despacho.ENVIO_ESTADO WHERE envio_estado_id = 1)
    INSERT INTO despacho.ENVIO_ESTADO (envio_estado_id, codigo, nombre) VALUES (1, 'PENDIENTE', 'Pendiente');

IF NOT EXISTS (SELECT 1 FROM despacho.ENVIO_ESTADO WHERE envio_estado_id = 2)
    INSERT INTO despacho.ENVIO_ESTADO (envio_estado_id, codigo, nombre) VALUES (2, 'DESPACHADO', 'Despachado');

IF NOT EXISTS (SELECT 1 FROM despacho.ENVIO_ESTADO WHERE envio_estado_id = 3)
    INSERT INTO despacho.ENVIO_ESTADO (envio_estado_id, codigo, nombre) VALUES (3, 'ENTREGADO', 'Entregado');
GO