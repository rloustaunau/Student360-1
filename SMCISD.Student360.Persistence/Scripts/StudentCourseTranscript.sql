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

