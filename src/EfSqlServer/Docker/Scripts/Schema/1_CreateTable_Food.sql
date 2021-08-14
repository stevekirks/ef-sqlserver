CREATE TABLE [dbo].[Food]
(
	[Id] INTEGER NOT NULL,
	[Name] NVARCHAR(max),
	[Timestamp] DATETIME2,
    CONSTRAINT PK_Food PRIMARY KEY (Id)
);

PRINT 'Created [dbo].[Food] table...'