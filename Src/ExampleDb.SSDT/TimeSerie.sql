CREATE TABLE [dbo].[TimeSerie]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[OutputId] int not null references [dbo].[Output](Id),
	[DateTime] datetime not null,
	[Value] decimal(18,2) null
)
