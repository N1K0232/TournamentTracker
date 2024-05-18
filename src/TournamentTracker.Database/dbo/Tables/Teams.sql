CREATE TABLE [dbo].[Teams] (
    [Id]                   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [TournamentId]         UNIQUEIDENTIFIER NOT NULL,
    [Name]                 NVARCHAR (100)   NOT NULL,
    [CreationDate]         DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [LastModificationDate] DATETIME         NULL,

    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([TournamentId]) REFERENCES [dbo].[Tournaments] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TeamName]
    ON [dbo].[Teams]([TournamentId] ASC, [Name] ASC);