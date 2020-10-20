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