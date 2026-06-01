IF DB_ID(N'OnibusDb') IS NULL
BEGIN
    CREATE DATABASE [OnibusDb];
END
GO

USE [OnibusDb];
GO

IF OBJECT_ID(N'[dbo].[Rotas]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Rotas]
    (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Origem] NVARCHAR(120) NOT NULL,
        [Destino] NVARCHAR(120) NOT NULL,
        [DuracaoEstimada] TIME NOT NULL,
        CONSTRAINT [PK_Rotas] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

IF OBJECT_ID(N'[dbo].[Viagens]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Viagens]
    (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [RotaId] INT NOT NULL,
        [DataHoraPartida] DATETIME2 NOT NULL,
        [Preco] DECIMAL(10,2) NOT NULL,
        [TotalAssentos] INT NOT NULL
            CONSTRAINT [DF_Viagens_TotalAssentos] DEFAULT (46),
        CONSTRAINT [PK_Viagens] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Viagens_Rotas_RotaId] FOREIGN KEY ([RotaId])
            REFERENCES [dbo].[Rotas] ([Id])
            ON DELETE NO ACTION
    );
END
GO

IF OBJECT_ID(N'[dbo].[Reservas]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Reservas]
    (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Codigo] NVARCHAR(12) NOT NULL,
        [Nome] NVARCHAR(160) NOT NULL,
        [Cpf] NVARCHAR(14) NOT NULL,
        [Email] NVARCHAR(180) NOT NULL,
        [ViagemId] INT NOT NULL,
        [AssentoNumero] INT NOT NULL,
        [CriadaEm] DATETIME2 NOT NULL,
        [CanceladaEm] DATETIME2 NULL,
        CONSTRAINT [PK_Reservas] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Reservas_Viagens_ViagemId] FOREIGN KEY ([ViagemId])
            REFERENCES [dbo].[Viagens] ([Id])
            ON DELETE NO ACTION
    );
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_Rotas_Origem_Destino'
      AND [object_id] = OBJECT_ID(N'[dbo].[Rotas]')
)
BEGIN
    CREATE INDEX [IX_Rotas_Origem_Destino]
        ON [dbo].[Rotas] ([Origem], [Destino]);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_Viagens_RotaId'
      AND [object_id] = OBJECT_ID(N'[dbo].[Viagens]')
)
BEGIN
    CREATE INDEX [IX_Viagens_RotaId]
        ON [dbo].[Viagens] ([RotaId]);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_Reservas_Codigo'
      AND [object_id] = OBJECT_ID(N'[dbo].[Reservas]')
)
BEGIN
    CREATE UNIQUE INDEX [IX_Reservas_Codigo]
        ON [dbo].[Reservas] ([Codigo]);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE [name] = N'IX_Reservas_ViagemId_AssentoNumero'
      AND [object_id] = OBJECT_ID(N'[dbo].[Reservas]')
)
BEGIN
    CREATE UNIQUE INDEX [IX_Reservas_ViagemId_AssentoNumero]
        ON [dbo].[Reservas] ([ViagemId], [AssentoNumero])
        WHERE [CanceladaEm] IS NULL;
END
GO
