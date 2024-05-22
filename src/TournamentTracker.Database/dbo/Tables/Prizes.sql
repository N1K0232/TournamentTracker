CREATE TABLE [dbo].[Prizes]
(
	[Id] UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
    [TournamentId] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    [Value] DECIMAL(8, 2) NOT NULL,
    [CreationDate] DATETIME NOT NULL DEFAULT getutcdate(),
    [LastModificationDate] DATETIME NULL,

    PRIMARY KEY([Id]),
    FOREIGN KEY([TournamentId]) REFERENCES [dbo].[Tournaments]([Id]),
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TournamentPrize]
ON [dbo].[Prizes]([TournamentId], [Name]);