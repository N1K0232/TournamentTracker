CREATE TABLE [dbo].[Teams]
(
	[Id]                UNIQUEIDENTIFIER       NOT NULL,
    [TournamentId]      UNIQUEIDENTIFIER       NOT NULL,
    [Name]              NVARCHAR (255)         NOT NULL,
    [CreatedAt]         DATETIME2 (7)          NOT NULL,
    [LastModifiedAt]    DATETIME2 (7)          NULL,
);

GO
ALTER TABLE [dbo].[Teams]
ADD CONSTRAINT [PK_Teams] PRIMARY KEY([Id] ASC);

GO
ALTER TABLE [dbo].[Teams]
ADD CONSTRAINT [FK_Teams_Tournaments] FOREIGN KEY([TournamentId]) REFERENCES [dbo].[Tournaments]([Id])
ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[Teams]
ADD CONSTRAINT [DF_Teams_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[Teams]
ADD CONSTRAINT [DF_Teams_CreatedAt] DEFAULT (SYSUTCDATETIME()) FOR [CreatedAt];

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Teams_Name]
ON [dbo].[Teams]([Name] ASC);