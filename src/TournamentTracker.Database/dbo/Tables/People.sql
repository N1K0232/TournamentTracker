CREATE TABLE [dbo].[People] (
    [Id]                   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [TeamId]               UNIQUEIDENTIFIER NOT NULL,
    [FirstName]            NVARCHAR (256)   NOT NULL,
    [LastName]             NVARCHAR (256)   NOT NULL,
    [BirthDate]            DATE             NOT NULL,
    [CellphoneNumber]      VARCHAR (50)     NOT NULL,
    [EmailAddress]         VARCHAR (512)    NOT NULL,
    [CreationDate]         DATETIME         DEFAULT (getutcdate()) NOT NULL,
    [LastModificationDate] DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_CellphoneNumber]
    ON [dbo].[People]([CellphoneNumber] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EmailAddress]
    ON [dbo].[People]([EmailAddress] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_TeamMember]
    ON [dbo].[People]([TeamId] ASC, [FirstName] ASC, [LastName] ASC);

