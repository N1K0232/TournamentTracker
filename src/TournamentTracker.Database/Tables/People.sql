CREATE TABLE [dbo].[People]
(
	[Id]                UNIQUEIDENTIFIER    NOT NULL,
    [TeamId]            UNIQUEIDENTIFIER    NOT NULL,
    [FirstName]         NVARCHAR (255)      NOT NULL,
    [LastName]          NVARCHAR (255)      NOT NULL,
    [BirthDate]         DATE                NOT NULL,
    [City]              NVARCHAR (100)      NOT NULL,
    [CellphoneNumber]   NVARCHAR (100)      NOT NULL,
    [EmailAddress]      NVARCHAR (255)      NOT NULL,
    [CreatedAt]         DATETIME2 (7)       NOT NULL,
    [LastModifiedAt]    DATETIME2 (7)       NULL
);

GO
ALTER TABLE [dbo].[People]
ADD CONSTRAINT [PK_People] PRIMARY KEY ([Id] ASC);

GO
ALTER TABLE [dbo].[People]
ADD CONSTRAINT [FK_People_Teams] FOREIGN KEY([TeamId]) REFERENCES [dbo].[Teams]([Id])
ON DELETE CASCADE;

GO
ALTER TABLE [dbo].[People]
ADD CONSTRAINT [DF_People_Id] DEFAULT (NEWSEQUENTIALID()) FOR [Id];

GO
ALTER TABLE [dbo].[People]
ADD CONSTRAINT [DF_People_CreatedAt] DEFAULT (SYSUTCDATETIME()) FOR [CreatedAt];

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_People_CellphoneNumber]
ON [dbo].[People]([CellphoneNumber] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_People_EmailAddress]
ON [dbo].[People]([EmailAddress] ASC);