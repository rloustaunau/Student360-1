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

