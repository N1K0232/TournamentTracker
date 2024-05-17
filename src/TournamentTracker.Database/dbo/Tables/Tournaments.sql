CREATE TABLE [dbo].[Tournaments] (
    [Id]                   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Name]                 NVARCHAR (100)   NOT NULL,
    [EntryFee]             DECIMAL (8, 2)   NOT NULL,
    [StartDate]            DATE             NOT NULL,
    [CreationDate]         DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [LastModificationDate] DATETIME         NULL,
    [IsDeleted]            BIT              DEFAULT (0) NOT NULL,
    [DeletedDate]          DATETIME         NULL,

    PRIMARY KEY CLUSTERED ([Id] ASC)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tournament]
ON [dbo].[Tournaments]([Name], [EntryFee]);