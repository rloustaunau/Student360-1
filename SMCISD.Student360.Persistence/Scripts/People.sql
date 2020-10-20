CREATE TABLE [dbo].[StaffLevels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LevelDescriptor] [varchar](50) NULL,
	[StaffClassificationDescriptor] [varchar](50) NULL,
	[LevelId] [int] NULL,
	[StaffClassificationDescriptorId] [varchar](50) NULL,
 CONSTRAINT [PK_auth.StaffLevels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE VIEW [student360].[People] AS
(SELECT s.StaffUSI USI, StaffUniqueId UniqueId, FirstName, LastSurname, ElectronicMailAddress, 'Staff' PersonType, d.CodeValue PositionTitle , sch.SchoolId, lea.LocalEducationAgencyId, sl.LevelDescriptor AccessLevel, sl.LevelId
FROM edfi.Staff s
INNER JOIN edfi.StaffElectronicMail se on s.StaffUSI = se.StaffUSI
INNER JOIN edfi.StaffEducationOrganizationAssignmentAssociation seoaa on s.StaffUSI = seoaa.StaffUSI
INNER JOIN edfi.Descriptor as d on seoaa.StaffClassificationDescriptorId = d.DescriptorId
INNER JOIN dbo.StaffLevels as sl on seoaa.StaffClassificationDescriptorId = sl.StaffClassificationDescriptorId
LEFT JOIN edfi.LocalEducationAgency as lea on lea.LocalEducationAgencyId = seoaa.EducationOrganizationId
LEFT JOIN edfi.School as sch on sch.SchoolId = seoaa.EducationOrganizationId
)
UNION
(SELECT p.ParentUSI USI, ParentUniqueId UniqueId, FirstName, LastSurname, ElectronicMailAddress, 'Parent' PersonType, rd.CodeValue PositionTitle, ssa.SchoolId, null LocalEducationAgencyId, 'Student' AccessLevel, 4 LevelId
FROM edfi.Parent p
inner join edfi.ParentElectronicMail pe on p.ParentUSI = pe.ParentUSI
inner join edfi.StudentParentAssociation spa on p.ParentUSI = spa.ParentUSI
inner join edfi.Descriptor rd on spa.RelationDescriptorId = rd.DescriptorId
inner join edfi.StudentSchoolAssociation  as ssa on spa.StudentUSI = ssa.StudentUSI)
UNION
(SELECT s.StudentUSI USI, StudentUniqueId UniqueId, FirstName, LastSurname, ElectronicMailAddress, 'Student' PersonType, 'Student' PositionTitle, ssa.SchoolId, null LocalEducationAgencyId, 'Student' AccessLevel, 4 LevelId
FROM edfi.Student s
INNER JOIN edfi.StudentSchoolAssociation as ssa on s.StudentUSI = ssa.StudentUSI
INNER JOIN edfi.StudentEducationOrganizationAssociation seoa on s.StudentUSI = seoa.StudentUSI and ssa.SchoolId = seoa.EducationOrganizationId
INNER JOIN edfi.StudentEducationOrganizationAssociationElectronicMail se on s.StudentUSI = se.StudentUSI and seoa.EducationOrganizationId = se.EducationOrganizationId)
GO

