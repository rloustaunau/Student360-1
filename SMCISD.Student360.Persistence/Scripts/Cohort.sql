Create view [student360].[Cohort]
AS
Select Distinct(GraduationSchoolYear) from edfi.StudentSchoolAssociation where GraduationSchoolYear is not null;
GO