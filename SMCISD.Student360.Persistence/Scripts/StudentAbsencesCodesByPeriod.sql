Create View [student360].[StudentAbsencesCodesByPeriod]
as
select d.CodeValue AbsenceCode, d.Description, Count(d.CodeValue) Quantity, ssae.StudentUSI, scp.ClassPeriodName
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
--and d.CodeValue in('A','AES','B','C','CHI','CRT','CV','DR','E','FN','HS','I','LTS','M','N','O','OA','OC','OL','PRE','PS','PU','RE','REL','RS','RU','S','T','TRU','TST','U','UP','W','X')
and d.CodeValue in('CRT','CV','DR','E','FN','HS','I','M','N','OA','OL','PU','RE','RS','RU','S','U','UP','W','X')
group by d.CodeValue, d.Description, ssae.StudentUSI, scp.ClassPeriodName
GO

