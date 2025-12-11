CREATE TABLE [dbo].[Tournaments]
(
	[Id]                UNIQUEIDENTIFIER    NOT NULL,
    [Name]              NVARCHAR (255)      NOT NULL,
    [EntryFee]          DECIMAL (6, 2)      NOT NULL,
    [StartsAt]          DATETIMEOFFSET (7)  NOT NULL,
    [EndsAt]            DATETIMEOFFSET (7)  NOT NULL,
    [CreatedAt]         DATETIME2 (7)       NOT NULL,
    [LastModifiedAt]    DATETIME2 (7)       NULL
);

GO
ALTER TABLE [dbo].[Tournaments]
ADD CONSTRAINT [PK_Tournaments] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[Tournaments]
ADD CONSTRAINT [DF_Tournaments_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[Tournaments]
ADD CONSTRAINT [DF_Tournaments_CreatedAt] DEFAULT (SYSUTCDATETIME()) FOR [CreatedAt];

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tournaments_Name]
ON [dbo].[Tournaments]([Name] ASC);