CREATE TABLE [dbo].[Tournaments] (
    [Id]                   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Name]                 NVARCHAR (100)   NOT NULL,
    [EntryFee]             DECIMAL (8, 2)   NOT NULL,
    [StartDate]            DATE             NOT NULL,
    [CreationDate]         DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [LastModificationDate] DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

