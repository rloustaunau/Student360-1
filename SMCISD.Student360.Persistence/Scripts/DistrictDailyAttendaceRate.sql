CREATE TABLE [dbo].[DIM_Daily_Att_Rates_District](
	[LEVEL] [varchar](12) NULL,
	[MEMBERSHIPTOTAL] [nvarchar](4000) NULL,
	[ABS] [nvarchar](4000) NULL,
	[PRESENT] [nvarchar](4000) NULL,
	[YTDRATE] [nvarchar](4000) NULL,
	[INSTRUCTIONALDAY] [smallint] NULL,
	[CAL_DATE] [datetime] NULL,
	[SCHOOL_YEAR] [nchar](10) NULL
) ON [PRIMARY]
GO

create view [student360].[DistrictDailyAttendanceRate]
as
select MEMBERSHIPTOTAL Membership, PRESENT Present, CAL_DATE Date from dbo.DIM_Daily_Att_Rates_District
	where SCHOOL_YEAR = (select SchoolYear from edfi.SchoolYearType where CurrentSchoolYear = 1);
GO
