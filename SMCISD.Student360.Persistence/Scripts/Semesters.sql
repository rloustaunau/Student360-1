CREATE VIEW [student360].[Semesters]
AS
SELECT Distinct(SessionName) FROM edfi.Session;
GO