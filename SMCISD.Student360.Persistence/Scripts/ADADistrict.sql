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