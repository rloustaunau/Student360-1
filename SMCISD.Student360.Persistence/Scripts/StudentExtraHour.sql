CREATE TABLE [student360].[Reasons](
	[Description] [nvarchar](max) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[ReasonId] [int] NOT NULL IDENTITY(1,1),
	[CreateDate] [datetime2](7) NOT NULL,
 CONSTRAINT [Reasons_PK] PRIMARY KEY CLUSTERED 
(
	[ReasonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


ALTER TABLE [student360].[Reasons] ADD CONSTRAINT [Reasons_DF_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO


insert into student360.Reasons ([Description], [Value]) 
values ('After School Detention', 'After School Detention'),
('Attendance Officer', 'Attendance Officer'),
('Discipline DAEP', 'Discipline DAEP'),
('Other', 'Other'),
('Other Attendance', 'Other Attendance'),
('Phone Call With Parents', 'Phone Call With Parents'),
('Saturday School', 'Saturday School'),
('Student Counseling', 'Student Counseling');

CREATE TABLE [student360].[StudentExtraHours](
	[StudentUniqueId] [nvarchar](32) NOT NULL,
	[GradeLevel] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](75) NOT NULL,
	[LastSurname] [nvarchar](75) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Hours] [int] NOT NULL,
	[SchoolYear] [smallint] NULL,
	[UserCreatedUniqueId] [nvarchar](50) NOT NULL,
	[UserRole] [nvarchar](100) NOT NULL,
	[Comments] [nvarchar](100) NOT NULL,
	[ReasonId] [int] NOT NULL,
	[UserFirstName] [nvarchar](75) NOT NULL,
	[UserLastSurname] [nvarchar](75) NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [StudentExtraHours_PK] PRIMARY KEY CLUSTERED 
(
	[StudentUniqueId] ASC,
	[Date] ASC,
	[ReasonId] ASC,
	[UserCreatedUniqueId] ASC,
	[UserRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [student360].[StudentExtraHours] ADD  CONSTRAINT [StudentExtraHours_DF_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO

ALTER TABLE [student360].[StudentExtraHours] ADD  CONSTRAINT [StudentExtraHours_DF_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [student360].[StudentExtraHours]  WITH CHECK ADD  CONSTRAINT [FK_StudentExtraHours_Reason] FOREIGN KEY([ReasonId])
REFERENCES [student360].[Reasons] ([ReasonId])
GO

ALTER TABLE [student360].[StudentExtraHours] CHECK CONSTRAINT [FK_StudentExtraHours_Reason]
GO

Create View [student360].[StudentExtraHourGrid]
as
select seh.StudentUniqueId, seh.GradeLevel, seh.FirstName, seh.LastSurname, seh.SchoolYear, s.StudentUSI, sch.SchoolId, sch.LocalEducationAgencyId,
(select SUM(scg.Hours) from student360.StudentExtraHourCurrentGrid scg where scg.StudentUniqueId = seh.StudentUniqueId and seh.SchoolYear = scg.SchoolYear) TotalHours
 from student360.StudentExtraHours seh
	inner join edfi.Student s on s.StudentUniqueId = seh.StudentUniqueId
	inner join edfi.StudentSchoolAssociation ssa on ssa.StudentUSI = s.StudentUSI
	inner join edfi.School sch on sch.SchoolId = ssa.SchoolId
	inner join edfi.SchoolYearType syt on syt.SchoolYear = ssa.SchoolYear
where syt.CurrentSchoolYear = 1
group by seh.StudentUniqueId, seh.GradeLevel, seh.FirstName, seh.LastSurname, seh.SchoolYear, s.StudentUSI, sch.SchoolId, sch.LocalEducationAgencyId
GO

Create View [student360].[StudentExtraHourCurrentGrid]
as
select seh.StudentUniqueId, seh.GradeLevel, seh.FirstName, seh.LastSurname,CONVERT(nvarchar,CAST(seh.Date as Date)) Date, seh.SchoolYear, seh.Hours, rs.Value Reason,seh.ReasonId,  seh.Comments, seh.UserCreatedUniqueId, seh.UserRole, seh.UserFirstName, seh.UserLastSurname, seh.CreateDate, seh.Id, s.StudentUSI, sch.SchoolId, sch.LocalEducationAgencyId 
from student360.StudentExtraHours seh
	inner join edfi.Student s on s.StudentUniqueId = seh.StudentUniqueId
	inner join edfi.StudentSchoolAssociation ssa on ssa.StudentUSI = s.StudentUSI
	inner join edfi.School sch on sch.SchoolId = ssa.SchoolId
	inner join edfi.SchoolYearType syt on syt.SchoolYear = ssa.SchoolYear
	inner join student360.Reasons rs on rs.ReasonId = seh.ReasonId
where syt.CurrentSchoolYear = 1 
GO

