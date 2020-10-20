CREATE VIEW [auth].[LocalEducationAgencyIdToStaffUSI]
AS
-- LEA to Staff (through employment)
Select LocalEducationAgencyId,StaffUSI StaffUsi FROM (
		select	emp.EducationOrganizationId as LocalEducationAgencyId, emp.StaffUSI
		from	edfi.LocalEducationAgency lea
				inner join auth.EducationOrganizationToStaffUSI_Employment emp
					on lea.LocalEducationAgencyId = emp.EducationOrganizationId
		UNION ALL
		-- LEA to Staff (through assignment)
		select	assgn.EducationOrganizationId as LocalEducationAgencyId, assgn.StaffUSI
		from	edfi.LocalEducationAgency lea
				inner join auth.EducationOrganizationToStaffUSI_Assignment assgn
					on lea.LocalEducationAgencyId = assgn.EducationOrganizationId
		UNION ALL
		-- School to Staff (through employment)
		select	sch.LocalEducationAgencyId, emp.StaffUSI
		from	edfi.School sch
				inner join auth.EducationOrganizationToStaffUSI_Employment emp
					on sch.SchoolId = emp.EducationOrganizationId
		UNION ALL
		-- School to Staff (through assignment)
		select	sch.LocalEducationAgencyId, assgn.StaffUSI
		from	edfi.School sch
				inner join auth.EducationOrganizationToStaffUSI_Assignment assgn
					on sch.SchoolId = assgn.EducationOrganizationId
) s
group by s.StaffUSI, s.LocalEducationAgencyId
--
GO


