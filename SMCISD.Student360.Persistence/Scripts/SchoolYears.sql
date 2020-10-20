CREATE VIEW [student360].[SchoolYears]
AS
SELECT SchoolYear from edfi.SchoolYearType WHERE CurrentSchoolYear = 1;
GO
