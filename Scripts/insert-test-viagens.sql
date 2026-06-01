USE [OnibusDb];
GO

DECLARE @SaoPauloRioId INT;
DECLARE @SaoPauloCuritibaId INT;
DECLARE @CampinasBeloHorizonteId INT;
DECLARE @RioVitoriaId INT;

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Rotas]
    WHERE [Origem] = N'Sao Paulo'
      AND [Destino] = N'Rio de Janeiro'
)
BEGIN
    INSERT INTO [dbo].[Rotas] ([Origem], [Destino], [DuracaoEstimada])
    VALUES (N'Sao Paulo', N'Rio de Janeiro', '06:00:00');
END

SELECT @SaoPauloRioId = [Id]
FROM [dbo].[Rotas]
WHERE [Origem] = N'Sao Paulo'
  AND [Destino] = N'Rio de Janeiro';

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Rotas]
    WHERE [Origem] = N'Sao Paulo'
      AND [Destino] = N'Curitiba'
)
BEGIN
    INSERT INTO [dbo].[Rotas] ([Origem], [Destino], [DuracaoEstimada])
    VALUES (N'Sao Paulo', N'Curitiba', '06:30:00');
END

SELECT @SaoPauloCuritibaId = [Id]
FROM [dbo].[Rotas]
WHERE [Origem] = N'Sao Paulo'
  AND [Destino] = N'Curitiba';

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Rotas]
    WHERE [Origem] = N'Campinas'
      AND [Destino] = N'Belo Horizonte'
)
BEGIN
    INSERT INTO [dbo].[Rotas] ([Origem], [Destino], [DuracaoEstimada])
    VALUES (N'Campinas', N'Belo Horizonte', '08:00:00');
END

SELECT @CampinasBeloHorizonteId = [Id]
FROM [dbo].[Rotas]
WHERE [Origem] = N'Campinas'
  AND [Destino] = N'Belo Horizonte';

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Rotas]
    WHERE [Origem] = N'Rio de Janeiro'
      AND [Destino] = N'Vitoria'
)
BEGIN
    INSERT INTO [dbo].[Rotas] ([Origem], [Destino], [DuracaoEstimada])
    VALUES (N'Rio de Janeiro', N'Vitoria', '07:00:00');
END

SELECT @RioVitoriaId = [Id]
FROM [dbo].[Rotas]
WHERE [Origem] = N'Rio de Janeiro'
  AND [Destino] = N'Vitoria';

DECLARE @Amanha DATE = DATEADD(DAY, 1, CONVERT(DATE, GETDATE()));
DECLARE @DepoisDeAmanha DATE = DATEADD(DAY, 2, CONVERT(DATE, GETDATE()));
DECLARE @TresDias DATE = DATEADD(DAY, 3, CONVERT(DATE, GETDATE()));

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Viagens]
    WHERE [RotaId] = @SaoPauloRioId
      AND [DataHoraPartida] = DATEADD(HOUR, 8, CAST(@Amanha AS DATETIME2))
)
BEGIN
    INSERT INTO [dbo].[Viagens] ([RotaId], [DataHoraPartida], [Preco], [TotalAssentos])
    VALUES (@SaoPauloRioId, DATEADD(HOUR, 8, CAST(@Amanha AS DATETIME2)), 129.90, 46);
END

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Viagens]
    WHERE [RotaId] = @SaoPauloRioId
      AND [DataHoraPartida] = DATEADD(HOUR, 22, CAST(@Amanha AS DATETIME2))
)
BEGIN
    INSERT INTO [dbo].[Viagens] ([RotaId], [DataHoraPartida], [Preco], [TotalAssentos])
    VALUES (@SaoPauloRioId, DATEADD(HOUR, 22, CAST(@Amanha AS DATETIME2)), 149.90, 46);
END

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Viagens]
    WHERE [RotaId] = @SaoPauloCuritibaId
      AND [DataHoraPartida] = DATEADD(HOUR, 9, CAST(@DepoisDeAmanha AS DATETIME2))
)
BEGIN
    INSERT INTO [dbo].[Viagens] ([RotaId], [DataHoraPartida], [Preco], [TotalAssentos])
    VALUES (@SaoPauloCuritibaId, DATEADD(HOUR, 9, CAST(@DepoisDeAmanha AS DATETIME2)), 119.90, 42);
END

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Viagens]
    WHERE [RotaId] = @CampinasBeloHorizonteId
      AND [DataHoraPartida] = DATEADD(HOUR, 7, CAST(@DepoisDeAmanha AS DATETIME2))
)
BEGIN
    INSERT INTO [dbo].[Viagens] ([RotaId], [DataHoraPartida], [Preco], [TotalAssentos])
    VALUES (@CampinasBeloHorizonteId, DATEADD(HOUR, 7, CAST(@DepoisDeAmanha AS DATETIME2)), 179.90, 44);
END

IF NOT EXISTS (
    SELECT 1 FROM [dbo].[Viagens]
    WHERE [RotaId] = @RioVitoriaId
      AND [DataHoraPartida] = DATEADD(HOUR, 13, CAST(@TresDias AS DATETIME2))
)
BEGIN
    INSERT INTO [dbo].[Viagens] ([RotaId], [DataHoraPartida], [Preco], [TotalAssentos])
    VALUES (@RioVitoriaId, DATEADD(HOUR, 13, CAST(@TresDias AS DATETIME2)), 159.90, 40);
END

SELECT
    v.[Id],
    r.[Origem],
    r.[Destino],
    v.[DataHoraPartida],
    v.[Preco],
    v.[TotalAssentos]
FROM [dbo].[Viagens] v
INNER JOIN [dbo].[Rotas] r ON r.[Id] = v.[RotaId]
ORDER BY v.[DataHoraPartida];
GO
