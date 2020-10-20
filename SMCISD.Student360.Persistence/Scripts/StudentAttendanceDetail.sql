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