CREATE TABLE [dbo].[Report](
	[Id] [int] NOT NULL,
	[ReportName] [nvarchar](50) NULL,
	[ReportUri] [nvarchar](150) NULL,
	[LevelID] [int] NULL
) ON [PRIMARY]
GO


CREATE VIEW [student360].[Report]
AS
SELECT [Id]
      ,[ReportName]
      ,[ReportUri]
      ,[LevelID]
  FROM [dbo].[Report]
GO