CREATE TABLE [dbo].[Images] (
    [Id]                   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [Path]                 NVARCHAR (512)   NOT NULL,
    [Length]               BIGINT           NOT NULL,
    [ContentType]          NVARCHAR (50)    NOT NULL,
    [CreationDate]         DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [LastModificationDate] DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_UniquePath]
    ON [dbo].[Images]([Path] ASC);

