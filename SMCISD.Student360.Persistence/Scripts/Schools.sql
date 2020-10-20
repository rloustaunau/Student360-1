CREATE VIEW [student360].[Schools]
AS
SELECT s.SchoolId, eo.NameOfInstitution, d.CodeValue GradeLevel, s.LocalEducationAgencyId from edfi.School as s
INNER JOIN edfi.EducationOrganization as eo
	on s.SchoolId = eo.EducationOrganizationId
INNER JOIN edfi.SchoolGradeLevel as sgl
	on sgl.SchoolId = s.SchoolId
INNER JOIN edfi.Descriptor d on d.DescriptorId = sgl.GradeLevelDescriptorId
GO