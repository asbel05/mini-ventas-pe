USE MiniVentas;
GO


-- ESQUEMA SEGURIDAD
IF OBJECT_ID(N'seguridad.ROL', N'U') IS NULL
BEGIN
    CREATE TABLE seguridad.ROL (
        rol_id TINYINT NOT NULL,
        codigo VARCHAR(24) NOT NULL,
        nombre NVARCHAR(60) NOT NULL,
        CONSTRAINT PK_ROL PRIMARY KEY (rol_id),
        CONSTRAINT UQ_ROL_codigo UNIQUE (codigo)
    );
END
GO

IF OBJECT_ID(N'seguridad.USUARIO', N'U') IS NULL
BEGIN
    CREATE TABLE seguridad.USUARIO (
        usuario_id INT IDENTITY(1,1) NOT NULL,
        nombre_usuario NVARCHAR(100) NOT NULL,
        password_hash VARCHAR(255) NOT NULL,
        rol_id TINYINT NOT NULL,
        nombre NVARCHAR(120) NOT NULL,
        activo BIT NOT NULL CONSTRAINT DF_USUARIO_activo DEFAULT (1),
        CONSTRAINT PK_USUARIO PRIMARY KEY (usuario_id),
        CONSTRAINT UQ_USUARIO_nombre_usuario UNIQUE (nombre_usuario),
        CONSTRAINT FK_USUARIO_ROL FOREIGN KEY (rol_id) REFERENCES seguridad.ROL(rol_id)
    );
END
GO

-- ESQUEMA VENTAS
IF OBJECT_ID(N'ventas.CLIENTE', N'U') IS NULL
BEGIN
    CREATE TABLE ventas.CLIENTE (
        cliente_id INT IDENTITY(1,1) NOT NULL,
        tipo_documento CHAR(1) NOT NULL, -- 1: DNI, 6: RUC
        numero_documento VARCHAR(12) NOT NULL,
        nombre NVARCHAR(160) NOT NULL,
        origen_datos VARCHAR(12) NOT NULL, -- MANUAL, RENIEC, SUNAT, VENTA_DIRECTA
        consultado_en DATETIME2 NULL,
        CONSTRAINT PK_CLIENTE PRIMARY KEY (cliente_id),
        CONSTRAINT UQ_CLIENTE_documento UNIQUE (tipo_documento, numero_documento),
        CONSTRAINT CK_CLIENTE_tipo_documento CHECK (
            (tipo_documento = '0' AND numero_documento = '00000000' AND nombre = N'CLIENTES VARIOS')
            OR tipo_documento IN ('1', '4', '6', '7')
        )
    );
END
GO

IF OBJECT_ID(N'ventas.VENDEDOR', N'U') IS NULL
BEGIN
    CREATE TABLE ventas.VENDEDOR (
        vendedor_id INT IDENTITY(1,1) NOT NULL,
        usuario_id INT NOT NULL,
        codigo_vendedor VARCHAR(24) NOT NULL,
        zona_ventas NVARCHAR(80) NOT NULL,
        CONSTRAINT PK_VENDEDOR PRIMARY KEY (vendedor_id),
        CONSTRAINT UQ_VENDEDOR_usuario_id UNIQUE (usuario_id),
        CONSTRAINT UQ_VENDEDOR_codigo UNIQUE (codigo_vendedor),
        CONSTRAINT FK_VENDEDOR_USUARIO FOREIGN KEY (usuario_id) REFERENCES seguridad.USUARIO(usuario_id)
    );
END
GO

IF OBJECT_ID(N'ventas.PRODUCTO', N'U') IS NULL
BEGIN
    CREATE TABLE ventas.PRODUCTO (
        producto_id INT IDENTITY(1,1) NOT NULL,
        sku VARCHAR(32) NOT NULL,
        nombre NVARCHAR(120) NOT NULL,
        precio_venta DECIMAL(12,2) NOT NULL,
        activo BIT NOT NULL CONSTRAINT DF_PRODUCTO_activo DEFAULT (1),
        CONSTRAINT PK_PRODUCTO PRIMARY KEY (producto_id),
        CONSTRAINT UQ_PRODUCTO_sku UNIQUE (sku),
        CONSTRAINT CK_PRODUCTO_precio CHECK (precio_venta >= 0.10)
    );
END
GO

IF OBJECT_ID(N'ventas.PEDIDO_ESTADO', N'U') IS NULL
BEGIN
    CREATE TABLE ventas.PEDIDO_ESTADO (
        pedido_estado_id TINYINT NOT NULL,
        codigo VARCHAR(16) NOT NULL,
        nombre NVARCHAR(60) NOT NULL,
        CONSTRAINT PK_PEDIDO_ESTADO PRIMARY KEY (pedido_estado_id),
        CONSTRAINT UQ_PEDIDO_ESTADO_codigo UNIQUE (codigo)
    );
END
GO

IF OBJECT_ID(N'ventas.PEDIDO', N'U') IS NULL
BEGIN
    CREATE TABLE ventas.PEDIDO (
        pedido_id INT IDENTITY(1,1) NOT NULL,
        cliente_id INT NOT NULL,
        vendedor_id INT NOT NULL,
        registrado_en DATETIME2 NOT NULL CONSTRAINT DF_PEDIDO_registrado_en DEFAULT (SYSUTCDATETIME()),
        moneda CHAR(3) NOT NULL CONSTRAINT DF_PEDIDO_moneda DEFAULT ('PEN'),
        total DECIMAL(12,2) NOT NULL,
        pedido_estado_id TINYINT NOT NULL,
        operacion_id UNIQUEIDENTIFIER NOT NULL, -- Clave de idempotencia contra doble clic
        CONSTRAINT PK_PEDIDO PRIMARY KEY (pedido_id),
        CONSTRAINT UQ_PEDIDO_operacion_id UNIQUE (operacion_id),
        CONSTRAINT FK_PEDIDO_CLIENTE FOREIGN KEY (cliente_id) REFERENCES ventas.CLIENTE(cliente_id),
        CONSTRAINT FK_PEDIDO_VENDEDOR FOREIGN KEY (vendedor_id) REFERENCES ventas.VENDEDOR(vendedor_id),
        CONSTRAINT FK_PEDIDO_ESTADO FOREIGN KEY (pedido_estado_id) REFERENCES ventas.PEDIDO_ESTADO(pedido_estado_id)
    );
END
GO

IF OBJECT_ID(N'ventas.PEDIDO_DETALLE', N'U') IS NULL
BEGIN
    CREATE TABLE ventas.PEDIDO_DETALLE (
        pedido_detalle_id INT IDENTITY(1,1) NOT NULL,
        pedido_id INT NOT NULL,
        producto_id INT NOT NULL,
        descripcion NVARCHAR(160) NOT NULL,
        cantidad DECIMAL(12,3) NOT NULL,
        precio_unitario DECIMAL(12,2) NOT NULL,
        CONSTRAINT PK_PEDIDO_DETALLE PRIMARY KEY (pedido_detalle_id),
        CONSTRAINT FK_PEDIDO_DETALLE_PEDIDO FOREIGN KEY (pedido_id) REFERENCES ventas.PEDIDO(pedido_id),
        CONSTRAINT FK_PEDIDO_DETALLE_PRODUCTO FOREIGN KEY (producto_id) REFERENCES ventas.PRODUCTO(producto_id),
        CONSTRAINT CK_PEDIDO_DETALLE_cantidad CHECK (cantidad > 0),
        CONSTRAINT CK_PEDIDO_DETALLE_precio CHECK (precio_unitario >= 0)
    );
END
GO

IF OBJECT_ID(N'ventas.PAGO_ESTADO', N'U') IS NULL
BEGIN
    CREATE TABLE ventas.PAGO_ESTADO (
        pago_estado_id TINYINT NOT NULL,
        codigo VARCHAR(16) NOT NULL,
        nombre NVARCHAR(60) NOT NULL,
        CONSTRAINT PK_PAGO_ESTADO PRIMARY KEY (pago_estado_id),
        CONSTRAINT UQ_PAGO_ESTADO_codigo UNIQUE (codigo)
    );
END
GO

IF OBJECT_ID(N'ventas.MEDIO_PAGO', N'U') IS NULL
BEGIN
    CREATE TABLE ventas.MEDIO_PAGO (
        medio_pago_id TINYINT NOT NULL,
        codigo VARCHAR(24) NOT NULL,
        nombre NVARCHAR(60) NOT NULL,
        activo BIT NOT NULL CONSTRAINT DF_MEDIO_PAGO_activo DEFAULT (1),
        CONSTRAINT PK_MEDIO_PAGO PRIMARY KEY (medio_pago_id),
        CONSTRAINT UQ_MEDIO_PAGO_codigo UNIQUE (codigo)
    );
END
GO

IF OBJECT_ID(N'ventas.PAGO', N'U') IS NULL
BEGIN
    CREATE TABLE ventas.PAGO (
        pago_id INT IDENTITY(1,1) NOT NULL,
        pedido_id INT NOT NULL,
        medio_pago_id TINYINT NOT NULL,
        monto DECIMAL(12,2) NOT NULL,
        registrado_en DATETIME2 NOT NULL CONSTRAINT DF_PAGO_registrado_en DEFAULT (SYSUTCDATETIME()),
        pago_estado_id TINYINT NOT NULL,
        referencia VARCHAR(100) NULL,
        CONSTRAINT PK_PAGO PRIMARY KEY (pago_id),
        CONSTRAINT FK_PAGO_PEDIDO FOREIGN KEY (pedido_id) REFERENCES ventas.PEDIDO(pedido_id),
        CONSTRAINT FK_PAGO_MEDIO FOREIGN KEY (medio_pago_id) REFERENCES ventas.MEDIO_PAGO(medio_pago_id),
        CONSTRAINT FK_PAGO_ESTADO FOREIGN KEY (pago_estado_id) REFERENCES ventas.PAGO_ESTADO(pago_estado_id),
        CONSTRAINT CK_PAGO_monto CHECK (monto > 0)
    );
END
GO

-- ESQUEMA FACTURACIÓN
IF OBJECT_ID(N'facturacion.COMPROBANTE_ESTADO', N'U') IS NULL
BEGIN
    CREATE TABLE facturacion.COMPROBANTE_ESTADO (
        comprobante_estado_id TINYINT NOT NULL,
        codigo VARCHAR(16) NOT NULL,
        nombre NVARCHAR(60) NOT NULL,
        CONSTRAINT PK_COMPROBANTE_ESTADO PRIMARY KEY (comprobante_estado_id),
        CONSTRAINT UQ_COMPROBANTE_ESTADO_codigo UNIQUE (codigo)
    );
END
GO

IF OBJECT_ID(N'facturacion.SERIE_COMPROBANTE', N'U') IS NULL
BEGIN
    CREATE TABLE facturacion.SERIE_COMPROBANTE (
        serie_comprobante_id INT IDENTITY(1,1) NOT NULL,
        tipo VARCHAR(8) NOT NULL, -- BOLETA, FACTURA
        serie CHAR(4) NOT NULL, -- B001, F001
        ultimo_correlativo INT NOT NULL CONSTRAINT DF_SERIE_ultimo_correlativo DEFAULT (0),
        activo BIT NOT NULL CONSTRAINT DF_SERIE_activo DEFAULT (1),
        CONSTRAINT PK_SERIE_COMPROBANTE PRIMARY KEY (serie_comprobante_id),
        CONSTRAINT UQ_SERIE_tipo_serie UNIQUE (tipo, serie)
    );
END
GO

IF OBJECT_ID(N'facturacion.COMPROBANTE', N'U') IS NULL
BEGIN
    CREATE TABLE facturacion.COMPROBANTE (
        comprobante_id INT IDENTITY(1,1) NOT NULL,
        pedido_id INT NOT NULL,
        serie_comprobante_id INT NOT NULL,
        correlativo INT NOT NULL,
        receptor_tipo_doc CHAR(1) NOT NULL,
        receptor_numero_doc VARCHAR(12) NOT NULL,
        receptor_nombre NVARCHAR(160) NOT NULL,
        emitido_en DATETIME2 NULL,
        comprobante_estado_id TINYINT NOT NULL,
        CONSTRAINT PK_COMPROBANTE PRIMARY KEY (comprobante_id),
        CONSTRAINT UQ_COMPROBANTE_pedido_id UNIQUE (pedido_id), -- Relación 1 a 1 con PEDIDO
        CONSTRAINT UQ_COMPROBANTE_serie_correlativo UNIQUE (serie_comprobante_id, correlativo),
        CONSTRAINT FK_COMPROBANTE_PEDIDO FOREIGN KEY (pedido_id) REFERENCES ventas.PEDIDO(pedido_id),
        CONSTRAINT FK_COMPROBANTE_SERIE FOREIGN KEY (serie_comprobante_id) REFERENCES facturacion.SERIE_COMPROBANTE(serie_comprobante_id),
        CONSTRAINT FK_COMPROBANTE_ESTADO FOREIGN KEY (comprobante_estado_id) REFERENCES facturacion.COMPROBANTE_ESTADO(comprobante_estado_id)
    );
END
GO

-- ESQUEMA DESPACHO
IF OBJECT_ID(N'despacho.TRANSPORTISTA', N'U') IS NULL
BEGIN
    CREATE TABLE despacho.TRANSPORTISTA (
        transportista_id INT IDENTITY(1,1) NOT NULL,
        ruc CHAR(11) NOT NULL,
        razon_social NVARCHAR(160) NOT NULL,
        activo BIT NOT NULL CONSTRAINT DF_TRANSPORTISTA_activo DEFAULT (1),
        CONSTRAINT PK_TRANSPORTISTA PRIMARY KEY (transportista_id),
        CONSTRAINT UQ_TRANSPORTISTA_ruc UNIQUE (ruc)
    );
END
GO

IF OBJECT_ID(N'despacho.ENVIO_ESTADO', N'U') IS NULL
BEGIN
    CREATE TABLE despacho.ENVIO_ESTADO (
        envio_estado_id TINYINT NOT NULL,
        codigo VARCHAR(16) NOT NULL,
        nombre NVARCHAR(60) NOT NULL,
        CONSTRAINT PK_ENVIO_ESTADO PRIMARY KEY (envio_estado_id),
        CONSTRAINT UQ_ENVIO_ESTADO_codigo UNIQUE (codigo)
    );
END
GO

IF OBJECT_ID(N'despacho.ENVIO', N'U') IS NULL
BEGIN
    CREATE TABLE despacho.ENVIO (
        envio_id INT IDENTITY(1,1) NOT NULL,
        pedido_id INT NOT NULL,
        punto_partida NVARCHAR(220) NOT NULL,
        punto_llegada NVARCHAR(220) NOT NULL,
        fecha_programada DATE NULL,
        modalidad VARCHAR(12) NOT NULL, -- PRIVADO, PUBLICO
        envio_estado_id TINYINT NOT NULL,
        transportista_id INT NULL,
        CONSTRAINT PK_ENVIO PRIMARY KEY (envio_id),
        CONSTRAINT UQ_ENVIO_pedido_id UNIQUE (pedido_id), -- Relación 1 a 1 con PEDIDO
        CONSTRAINT FK_ENVIO_PEDIDO FOREIGN KEY (pedido_id) REFERENCES ventas.PEDIDO(pedido_id),
        CONSTRAINT FK_ENVIO_ESTADO FOREIGN KEY (envio_estado_id) REFERENCES despacho.ENVIO_ESTADO(envio_estado_id),
        CONSTRAINT FK_ENVIO_TRANSPORTISTA FOREIGN KEY (transportista_id) REFERENCES despacho.TRANSPORTISTA(transportista_id)
    );
END
GO

IF OBJECT_ID(N'despacho.GUIA_REMISION', N'U') IS NULL
BEGIN
    CREATE TABLE despacho.GUIA_REMISION (
        guia_id INT IDENTITY(1,1) NOT NULL,
        envio_id INT NOT NULL,
        tipo VARCHAR(16) NOT NULL, -- REMITENTE, TRANSPORTISTA
        emisor_ruc CHAR(11) NOT NULL,
        serie CHAR(4) NOT NULL, -- T001
        correlativo INT NOT NULL,
        emitida_en DATETIME2 NOT NULL CONSTRAINT DF_GUIA_emitida_en DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_GUIA_REMISION PRIMARY KEY (guia_id),
        CONSTRAINT UQ_GUIA_serie_correlativo UNIQUE (emisor_ruc, tipo, serie, correlativo),
        CONSTRAINT FK_GUIA_ENVIO FOREIGN KEY (envio_id) REFERENCES despacho.ENVIO(envio_id)
    );
END
GO
