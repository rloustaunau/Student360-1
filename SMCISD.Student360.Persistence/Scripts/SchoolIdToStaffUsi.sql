CREATE VIEW [auth].[SchoolIdToStaffUSI]
AS
-- School to Staff (through employment)
Select SchoolId, StaffUSI StaffUsi FROM (
	select	sch.SchoolId, seo_empl.StaffUSI, HireDate as BeginDate, EndDate
	from	edfi.School sch
			inner join edfi.StaffEducationOrganizationEmploymentAssociation seo_empl
				on sch.SchoolId = seo_empl.EducationOrganizationId
	UNION ALL
	-- School to Staff (through assignment)
	select	sch.SchoolId, seo_assgn.StaffUSI, BeginDate, EndDate
	from	edfi.School sch
			inner join edfi.StaffEducationOrganizationAssignmentAssociation seo_assgn
				on sch.SchoolId = seo_assgn.EducationOrganizationId 
) s
Group by s.SchoolId, s.StaffUSI -- Removing duplicates, causes data duplication on security filtering
 
--
GO