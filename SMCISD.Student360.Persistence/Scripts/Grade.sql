Create view [student360].[Grade]
AS
select CodeValue from edfi.Descriptor where Namespace like '%uri://www.smcisd.net/GradeLevelDescriptor';
GO