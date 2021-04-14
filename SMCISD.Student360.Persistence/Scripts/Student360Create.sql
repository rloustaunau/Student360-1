Create view [student360].[Grade]
AS
select CodeValue from edfi.Descriptor where Namespace like '%uri://www.smcisd.net/GradeLevelDescriptor';
GO

Create view [student360].[Cohort]
AS
Select Distinct(GraduationSchoolYear) from edfi.StudentSchoolAssociation where GraduationSchoolYear is not null;
GO

CREATE VIEW [auth].[LocalEducationAgencyIdToStaffUSI]
AS
-- LEA to Staff (through employment)
Select LocalEducationAgencyId,StaffUSI StaffUsi FROM (
		select	emp.EducationOrganizationId as LocalEducationAgencyId, emp.StaffUSI
		from	edfi.LocalEducationAgency lea
				inner join auth.EducationOrganizationToStaffUSI_Employment emp
					on lea.LocalEducationAgencyId = emp.EducationOrganizationId
		UNION ALL
		-- LEA to Staff (through assignment)
		select	assgn.EducationOrganizationId as LocalEducationAgencyId, assgn.StaffUSI
		from	edfi.LocalEducationAgency lea
				inner join auth.EducationOrganizationToStaffUSI_Assignment assgn
					on lea.LocalEducationAgencyId = assgn.EducationOrganizationId
		UNION ALL
		-- School to Staff (through employment)
		select	sch.LocalEducationAgencyId, emp.StaffUSI
		from	edfi.School sch
				inner join auth.EducationOrganizationToStaffUSI_Employment emp
					on sch.SchoolId = emp.EducationOrganizationId
		UNION ALL
		-- School to Staff (through assignment)
		select	sch.LocalEducationAgencyId, assgn.StaffUSI
		from	edfi.School sch
				inner join auth.EducationOrganizationToStaffUSI_Assignment assgn
					on sch.SchoolId = assgn.EducationOrganizationId
) s
group by s.StaffUSI, s.LocalEducationAgencyId
--
GO

CREATE VIEW [student360].[Schools]
AS
SELECT s.SchoolId, eo.NameOfInstitution, d.CodeValue GradeLevel, s.LocalEducationAgencyId from edfi.School as s
INNER JOIN edfi.EducationOrganization as eo
	on s.SchoolId = eo.EducationOrganizationId
INNER JOIN edfi.SchoolGradeLevel as sgl
	on sgl.SchoolId = s.SchoolId
INNER JOIN edfi.Descriptor d on d.DescriptorId = sgl.GradeLevelDescriptorId
GO

CREATE VIEW [student360].[Semesters]
AS
SELECT Distinct(SessionName) FROM edfi.Session;
GO

CREATE VIEW [student360].[SchoolYears]
AS
SELECT TOP 100 SchoolYear from edfi.SchoolYearType where SchoolYear <= 
(SELECT SchoolYear from edfi.SchoolYearType WHERE CurrentSchoolYear = 1)
order by SchoolYear desc
GO

CREATE VIEW [auth].[SchoolIdToStaffUSI]
AS
-- School to Staff (through employment)
Select SchoolId, StaffUSI StaffUsi FROM (
	select	sch.SchoolId, seo_empl.StaffUSI, HireDate as BeginDate, EndDate
	from	edfi.School sch
			inner join edfi.StaffEducationOrganizationEmploymentAssociation seo_empl
				on sch.SchoolId = seo_empl.EducationOrganizationId
	UNION ALL
	-- School to Staff (through assignment)
	select	sch.SchoolId, seo_assgn.StaffUSI, BeginDate, EndDate
	from	edfi.School sch
			inner join edfi.StaffEducationOrganizationAssignmentAssociation seo_assgn
				on sch.SchoolId = seo_assgn.EducationOrganizationId 
) s
Group by s.SchoolId, s.StaffUSI -- Removing duplicates, causes data duplication on security filtering
 
--
GO

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

create view [student360].[ADACampus]
as
(select c.NameOfInstitution, (c.MaxStudentAttendance - c.CampusAbsences) StudentAttendance,
	   c.MaxStudentAttendance, 
	   ((c.MaxStudentAttendance - c.CampusAbsences) / CONVERT(decimal(12,2), c.MaxStudentAttendance)) * 100 AttendancePercent
 from (select eo.NameOfInstitution,
	    Count(ssae.EventDate) CampusAbsences,
		(select Count(Distinct(cdce.Date))  from  edfi.CalendarDateCalendarEvent as cdce
			inner join edfi.Descriptor as d on cdce.CalendarEventDescriptorId = d.DescriptorId
			where ssa.SchoolYear = cdce.SchoolYear and ssa.SchoolId = cdce.SchoolId
			and d.Description = 'Membership Day') * 
			(select count(*) from edfi.StudentSchoolAssociation issa where issa.SchoolId = ssa.SchoolId
			and issa.SchoolYear = ssa.SchoolYear) MaxStudentAttendance
from edfi.StudentSchoolAssociation ssa
INNER JOIN edfi.EducationOrganization eo
	on ssa.SchoolId = eo.EducationOrganizationId
INNER JOIN edfi.StudentSectionAttendanceEvent ssae 
	on ssa.StudentUSI = ssae.StudentUSI 
	and ssa.SchoolId = ssae.SchoolId 
	and ssa.SchoolYear = ssae.SchoolYear
INNER JOIN edfi.Descriptor aed 
	ON ssae.AttendanceEventCategoryDescriptorId = aed.DescriptorId
	and aed.CodeValue in ('E','U','DR', 'A', 'UP', 'S','W','X','M')
where ssa.EntryDate = (select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = ssa.StudentUSI)
and ssae.EventDate >= (select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = ssa.StudentUSI)
group by eo.NameOfInstitution, ssa.SchoolId, ssa.SchoolYear
) c)
GO

create view [student360].[ADADistrict]
as
(select (c.MaxStudentAttendance - c.CampusAbsences) StudentAttendance,
	   c.MaxStudentAttendance, 
	   ((c.MaxStudentAttendance - c.CampusAbsences) / CONVERT(decimal(12,2), c.MaxStudentAttendance)) * 100 AttendancePercent
 from (select Count(ssae.EventDate) CampusAbsences,
		(select Count(Distinct(cdce.Date))  from  edfi.CalendarDateCalendarEvent as cdce
			inner join edfi.Descriptor as d on cdce.CalendarEventDescriptorId = d.DescriptorId
			where ssa.SchoolYear = cdce.SchoolYear 
			and d.Description = 'Membership Day') * 
			(select count(*) from edfi.StudentSchoolAssociation issa where issa.SchoolYear = ssa.SchoolYear) MaxStudentAttendance
from edfi.StudentSchoolAssociation ssa
INNER JOIN edfi.EducationOrganization eo
	on ssa.SchoolId = eo.EducationOrganizationId
INNER JOIN edfi.StudentSectionAttendanceEvent ssae 
	on ssa.StudentUSI = ssae.StudentUSI 
	and ssa.SchoolId = ssae.SchoolId 
	and ssa.SchoolYear = ssae.SchoolYear
INNER JOIN edfi.Descriptor aed 
	ON ssae.AttendanceEventCategoryDescriptorId = aed.DescriptorId
	and aed.CodeValue in ('E','U','DR', 'A', 'UP', 'S','W','X','M')
where ssa.EntryDate = (select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = ssa.StudentUSI)
and ssae.EventDate >= (select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = ssa.StudentUSI)
group by ssa.SchoolYear
) c)

GO

CREATE TABLE [dbo].[StaffLevels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LevelDescriptor] [varchar](50) NULL,
	[StaffClassificationDescriptor] [varchar](50) NULL,
	[LevelId] [int] NULL,
	[StaffClassificationDescriptorId] [varchar](50) NULL,
 CONSTRAINT [PK_auth.StaffLevels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE VIEW [student360].[People] AS
(SELECT s.StaffUSI USI, StaffUniqueId UniqueId, FirstName, LastSurname, ElectronicMailAddress, 'Staff' PersonType, d.CodeValue PositionTitle , sch.SchoolId, lea.LocalEducationAgencyId, sl.LevelDescriptor AccessLevel, sl.LevelId
FROM edfi.Staff s
INNER JOIN edfi.StaffElectronicMail se on s.StaffUSI = se.StaffUSI
INNER JOIN edfi.StaffEducationOrganizationAssignmentAssociation seoaa on s.StaffUSI = seoaa.StaffUSI
INNER JOIN edfi.Descriptor as d on seoaa.StaffClassificationDescriptorId = d.DescriptorId
INNER JOIN dbo.StaffLevels as sl on seoaa.StaffClassificationDescriptorId = sl.StaffClassificationDescriptorId
LEFT JOIN edfi.LocalEducationAgency as lea on lea.LocalEducationAgencyId = seoaa.EducationOrganizationId
LEFT JOIN edfi.School as sch on sch.SchoolId = seoaa.EducationOrganizationId
)
--UNION
--(SELECT p.ParentUSI USI, ParentUniqueId UniqueId, FirstName, LastSurname, ElectronicMailAddress, 'Parent' PersonType, rd.CodeValue PositionTitle, ssa.SchoolId, null LocalEducationAgencyId, 'Student' AccessLevel, 4 LevelId
--FROM edfi.Parent p
--inner join edfi.ParentElectronicMail pe on p.ParentUSI = pe.ParentUSI
--inner join edfi.StudentParentAssociation spa on p.ParentUSI = spa.ParentUSI
--inner join edfi.Descriptor rd on spa.RelationDescriptorId = rd.DescriptorId
--inner join edfi.StudentSchoolAssociation  as ssa on spa.StudentUSI = ssa.StudentUSI)
--UNION
--(SELECT s.StudentUSI USI, StudentUniqueId UniqueId, FirstName, LastSurname, ElectronicMailAddress, 'Student' PersonType, 'Student' PositionTitle, ssa.SchoolId, null LocalEducationAgencyId, 'Student' AccessLevel, 4 LevelId
--FROM edfi.Student s
--INNER JOIN edfi.StudentSchoolAssociation as ssa on s.StudentUSI = ssa.StudentUSI
--INNER JOIN edfi.StudentEducationOrganizationAssociation seoa on s.StudentUSI = seoa.StudentUSI and ssa.SchoolId = seoa.EducationOrganizationId
--INNER JOIN edfi.StudentEducationOrganizationAssociationElectronicMail se on s.StudentUSI = se.StudentUSI and seoa.EducationOrganizationId = se.EducationOrganizationId)
GO



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


create view [student360].[StudentAbsencesByPeriod]
as
select Count(ssae.EventDate) Quantity, ssae.StudentUSI, scp.ClassPeriodName
from edfi.StudentSectionAttendanceEvent ssae
inner join edfi.Descriptor d on ssae.AttendanceEventCategoryDescriptorId = d.DescriptorId
inner join edfi.SectionClassPeriod scp
	on scp.SchoolId = ssae.SchoolId
	and scp.SchoolYear = ssae.SchoolYear
	and scp.SectionIdentifier = ssae.SectionIdentifier
	and scp.SessionName = ssae.SessionName
	and scp.LocalCourseCode = ssae.LocalCourseCode
inner join edfi.SchoolYearType syt
	on ssae.SchoolYear = syt.SchoolYear
where syt.CurrentSchoolYear = 1
and d.CodeValue in ('E','U','DR', 'A', 'UP', 'S','W','X','M')
group by ssae.StudentUSI, scp.ClassPeriodName
GO


create view [student360].[StudentAbsencesByCourse]
as
select sched.StudentUSI, sched.SchoolId,  sched.LocalCourseCode,  sched.SectionIdentifier, sched.AvailableCredits Credits, 
  sched.CourseCode, sched.LocalCourseTitle, sched.SchoolYear,  GradeLevel, LocalEducationAgencyId,
  StudentUniqueId, StudentFirstName, StudentLastSurname, GraduationSchoolYear, NameOfInstitution,
  SessionName, TeacherLastSurname, Room = LocationClassroomIdentificationCode, ClassPeriodName,  
  	   'MARK9W1' = (select top 1 LetterGradeEarned from edfi.Grade gr
						Inner join edfi.Descriptor d on gr.GradeTypeDescriptorId = d.DescriptorId
						where gr.StudentUSI = sched.StudentUSI 
						and gr.LocalCourseCode = sched.LocalCourseCode 
						and d.CodeValue = 'Grading Period'
						and gr.GradingPeriodSequence = 1),
	   'MARK9W2' = (select top 1 LetterGradeEarned from edfi.Grade gr
						Inner join edfi.Descriptor d on gr.GradeTypeDescriptorId = d.DescriptorId
						where gr.StudentUSI = sched.StudentUSI 
						and gr.LocalCourseCode = sched.LocalCourseCode 
						and d.CodeValue = 'Grading Period'
						and gr.GradingPeriodSequence = 2),
	   'MARK9W3' = (select top 1 LetterGradeEarned from edfi.Grade gr
						Inner join edfi.Descriptor d on gr.GradeTypeDescriptorId = d.DescriptorId
						where gr.StudentUSI = sched.StudentUSI 
						and gr.LocalCourseCode = sched.LocalCourseCode 
						and d.CodeValue = 'Grading Period'
						and gr.GradingPeriodSequence = 3),
	   'MARK9W4' = (select top 1 LetterGradeEarned from edfi.Grade gr
						Inner join edfi.Descriptor d on gr.GradeTypeDescriptorId = d.DescriptorId
						where gr.StudentUSI = sched.StudentUSI 
						and gr.LocalCourseCode = sched.LocalCourseCode 
						and d.CodeValue = 'Grading Period'
						and gr.GradingPeriodSequence = 4),
		'FS1' = (select top 1 LetterGradeEarned from edfi.Grade gr 
				inner join edfi.Descriptor d
				on d.DescriptorId = gr.GradeTypeDescriptorId 
				and d.CodeValue = 'Final'
				and gr.SessionName = 'Fall Semester'
			where gr.StudentUSI = sched.StudentUSI 
				and gr.LocalCourseCode = sched.LocalCourseCode 
				and sched.SectionIdentifier = gr.SectionIdentifier ),
		'FS2' = (select top 1 LetterGradeEarned from edfi.Grade gr 
				inner join edfi.Descriptor d
				on d.DescriptorId = gr.GradeTypeDescriptorId 
				and d.CodeValue = 'Final'
				and gr.SessionName = 'Spring Semester'
			where gr.StudentUSI = sched.StudentUSI 
				and gr.LocalCourseCode = sched.LocalCourseCode 
				and sched.SectionIdentifier = gr.SectionIdentifier ),
		'YFinal' = (select top 1 LetterGradeEarned from edfi.Grade gr 
				inner join edfi.Descriptor d
				on d.DescriptorId = gr.GradeTypeDescriptorId 
				and d.CodeValue = 'Final'
				and gr.SessionName = 'Year round'
			where gr.StudentUSI = sched.StudentUSI 
				and gr.LocalCourseCode = sched.LocalCourseCode 
				and sched.SectionIdentifier = gr.SectionIdentifier),
	    'S1Abs' = (Select Count(EventDate) from edfi.StudentSectionAttendanceEvent  ssatt
					INNER JOIN edfi.Descriptor ON ssatt.AttendanceEventCategoryDescriptorId = DescriptorId
					INNER JOIN edfi.CourseOffering ico 
						on ico.LocalCourseCode = ssatt.LocalCourseCode 
						and ico.SchoolId = ssatt.SchoolId	 
						and ico.SchoolYear = ssatt.SchoolYear
						and ico.SessionName = ssatt.SessionName
					where StudentUSI = sched.StudentUSI 
						and ico.CourseCode = sched.CourseCode  
						and ssatt.SchoolId = sched.SchoolId 
						and ssatt.SchoolYear = sched.SchoolYear 
						and ssatt.SectionIdentifier = sched.SectionIdentifier
						and ssatt.SessionName = 'Fall Semester'
						and ssatt.StudentUSI = sched.StudentUSI
						and CodeValue in ('X','A','E','S','U', 'UP','PE','PS','PU','RE','RS','RU','W','M','FN')),
		'S2Abs' = (Select Count(EventDate) from edfi.StudentSectionAttendanceEvent  ssatt
					INNER JOIN edfi.Descriptor ON ssatt.AttendanceEventCategoryDescriptorId = DescriptorId
					inner join edfi.CourseOffering ico 
						on ico.LocalCourseCode = ssatt.LocalCourseCode 
						and ico.SchoolId = ssatt.SchoolId	 
						and ico.SchoolYear = ssatt.SchoolYear
						and ico.SessionName = ssatt.SessionName
					where StudentUSI = sched.StudentUSI 
						and ico.CourseCode  = sched.CourseCode 
						and ssatt.SchoolId = sched.SchoolId 
						and ssatt.SchoolYear = sched.SchoolYear 
						and ssatt.SectionIdentifier = sched.SectionIdentifier
						and ssatt.SessionName = 'Spring Semester'
						and ssatt.StudentUSI = sched.StudentUSI
						and CodeValue in ('X','A','E','S','U', 'UP','PE','PS','PU','RE','RS','RU','W','M','FN')),
		'AbsencesCount' = (Select Count(EventDate) from edfi.StudentSectionAttendanceEvent  ssatt
					INNER JOIN edfi.Descriptor ON ssatt.AttendanceEventCategoryDescriptorId = DescriptorId
					inner join edfi.CourseOffering ico 
						on ico.LocalCourseCode = ssatt.LocalCourseCode 
						and ico.SchoolId = ssatt.SchoolId	 
						and ico.SchoolYear = ssatt.SchoolYear
						and ico.SessionName = ssatt.SessionName
					where StudentUSI = sched.StudentUSI 
						and ico.CourseCode  = sched.CourseCode 
						and ssatt.SchoolId = sched.SchoolId 
						and ssatt.SchoolYear = sched.SchoolYear 
						and ssatt.SectionIdentifier = sched.SectionIdentifier
						and ssatt.StudentUSI = sched.StudentUSI
						and CodeValue in ('X','A','E','S','U', 'UP','PE','PS','PU','RE','RS','RU','W','M','FN'))
from (
		 select ssa.SchoolId,  ssa.LocalCourseCode, ssa.StudentUSI
		 , s.StudentUniqueId, s.FirstName StudentFirstName
		 ,s.LastSurname StudentLastSurname, ssca.GraduationSchoolYear
		 ,edorg.NameOfInstitution, sch.LocalEducationAgencyId
		 ,ssa.SectionIdentifier, sc.AvailableCredits, 
		  co.CourseCode, co.LocalCourseTitle, ssa.SchoolYear  , ssca.EntryGradeLevelDescriptorId        
		  , SessionName = (case when ssa.SchoolId = 1 then ssa.SessionName else '' end)
		  , LocationClassroomIdentificationCode, staff.LastSurname TeacherLastSurname
		  ,GradeLevel = gldd.CodeValue, scp.ClassPeriodName
		  from edfi.StudentSectionAssociation ssa
		  inner join edfi.Student s on ssa.StudentUSI = s.StudentUSI
		   inner join edfi.StudentSchoolAssociation ssca 
		   on ssca.StudentUSI = ssa.StudentUSI 
		   inner join edfi.Section sc 
		     on sc.SectionIdentifier = ssa.SectionIdentifier 
			 and sc.LocalCourseCode = ssa.LocalCourseCode 
			 and sc.SchoolId = ssa.SchoolId
			 and sc.SchoolYear = ssa.SchoolYear
		   inner join edfi.CourseOffering co 
				on co.LocalCourseCode = ssa.LocalCourseCode 
				and co.SchoolId = ssa.SchoolId	 
				and co.SchoolYear = ssa.SchoolYear
				and co.SessionName = ssa.SessionName
		   inner join edfi.StaffSectionAssociation staffSec 
		       ON ssa.LocalCourseCode = staffSec.LocalCourseCode 
			   and ssa.SchoolId = staffSec.SchoolId 
			   and ssa.SchoolYear = staffSec.SchoolYear 
			   and ssa.SectionIdentifier = staffSec.SectionIdentifier 
			   and ssa.SessionName = staffSec.SessionName
		   inner join edfi.Staff staff ON staffSec.StaffUSI = staff.StaffUSI
		   inner join EDFI.Descriptor posDesc 
				ON staffSec.ClassroomPositionDescriptorId = posDesc.DescriptorId 
				and posDesc.CodeValue = 'Teacher of Record'
		   INNER JOIN edfi.EducationOrganization edorg ON sc.SchoolId = edorg.EducationOrganizationId
		   inner join edfi.Descriptor gldd on ssca.EntryGradeLevelDescriptorId = gldd.DescriptorId
		   INNER JOIN edfi.School sch on ssa.SchoolId = sc.SchoolId
		   inner join  edfi.SectionClassPeriod scp ON ssa.LocalCourseCode = scp.LocalCourseCode and ssa.SchoolId = scp.SchoolId 
		       and ssa.SchoolYear = scp.SchoolYear and ssa.SectionIdentifier = scp.SectionIdentifier 
	   	       and ssa.SessionName = scp.SessionName --and substring(scp.ClassPeriodName,1,1) = '8'
		 where  ssa.SectionIdentifier  = 
		 ( 
			select top 1 a2.SectionIdentifier from  edfi.StudentSectionAssociation a2 
			 where a2.StudentUSI = ssa.StudentUSI  
			 and LEFT(a2.LocalCourseCode,CHARINDEX('-',a2.LocalCourseCode ) -1) = co.CourseCode --LEFT(a.LocalCourseCode,CHARINDEX('-',a.LocalCourseCode ) -1) 
			 order by LEFT(a2.LocalCourseCode,CHARINDEX('-',a2.LocalCourseCode ) -1), a2.BeginDate desc
			
		 )
		 group by ssa.SchoolId, ssa.LocalCourseCode, ssa.StudentUSI, ssa.SectionIdentifier, sc.AvailableCredits, co.CourseCode, co.LocalCourseTitle, ssa.SchoolYear,
			ssca.EntryGradeLevelDescriptorId ,  (case when ssa.SchoolId = 1 then ssa.SessionName else '' end), 
			staff.LastSurname, LocationClassroomIdentificationCode, gldd.CodeValue, scp.ClassPeriodName,
			s.StudentUniqueId, s.FirstName, s.LastSurname, ssca.GraduationSchoolYear, edorg.NameOfInstitution, sch.LocalEducationAgencyId
 ) sched
GO

create view [student360].[StudentAbsencesLocation]
as
select seoaa.StudentUSI, s.StudentUniqueId, s.FirstName, s.MiddleName, s.LastSurname, gldd.CodeValue GradeLevel, ssa.GraduationSchoolYear, ssa.SchoolYear, ssa.SchoolId, sc.LocalEducationAgencyId, MAX(seoaa.Latitude) Latitude, MAX(seoaa.Longitude) Longitude,
(select Count(Distinct(issae.EventDate)) from edfi.StudentSectionAttendanceEvent issae 
inner join edfi.StudentSchoolAssociation issa
	on issa.SchoolYear = issae.SchoolYear
	and issa.SchoolId = issae.SchoolId
	and issa.StudentUSI = issae.StudentUSI
inner join edfi.SectionClassPeriod iscp
	on iscp.SchoolId = issae.SchoolId
	and iscp.SchoolYear = issae.SchoolYear
	and iscp.SectionIdentifier = issae.SectionIdentifier
	and iscp.SessionName = issae.SessionName
	and iscp.LocalCourseCode = issae.LocalCourseCode
INNER JOIN edfi.Descriptor aed 
	ON issae.AttendanceEventCategoryDescriptorId = aed.DescriptorId
	and aed.CodeValue in ('E','U','DR', 'A', 'UP', 'S','W','X','M')
inner join edfi.SchoolYearType isyt
	on issae.SchoolYear = isyt.SchoolYear
where issae.StudentUSI = seoaa.StudentUSI
	and iscp.ClassPeriodName = '2'
	and isyt.CurrentSchoolYear = 1
	and (issae.EventDate <= ANY (select (CASE WHEN ExitWithdrawDate is null then issae.EventDate else ExitWithdrawDate END) from edfi.StudentSchoolAssociation where StudentUSI = issa.StudentUSI)
and issae.EventDate >= ANY (select EntryDate from edfi.StudentSchoolAssociation where StudentUSI = issa.StudentUSI))) AdaAbsences,
(select Max(Quantity) from student360.StudentAbsencesByPeriod sabp where sabp.StudentUSI = seoaa.StudentUSI) HighestCourseCount,
DATEDIFF(day,(select MAX(issae.EventDate) from  edfi.StudentSectionAttendanceEvent issae where issae.StudentUSI = seoaa.StudentUSI),GETDATE()) DaysFromLastAbsence
from edfi.StudentEducationOrganizationAssociationAddress seoaa
INNER JOIN edfi.StudentSchoolAssociation ssa ON seoaa.StudentUSI = ssa.StudentUSI
INNER JOIN edfi.School sc on ssa.SchoolId = sc.SchoolId
INNER JOIN edfi.Descriptor gldd on ssa.EntryGradeLevelDescriptorId = gldd.DescriptorId
INNER JOIN edfi.Student s on seoaa.StudentUSI = s.StudentUsi
where ExitWithdrawDate is null
group by seoaa.StudentUSI, ssa.SchoolId, sc.LocalEducationAgencyId, gldd.CodeValue, ssa.GraduationSchoolYear, ssa.SchoolYear, s.FirstName, s.MiddleName, s.LastSurname, s.StudentUniqueId
GO

Create view [student360].[StudentAttendanceDetail]
as
select ssae.StudentUSI, ssae.EventDate Date, ssae.LocalCourseCode CourseCode, ssae.SessionName Semester, aed.CodeValue Code, ssae.SchoolId, sch.LocalEducationAgencyId,
co.LocalCourseTitle Course,
(Case when aed.CodeValue in ('E','U','DR', 'A', 'UP', 'S','W','X','M') 
and (ssae.EventDate <= ANY (select (CASE WHEN ExitWithdrawDate is null then ssae.EventDate else ExitWithdrawDate END) 
	from edfi.StudentSchoolAssociation where StudentUSI = ssae.StudentUSI)
and ssae.EventDate >= ANY (select EntryDate from edfi.StudentSchoolAssociation where StudentUSI = ssae.StudentUSI))
THEN 'Y' ELSE 'N' END)[State], scp.ClassPeriodName [Period] from edfi.StudentSectionAttendanceEvent ssae
INNER JOIN edfi.SchoolYearType syt on syt.SchoolYear = ssae.SchoolYear
LEFT JOIN edfi.Descriptor aed 
	ON ssae.AttendanceEventCategoryDescriptorId = aed.DescriptorId
INNER JOIN edfi.SectionClassPeriod scp
	on scp.LocalCourseCode = ssae.LocalCourseCode
	and scp.SchoolId = ssae.SchoolId
	and scp.SectionIdentifier = ssae.SectionIdentifier
	and scp.SessionName = ssae.SessionName
	and scp.SchoolYear = ssae.SchoolYear
INNER JOIN edfi.School sch on sch.SchoolId = ssae.SchoolId
INNER JOIN edfi.CourseOffering co 
	on co.LocalCourseCode = ssae.LocalCourseCode
	and co.SchoolYear = ssae.SchoolYear
	and co.SchoolId = ssae.SchoolId
	and co.SessionName = ssae.SessionName
where  syt.CurrentSchoolYear = 1
GO

Create view [student360].[StudentCourseTranscript]
as
select c.CourseCode, c.CourseTitle, sar.SchoolYear, ssa.SchoolId,sch.LocalEducationAgencyId , ssa.StudentUSI, ct.FinalNumericGradeEarned, ct.AttemptedCredits, ct.EarnedCredits, td.CodeValue  from edfi.StudentAcademicRecord sar
INNER JOIN edfi.StudentSchoolAssociation ssa
	on sar.SchoolYear = ssa.SchoolYear
	and sar.StudentUSI = ssa.StudentUSI
INNER JOIN edfi.CourseTranscript ct
	on ct.TermDescriptorId = sar.TermDescriptorId
	and ct.SchoolYear = ssa.SchoolYear
	and ct.StudentUSI = ssa.StudentUSI
INNer JOIN edfi.School sch
	on sch.SchoolId = ssa.SchoolId
LEFT JOIN edfi.Course c
	on c.CourseCode = ct.CourseCode
inner join edfi.Descriptor td on td.DescriptorId = ct.TermDescriptorId
Where ssa.SchoolId < 100 and td.CodeValue != 'Year Round'
GO

CREATE TABLE [student360].[Reasons](
	[Description] [nvarchar](max) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[HasHours] [bit] NOT NULL,
	[ReasonId] [int] IDENTITY(1,1) NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
 CONSTRAINT [Reasons_PK] PRIMARY KEY CLUSTERED 
(
	[ReasonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [student360].[Reasons] ADD  CONSTRAINT [Reasons_DF_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO

SET IDENTITY_INSERT student360.Reasons ON;
GO
insert into student360.Reasons ([ReasonId], [Description], [Value], [HasHours]) 
values (1,'After School Attendance Recovery', 'After School Attendance Recovery', 1),
(2,'Saturday School Attendance Recovery', 'Saturday School Attendance Recovery', 1),
(3,'Morning Tutorials Attendance Recovery', 'Morning Tutorials Attendance Recovery', 1),
(4,'Summer School Attendance Recovery', 'Summer School Attendance Recovery', 1),
(5,'Assign Campus Mentor', 'Assign Campus Mentor', 0),
(6,'Conference with Attendance Officer', 'Conference with Attendance Officer', 0),
(7,'Daily Morning Attendance Check-In', 'Daily Morning Attendance Check-In', 0),
(8,'Home Visit', 'Home Visit', 0),
(9,'Conference with Parents', 'Conference with Parents', 0),
(10,'Referral to Community Schools', 'Referral to Community Schools', 0),
(11,'Referral to GSMYC or other agency', 'Referral to GSMYC or other agency', 0),
(12,'Student Counseling', 'Student Counseling', 0),
(13,'Other Attendance', 'Other Attendance',0),
(14,'Other Non-Attendance', 'Other Non-Attendance',0),
(15,'3-Day','3 Day Letter',0),
(16,'5-Day','5 Day Letter',0),
(17,'10-Day','10 Day Letter',0);
GO
SET IDENTITY_INSERT student360.Reasons OFF
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [student360].[StudentExtraHours](
	[StudentExtraHoursId] [int] IDENTITY(1,1) NOT NULL,
	[Version] [int] NOT NULL,
	[StudentUniqueId] [nvarchar](32) NOT NULL,
	[GradeLevel] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](75) NOT NULL,
	[LastSurname] [nvarchar](75) NOT NULL,
	[Date] [datetime2](7) NOT NULL,
	[Hours] [int] NULL,
	[SchoolYear] [smallint] NOT NULL,
	[UserCreatedUniqueId] [nvarchar](50) NOT NULL,
	[UserRole] [nvarchar](100) NOT NULL,
	[Comments] [nvarchar](256) NULL,
	[ReasonId] [int] NOT NULL,
	[UserFirstName] [nvarchar](75) NOT NULL,
	[UserLastSurname] [nvarchar](75) NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [StudentExtraHours_PK] PRIMARY KEY CLUSTERED 
(
	[StudentExtraHoursId] ASC,
	[Version] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [student360].[StudentExtraHours] ADD  CONSTRAINT [StudentExtraHours_DF_CreateDate]  DEFAULT (getutcdate()) FOR [CreateDate]
GO

ALTER TABLE [student360].[StudentExtraHours] ADD  CONSTRAINT [StudentExtraHours_DF_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [student360].[StudentExtraHours]  WITH CHECK ADD  CONSTRAINT [FK_StudentExtraHours_Reason] FOREIGN KEY([ReasonId])
REFERENCES [student360].[Reasons] ([ReasonId])
GO

ALTER TABLE [student360].[StudentExtraHours] CHECK CONSTRAINT [FK_StudentExtraHours_Reason]
GO

Create View [student360].[StudentExtraHourHistory]
as
select seh.StudentExtraHoursId,seh.StudentUniqueId, seh.GradeLevel, seh.FirstName, seh.LastSurname, seh.Version,CONVERT(nvarchar,CAST(seh.Date as Date)) Date, seh.SchoolYear, seh.Hours, rs.Value Reason,seh.ReasonId,  seh.Comments, seh.UserCreatedUniqueId, seh.UserRole, seh.UserFirstName, seh.UserLastSurname, seh.CreateDate, seh.Id, s.StudentUSI, sch.SchoolId, sch.LocalEducationAgencyId 
from student360.StudentExtraHours seh
	inner join edfi.Student s on s.StudentUniqueId = seh.StudentUniqueId
	inner join edfi.StudentSchoolAssociation ssa on ssa.StudentUSI = s.StudentUSI
	inner join edfi.School sch on sch.SchoolId = ssa.SchoolId
	inner join student360.Reasons rs on rs.ReasonId = seh.ReasonId
GO


Create View [student360].[StudentExtraHourCurrentGrid]
as
select seh.StudentExtraHoursId, seh.StudentUniqueId, seh.GradeLevel, seh.FirstName, seh.LastSurname, seh.Date,  seh.Version,
 seh.SchoolYear, seh.Reason ,seh.ReasonId, seh.UserCreatedUniqueId, seh.UserRole, seh.UserFirstName,
  seh.UserLastSurname, seh.StudentUSI, seh.SchoolId, seh.LocalEducationAgencyId, seh.Id, seh.Hours, seh.CreateDate, seh.Comments
from student360.StudentExtraHourHistory seh
where seh.Version = (Select Max(iseh.Version) from student360.StudentExtraHourHistory iseh
 where iseh.StudentExtraHoursId = seh.StudentExtraHoursId)
GO

Create View [student360].[StudentExtraHourGrid]
as
select seh.StudentUniqueId, seh.GradeLevel, seh.FirstName, seh.LastSurname, ssa.SchoolYear, s.StudentUSI, sch.SchoolId, sch.LocalEducationAgencyId,
(select SUM(scg.Hours) from student360.[StudentExtraHourCurrentGrid] scg 
where scg.StudentUniqueId = seh.StudentUniqueId and ssa.SchoolYear = scg.SchoolYear) TotalHours
 from  student360.StudentExtraHours seh 
	inner join edfi.Student s on s.StudentUniqueId = seh.StudentUniqueId
	inner join edfi.StudentSchoolAssociation ssa on ssa.StudentUSI = s.StudentUSI
	inner join edfi.School sch on sch.SchoolId = ssa.SchoolId
group by seh.StudentUniqueId, seh.GradeLevel, seh.FirstName, seh.LastSurname, ssa.SchoolYear, s.StudentUSI, sch.SchoolId, sch.LocalEducationAgencyId
GO


CREATE VIEW [student360].[StudentHighestAbsenceCourseCount] AS
SELECT StudentUSI, FirstName, LastSurname, GradeLevel, GraduationSchoolYear, SchoolYear, NameOfInstitution
 --, SessionName no longer grouped by this field
	, Max(AbsencesCount) HighestCourseCount, SchoolId, LocalEducationAgencyId, StudentUniqueId, 
	CAST(CASE WHEN MAX(AttemptedCredits)> 0 THEN 1 ELSE 0 END AS BIT) AS HasCredits
	, CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI
			and d.CodeValue = 'Homeless') is null THEN 0 ELSE 1 END AS BIT) IsHomeless
	,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'Section 504 Placement') is null THEN 0 ELSE 1 END AS BIT) Section504
	,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'At Risk') is null THEN 0 ELSE 1 END AS BIT) Ar
     ,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'Special Education') is null THEN 0 ELSE 1 END AS BIT) Sped
	 ,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'Free and Reduced Lunch') is null THEN 0 ELSE 1 END AS BIT) EcoDis
	 ,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'SSI') is null THEN 0 ELSE 1 END AS BIT) Ssi
	 ,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'Limited English Proficiency') is null THEN 0 ELSE 1 END AS BIT) Ell
	 ,(select Count(Distinct(cdce.Date))  from  edfi.CalendarDateCalendarEvent as cdce
		inner join edfi.Descriptor as d on cdce.CalendarEventDescriptorId = d.DescriptorId
		where SchoolYear = SchoolYear and SchoolId = SchoolId
		and d.Description = 'Membership Day') TotalInstructionalDays
	,(select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = s.StudentUSI) EntryDate
	,(select ((((Max(AbsencesCount)) / CONVERT(decimal(12,2), Count(Distinct(cdce.Date)))))*100)  from  edfi.CalendarDateCalendarEvent as cdce
		inner join edfi.Descriptor as d on cdce.CalendarEventDescriptorId = d.DescriptorId
		where SchoolYear = SchoolYear and SchoolId = SchoolId
		and d.Description = 'Membership Day'
		and cdce.Date >= (select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = s.StudentUSI)) AbsencePercent
	 ,(select TOP 1 sar.CumulativeGradePointAverage  from edfi.StudentAcademicRecord sar inner join edfi.SchoolYearType isyt on isyt.SchoolYear = sar.SchoolYear
	  where isyt.CurrentSchoolYear = 1 and sar.StudentUSI = s.StudentUSI ) Gpa
FROM
(SELECT s.StudentUSI, s.FirstName, s.LastSurname, gldd.CodeValue GradeLevel, ssa.GraduationSchoolYear, ssec.SchoolYear, edorg.NameOfInstitution,
	--ssec.SessionName,
	 ssec.LocalCourseCode, count(ssae.EventDate) AbsencesCount, sc.SchoolId, sc.LocalEducationAgencyId, s.StudentUniqueId, 
	MAX(sec.AvailableCredits) AttemptedCredits
		--ssae.EventDate, aed.CodeValue, aed.Description
FROM edfi.Student s
INNER JOIN edfi.StudentSchoolAssociation ssa ON s.StudentUSI = ssa.StudentUSI
INNER JOIN edfi.School sc on ssa.SchoolId = sc.SchoolId
INNER JOIN edfi.EducationOrganization edorg ON sc.SchoolId = edorg.EducationOrganizationId
INNER JOIN edfi.Descriptor gldd on ssa.EntryGradeLevelDescriptorId = gldd.DescriptorId
LEFT JOIN edfi.StudentSectionAssociation ssec 
	on ssa.StudentUSI = ssec.StudentUSI 
	and ssa.SchoolId= ssec.SchoolId 
	and ssa.SchoolYear = ssec.SchoolYear 
LEFT JOIN edfi.section sec 
	on sec.SectionIdentifier = ssec.SectionIdentifier 
	and sec.LocalCourseCode = ssec.LocalCourseCode 
	and sec.SchoolId = ssec.SchoolId
	and sec.SchoolYear = ssec.SchoolYear
	and sec.SessionName = ssec.SessionName
LEFT JOIN edfi.StudentSectionAttendanceEvent ssae 
	on ssa.StudentUSI = ssae.StudentUSI 
	and ssec.LocalCourseCode = ssae.LocalCourseCode 
	and ssec.SchoolId = ssae.SchoolId 
	and ssec.SchoolYear = ssae.SchoolYear
	and ssec.SectionIdentifier = ssae.SectionIdentifier 
	and ssec.SessionName = ssae.SessionName
	and ssae.EventDate >= (select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = s.StudentUSI)
LEFT JOIN edfi.Descriptor aed 
	ON ssae.AttendanceEventCategoryDescriptorId = aed.DescriptorId
	-- NOTE: The following codes are the ones that we consider as Absences for the purposes of 90% rule.
	-- *NOTE: Data governance should agree on what codes should be taken into account.
	and aed.CodeValue in ('TRU','M','FN','U','UP', 'RS', 'RU','W','PS','PU','RE','S','X','A', 'E', 'PE')
	where ssa.ExitWithdrawDate is null
--WHERE ssec.SchoolYear=2020 and ssec.SessionName = 'Fall Semester' --and NameOfInstitution = 'San Marcos High School'
GROUP BY s.StudentUSI, s.FirstName, s.LastSurname, gldd.CodeValue, ssa.GraduationSchoolYear, ssec.SchoolYear, edorg.NameOfInstitution, ssec.LocalCourseCode, sc.SchoolId, sc.LocalEducationAgencyId , StudentUniqueId
) s
GROUP BY StudentUSI, FirstName, LastSurname, GradeLevel, GraduationSchoolYear, SchoolYear, NameOfInstitution, SchoolId, LocalEducationAgencyId, StudentUniqueId;
--WHERE ssec.SchoolYear=2020

--SELECT count(*) from edfi.Descriptor;
--SELECT aecd.AttendanceEventCategoryDescriptorId, d.Namespace, d.CodeValue, d.Description 
--FROM edfi.AttendanceEventCategoryDescriptor aecd 
--INNER JOIN edfi.Descriptor d on aecd.AttendanceEventCategoryDescriptorId = d.DescriptorId;
GO

create view [student360].[StudentGeneralDataForDna]
as
select seoaa.StudentUSI,
		eo.NameOfInstitution,
		seoaa.StreetNumberName,
		seoaa.ApartmentRoomSuiteNumber,
		seoaa.City,
		sabb.CodeValue [State],
		seoaa.PostalCode,
		(select MAX(CumulativeGradePointAverage) from edfi.StudentAcademicRecord sar
			inner join edfi.SchoolYearType syt on syt.SchoolYear = sar.SchoolYear
		 where sar.StudentUSI = ssa.StudentUSI
			and syt.CurrentSchoolYear = 1) Gpa
from edfi.StudentSchoolAssociation ssa
INNER JOIN edfi.EducationOrganization eo on eo.EducationOrganizationId = ssa.SchoolId
INNER JOIN edfi.StudentEducationOrganizationAssociationAddress seoaa on seoaa.StudentUSI = ssa.StudentUSI
INNER JOIN edfi.Descriptor atd on atd.DescriptorId = seoaa.AddressTypeDescriptorId
INNER JOIN edfi.Descriptor sabb on sabb.DescriptorId = seoaa.StateAbbreviationDescriptorId
where ExitWithdrawDate is null
and atd.CodeValue = 'P'
GO

CREATE VIEW [auth].[TeacherToStudentUsi]
AS
SELECT staffsec.StaffUSI StaffUsi, studsec.StudentUSI StudentUsi FROM edfi.StaffSectionAssociation staffsec
INNER JOIN edfi.StudentSectionAssociation studsec
		ON staffsec.LocalCourseCode = studsec.LocalCourseCode
		AND staffsec.SchoolId = studsec.SchoolId
		AND staffsec.SchoolYear = studsec.SchoolYear
		AND staffsec.SectionIdentifier = studsec.SectionIdentifier
		and staffsec.SessionName = studsec.SessionName
INNER JOIN edfi.SchoolYearType sy
		ON staffsec.SchoolYear = sy.SchoolYear
WHERE sy.CurrentSchoolYear = 1
Group by staffsec.StaffUSI,studsec.StudentUSI
GO

create view [student360].[YTDGradeLevel]
as
(select (c.MaxStudentAttendance - c.CampusAbsences) StudentAttendance,
	   c.MaxStudentAttendance, 
	   ((c.MaxStudentAttendance - c.CampusAbsences) / CONVERT(decimal(12,2), c.MaxStudentAttendance)) * 100 AttendancePercent,
	   GradeLevel,
	   SchoolYear
 from (select Count(ssae.EventDate) CampusAbsences,
		gld.CodeValue GradeLevel,
		(select Count(Distinct(cdce.Date))  from  edfi.CalendarDateCalendarEvent as cdce
			inner join edfi.Descriptor as d on cdce.CalendarEventDescriptorId = d.DescriptorId
			where ssa.SchoolYear = cdce.SchoolYear 
			and d.Description = 'Membership Day') * 
			(select count(*) from edfi.StudentSchoolAssociation issa where issa.SchoolYear = ssa.SchoolYear and issa.EntryGradeLevelDescriptorId = gld.DescriptorId) MaxStudentAttendance,
			ssa.SchoolYear
from edfi.StudentSchoolAssociation ssa
INNER JOIN edfi.EducationOrganization eo
	on ssa.SchoolId = eo.EducationOrganizationId
INNER JOIN edfi.StudentSectionAttendanceEvent ssae 
	on ssa.StudentUSI = ssae.StudentUSI 
	and ssa.SchoolId = ssae.SchoolId 
	and ssa.SchoolYear = ssae.SchoolYear
INNER JOIN edfi.Descriptor aed 
	ON ssae.AttendanceEventCategoryDescriptorId = aed.DescriptorId
	and aed.CodeValue in ('E','U','DR', 'A', 'UP', 'S','W','X','M')
INNER JOIN edfi.Descriptor gld
	on gld.DescriptorId = ssa.EntryGradeLevelDescriptorId
where ssa.EntryDate = (select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = ssa.StudentUSI)
and ssae.EventDate >= (select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = ssa.StudentUSI)
group by ssa.SchoolYear, gld.CodeValue, gld.DescriptorId
) c)
GO


create view [student360].[YTDSchoolLevels]
as
select SUM(v1.StudentAttendance) StudentAttendance,
 SUM(v1.MaxStudentAttendance) MaxStudentAttendance,
  (SUM(v1.StudentAttendance) / CONVERT(decimal(12,2), SUM(v1.MaxStudentAttendance))) * 100 AttendancePercent,
  'All SMCISD' SchoolLevel,
v1.SchoolYear from [student360].YTDGradeLevel v1 group by v1.SchoolYear
UNION ALL 
select SUM(v1.StudentAttendance) StudentAttendance,
 SUM(v1.MaxStudentAttendance) MaxStudentAttendance,
  (SUM(v1.StudentAttendance) / CONVERT(decimal(12,2), SUM(v1.MaxStudentAttendance))) * 100 AttendancePercent,
  'High School' SchoolLevel,
v1.SchoolYear from [student360].YTDGradeLevel v1 where v1.GradeLevel in ('09','10','11','12') group by v1.SchoolYear
UNION ALL 
select SUM(v1.StudentAttendance) StudentAttendance,
 SUM(v1.MaxStudentAttendance) MaxStudentAttendance,
  (SUM(v1.StudentAttendance) / CONVERT(decimal(12,2), SUM(v1.MaxStudentAttendance))) * 100 AttendancePercent,
  'Middle School' SchoolLevel,
v1.SchoolYear from [student360].YTDGradeLevel v1 where v1.GradeLevel in ('05','06','07','08') group by v1.SchoolYear
UNION ALL
select SUM(v1.StudentAttendance) StudentAttendance,
 SUM(v1.MaxStudentAttendance) MaxStudentAttendance,
  (SUM(v1.StudentAttendance) / CONVERT(decimal(12,2), SUM(v1.MaxStudentAttendance))) * 100 AttendancePercent,
  'Elementary' SchoolLevel,
v1.SchoolYear from [student360].YTDGradeLevel v1 where v1.GradeLevel in ('01','02','03','04') group by v1.SchoolYear
UNION ALL 
select SUM(v1.StudentAttendance) StudentAttendance,
 SUM(v1.MaxStudentAttendance) MaxStudentAttendance,
  (SUM(v1.StudentAttendance) / CONVERT(decimal(12,2), SUM(v1.MaxStudentAttendance))) * 100 AttendancePercent,
  'PreK' SchoolLevel,
v1.SchoolYear from [student360].YTDGradeLevel v1 where v1.GradeLevel in ('KG','PK','EE') group by v1.SchoolYear
GO

Create view [student360].[StudentAbsencesForEmails] 
AS
(select s.FirstName StudentFirstName, s.MiddleName StudentMiddleName, s.LastSurname StudentLastName, s.StudentUniqueId, aed.CodeValue AbsenceCode, ssae.EventDate,
staff.FirstName StaffFirstName, staff.MiddleName StaffMiddleName, staff.LastSurname StaffLastname, staff.StaffUniqueId, staff.StaffUSI, scp.ClassPeriodName Period,
 co.LocalCourseTitle, gld.CodeValue GradeLevel, ssae.LocalCourseCode CourseCode, eo.ShortNameOfInstitution ShortSchoolName,
 (select top 1 ProgramName from edfi.GeneralStudentProgramAssociation gpa where gpa.StudentUSI = s.StudentUSI
	and gpa.BeginDate <= ssae.EventDate and (ssae.EventDate <= gpa.EndDate or gpa.EndDate is null)) LearnLocation,
 (select Top 1 ElectronicMailAddress from edfi.StaffElectronicMail smail where smail.StaffUSI = staff.StaffUSI) StaffEmail,
 (select Top 1 ElectronicMailAddress from edfi.StaffElectronicMail smail
	where smail.StaffUSI = homeStaff.StaffUSI) StaffHomeRoomEmail,
	homeStaff.FirstName HomeRoomStaffFirstName,
	homeStaff.MiddleName HomeRoomStaffMiddleName,
	homeStaff.LastSurname HomeRoomStaffLastSurname,
	homeStaff.StaffUSI HomeRoomStaffUsi,
	ssa.SchoolId
 from edfi.StudentSectionAttendanceEvent ssae
inner join edfi.StudentSchoolAssociation ssa
	on ssae.StudentUSI = ssa.StudentUSI
	and ssae.SchoolYear = ssa.SchoolYear
	and ssae.SchoolId =  ssa.SchoolId
inner join edfi.Descriptor gld
	on gld.DescriptorId = ssa.EntryGradeLevelDescriptorId
inner join edfi.SectionClassPeriod scp
	on scp.LocalCourseCode = ssae.LocalCourseCode
	and scp.SchoolId = ssae.SchoolId
	and scp.SectionIdentifier = ssae.SectionIdentifier
	and scp.SchoolYear = ssae.SchoolYear
	and scp.SessionName = ssae.SessionName
inner join edfi.SchoolYearType syt
	on syt.SchoolYear = ssae.SchoolYear
inner join edfi.Student s on s.StudentUSI = ssae.StudentUSI
INNER JOIN edfi.Descriptor aed 
	ON ssae.AttendanceEventCategoryDescriptorId = aed.DescriptorId
LEFT JOIN edfi.StaffSectionAssociation staffsa
	on staffsa.SectionIdentifier = ssae.SectionIdentifier
	and staffsa.LocalCourseCode = ssae.LocalCourseCode
	and staffsa.SessionName = ssae.SessionName
	and staffsa.SchoolId = ssae.SchoolId
	and staffsa.SchoolYear = ssae.SchoolYear
LEFT JOIN edfi.Descriptor cpd on cpd.DescriptorId = staffsa.ClassroomPositionDescriptorId
LEFT JOIN edfi.Staff staff
	on staffsa.StaffUSI = staff.StaffUSI
LEFT JOIN edfi.Staff homeStaff
	on ssa.Discriminator = homeStaff.StaffUniqueId
LEFT JOIN edfi.CourseOffering co
	on co.LocalCourseCode = ssae.LocalCourseCode
	and co.SchoolId = ssae.SchoolId
	and co.SchoolYear = ssae.SchoolId
	and co.SessionName = ssae.SessionName
LEFT JOIN edfi.EducationOrganization eo
	on ssae.SchoolId = eo.EducationOrganizationId
where (ssae.EventDate <= ANY (select (CASE WHEN ExitWithdrawDate is null then ssae.EventDate else ExitWithdrawDate END) 
	from edfi.StudentSchoolAssociation where StudentUSI = ssa.StudentUSI)
and ssae.EventDate >= ANY (select EntryDate from edfi.StudentSchoolAssociation where StudentUSI = ssa.StudentUSI)) 
and syt.CurrentSchoolYear = 1
and aed.CodeValue in ('TRU','U','UP', 'RU', 'PU')
and ssa.ExitWithdrawDate is null
and cpd.CodeValue = 'Teacher of Record'
and ssae.EventDate <= GETDATE())
GO

Create view [student360].[CalendarMembershipDays] 
AS
select cdce.Date, cdce.SchoolId, cdce.SchoolYear from edfi.CalendarDateCalendarEvent as cdce
			inner join edfi.Descriptor as d on cdce.CalendarEventDescriptorId = d.DescriptorId
			inner join edfi.SchoolYearType syt
				on syt.SchoolYear = cdce.SchoolYear
			where d.Description = 'Membership Day' 
			and d.CodeValue = 'P' /* Check why they repeat by P and F this was added because they repeat */
			and cdce.Date <= GETDATE()
			and syt.CurrentSchoolYear = 1
GO


Create VIEW [student360].[HomeroomToStudentUsi]
AS
Select staff.StaffUSI, ssa.StudentUSI, sch.SchoolId, sch.LocalEducationAgencyId from edfi.StudentSchoolAssociation ssa
inner join edfi.Staff staff 
	on staff.StaffUniqueId = ssa.Discriminator
inner join edfi.School sch
	on sch.SchoolId = ssa.SchoolId
inner join edfi.SchoolYearType syt
	on syt.SchoolYear = ssa.SchoolYear
where syt.CurrentSchoolYear = 1 and ssa.ExitWithdrawDate is null
GO


Create View student360.FirstDayOfSchool
AS
select MIN(Date) Date from edfi.BellScheduleDate;
GO


CREATE TABLE [student360].[AttendanceLetterType](
	[AttendanceLetterTypeId] [int] IDENTITY(1,1) NOT NULL,
	[CodeValue] [nvarchar](50) NOT NULL,
	[ShortDescription] [nvarchar](75) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [AttendanceLetterType_PK] PRIMARY KEY CLUSTERED 
(
	[AttendanceLetterTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [student360].[AttendanceLetterType] ADD  CONSTRAINT [AttendanceLetterType_DF_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO

ALTER TABLE [student360].[AttendanceLetterType] ADD  CONSTRAINT [AttendanceLetterType_DF_Id]  DEFAULT (newid()) FOR [Id]
GO

SET IDENTITY_INSERT [student360].[AttendanceLetterType] ON;
GO
INSERT INTO [student360].[AttendanceLetterType] (AttendanceLetterTypeId,CodeValue, ShortDescription, Description) values
(1,'3-Days', '3 Day Letter', '3 Day Attendance Letter'),
(2,'5-Days', '5 Day Letter', '5 Day Attendance Letter'),
(3,'10-Days', '10 Day Letter', '10 Day Attendance Letter');
GO
SET IDENTITY_INSERT [student360].[AttendanceLetterType] OFF
GO

CREATE TABLE [student360].[AttendanceLetterStatus](
	[AttendanceLetterStatusId] [int] IDENTITY(1,1) NOT NULL,
	[CodeValue] [nvarchar](50) NOT NULL,
	[ShortDescription] [nvarchar](75) NOT NULL,
	[Description] [nvarchar](1024) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [AttendanceLetterStatus_PK] PRIMARY KEY CLUSTERED 
(
	[AttendanceLetterStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [student360].[AttendanceLetterStatus] ADD  CONSTRAINT [AttendanceLetterStatus_DF_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO

ALTER TABLE [student360].[AttendanceLetterStatus] ADD  CONSTRAINT [AttendanceLetterStatus_DF_Id]  DEFAULT (newid()) FOR [Id]
GO

SET IDENTITY_INSERT [student360].[AttendanceLetterStatus] ON;
GO
INSERT INTO [student360].[AttendanceLetterStatus] (AttendanceLetterStatusId,CodeValue, ShortDescription, Description) values
(1,'Auto-Cancelled', 'Auto-Cancelled', 'Auto-Cancelled'),
(2,'Admin Override', 'Admin Override', 'Admin Override'),
(3,'Sent', 'Sent', 'Sent'),
(4,'Open', 'Open', 'Open'),
(5,'Archived', 'Archived', 'Archived');
GO
SET IDENTITY_INSERT [student360].[AttendanceLetterStatus] OFF
GO


CREATE TABLE [student360].[AttendanceLetters](
	[AttendanceLetterId] [int] IDENTITY(1,1) NOT NULL,
	[AttendanceLetterTypeId] [int] NOT NULL,
	[AttendanceLetterStatusId] [int] NOT NULL,
	[ClassPeriodName] [nvarchar](60) NOT NULL,
	[FirstAbsence] [datetime2](7) NOT NULL,
	[LastAbsence] [datetime2](7) NOT NULL,
	[StudentUniqueId] [nvarchar](32) NOT NULL,
	[FirstName] [nvarchar](75) NOT NULL,
	[MiddleName] [nvarchar](75) NULL,
	[LastSurname] [nvarchar](75) NOT NULL,
	[Comments] [nvarchar](MAX) NULL,
	[ResolutionDate] [datetime2](7) NULL,
	[GradeLevel] [nvarchar](50) NOT NULL,
	[SchoolYear] [smallint] NOT NULL,
	[SchoolId] [int] NOT NULL,
	[UserCreatedUniqueId] [nvarchar](50) NULL,
	[UserFirstName] [nvarchar](75) NULL,
	[UserLastSurname] [nvarchar](75) NULL,
	[UserRole] [nvarchar](100)  NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[Id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [AttendanceLetters_PK] PRIMARY KEY CLUSTERED 
(
	[AttendanceLetterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [student360].[AttendanceLetters] ADD  CONSTRAINT [AttendanceLetters_DF_CreateDate]  DEFAULT (getdate()) FOR [CreateDate]
GO

ALTER TABLE [student360].[AttendanceLetters] ADD  CONSTRAINT [AttendanceLetters_DF_Id]  DEFAULT (newid()) FOR [Id]
GO

ALTER TABLE [student360].[AttendanceLetters]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceLetters_AttendanceLetterStatus] FOREIGN KEY([AttendanceLetterStatusId])
REFERENCES [student360].[AttendanceLetterStatus] ([AttendanceLetterStatusId])
GO

ALTER TABLE [student360].[AttendanceLetters] CHECK CONSTRAINT [FK_AttendanceLetters_AttendanceLetterStatus]
GO

ALTER TABLE [student360].[AttendanceLetters]  WITH CHECK ADD  CONSTRAINT [FK_AttendanceLetters_AttendanceLetterType] FOREIGN KEY([AttendanceLetterTypeId])
REFERENCES [student360].[AttendanceLetterType] ([AttendanceLetterTypeId])
GO

ALTER TABLE [student360].[AttendanceLetters] CHECK CONSTRAINT [FK_AttendanceLetters_AttendanceLetterType]
GO

create view [student360].[AttendanceLetterGrid]
AS
select al.*, als.CodeValue Status, alt.CodeValue Type, sch.LocalEducationAgencyId, s.StudentUSI from student360.AttendanceLetters al
inner join student360.AttendanceLetterStatus als on al.AttendanceLetterStatusId = als.AttendanceLetterStatusId
inner join student360.AttendanceLetterType alt on al.AttendanceLetterTypeId = alt.AttendanceLetterTypeId
inner join edfi.School sch on sch.SchoolId = al.SchoolId
inner join edfi.Student s on s.StudentUniqueId = al.StudentUniqueId
where al.AttendanceLetterStatusId != 1;

GO

Create view [student360].[EducationOrganizationInformation]
as
select eoa.StreetNumberName, eo.NameOfInstitution, eo.ShortNameOfInstitution, eoa.City,  sad.CodeValue State, eoa.PostalCode,
 '(512)757-8490' Phone, principal.* from edfi.EducationOrganizationAddress eoa
inner join edfi.Descriptor sad on sad.DescriptorId = eoa.StateAbbreviationDescriptorId
inner join edfi.EducationOrganization eo on eo.EducationOrganizationId = eoa.EducationOrganizationId
inner join
(select soaa.EducationOrganizationId, staff.FirstName PrincipalFirstName, staff.MiddleName PrincipalMiddleName,
 staff.LastSurname PrincipalLastSurname
from edfi.StaffEducationOrganizationAssignmentAssociation soaa
inner join edfi.Descriptor scd on scd.DescriptorId = soaa.StaffClassificationDescriptorId
inner join edfi.Staff staff on staff.StaffUSI = soaa.StaffUSI
where CodeValue = 'Principal' and soaa.EducationOrganizationId != 105902) principal on principal.EducationOrganizationId = eoa.EducationOrganizationId
GO

