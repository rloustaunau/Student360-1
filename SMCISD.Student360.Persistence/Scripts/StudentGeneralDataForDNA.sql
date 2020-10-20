CREATE VIEW [student360].[StudentHighestAbsenceCourseCount] AS
SELECT StudentUSI, FirstName, LastSurname, GradeLevel, GraduationSchoolYear, SchoolYear, NameOfInstitution
 --, SessionName no longer grouped by this field
	, Max(AbsencesCount) HighestCourseCount, SchoolId, LocalEducationAgencyId, StudentUniqueId, 
	CAST(CASE WHEN MAX(AttemptedCredits)> 0 THEN 1 ELSE 0 END AS BIT) AS HasCredits
	, CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI
			and d.CodeValue = 'Homeless') is null THEN 0 ELSE 1 END AS BIT) IsHomeless
	,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'Section 504 Placement') is null THEN 0 ELSE 1 END AS BIT) Section504
	,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'At Risk') is null THEN 0 ELSE 1 END AS BIT) Ar
     ,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'Special Education') is null THEN 0 ELSE 1 END AS BIT) Sped
	 ,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'Free and Reduced Lunch') is null THEN 0 ELSE 1 END AS BIT) EcoDis
	 ,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'SSI') is null THEN 0 ELSE 1 END AS BIT) Ssi
	 ,CAST(CASE WHEN (Select top 1 d.CodeValue from edfi.StudentEducationOrganizationAssociationProgramParticipation as seoap
			inner join edfi.Descriptor as d on seoap.ProgramTypeDescriptorId = d.DescriptorId
			where seoap.StudentUSI = s.StudentUSI and (seoap.EducationOrganizationId = s.SchoolId or seoap.EducationOrganizationId = s.LocalEducationAgencyId)
			and d.CodeValue = 'Limited English Proficiency') is null THEN 0 ELSE 1 END AS BIT) Ell
	 ,(select Count(Distinct(cdce.Date))  from  edfi.CalendarDateCalendarEvent as cdce
		inner join edfi.Descriptor as d on cdce.CalendarEventDescriptorId = d.DescriptorId
		where SchoolYear = SchoolYear and SchoolId = SchoolId
		and d.Description = 'Membership Day') TotalInstructionalDays
	,(select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = s.StudentUSI) EntryDate
	,(select ((((Max(AbsencesCount)) / CONVERT(decimal(12,2), Count(Distinct(cdce.Date)))))*100)  from  edfi.CalendarDateCalendarEvent as cdce
		inner join edfi.Descriptor as d on cdce.CalendarEventDescriptorId = d.DescriptorId
		where SchoolYear = SchoolYear and SchoolId = SchoolId
		and d.Description = 'Membership Day'
		and cdce.Date >= (select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = s.StudentUSI)) AbsencePercent
	 ,(select TOP 1 sar.CumulativeGradePointAverage  from edfi.StudentAcademicRecord sar inner join edfi.SchoolYearType isyt on isyt.SchoolYear = sar.SchoolYear
	  where isyt.CurrentSchoolYear = 1 and sar.StudentUSI = s.StudentUSI ) Gpa
FROM
(SELECT s.StudentUSI, s.FirstName, s.LastSurname, gldd.CodeValue GradeLevel, ssa.GraduationSchoolYear, ssec.SchoolYear, edorg.NameOfInstitution,
	--ssec.SessionName,
	 ssec.LocalCourseCode, count(ssae.EventDate) AbsencesCount, sc.SchoolId, sc.LocalEducationAgencyId, s.StudentUniqueId, 
	MAX(sec.AvailableCredits) AttemptedCredits
		--ssae.EventDate, aed.CodeValue, aed.Description
FROM edfi.Student s
INNER JOIN edfi.StudentSchoolAssociation ssa ON s.StudentUSI = ssa.StudentUSI
INNER JOIN edfi.School sc on ssa.SchoolId = sc.SchoolId
INNER JOIN edfi.EducationOrganization edorg ON sc.SchoolId = edorg.EducationOrganizationId
INNER JOIN edfi.Descriptor gldd on ssa.EntryGradeLevelDescriptorId = gldd.DescriptorId
LEFT JOIN edfi.StudentSectionAssociation ssec 
	on ssa.StudentUSI = ssec.StudentUSI 
	and ssa.SchoolId= ssec.SchoolId 
	and ssa.SchoolYear = ssec.SchoolYear 
LEFT JOIN edfi.section sec 
	on sec.SectionIdentifier = ssec.SectionIdentifier 
	and sec.LocalCourseCode = ssec.LocalCourseCode 
	and sec.SchoolId = ssec.SchoolId
	and sec.SchoolYear = ssec.SchoolYear
	and sec.SessionName = ssec.SessionName
LEFT JOIN edfi.StudentSectionAttendanceEvent ssae 
	on ssa.StudentUSI = ssae.StudentUSI 
	and ssec.LocalCourseCode = ssae.LocalCourseCode 
	and ssec.SchoolId = ssae.SchoolId 
	and ssec.SchoolYear = ssae.SchoolYear
	and ssec.SectionIdentifier = ssae.SectionIdentifier 
	and ssec.SessionName = ssae.SessionName
	and ssae.EventDate >= (select MAX(EntryDate) from edfi.StudentSchoolAssociation where StudentUSI = s.StudentUSI)
LEFT JOIN edfi.Descriptor aed 
	ON ssae.AttendanceEventCategoryDescriptorId = aed.DescriptorId
	-- NOTE: The following codes are the ones that we consider as Absences for the purposes of 90% rule.
	-- *NOTE: Data governance should agree on what codes should be taken into account.
	and aed.CodeValue in ('E','U','DR', 'A', 'UP', 'S','W','X','M')
	where ssa.ExitWithdrawDate is null
--WHERE ssec.SchoolYear=2020 and ssec.SessionName = 'Fall Semester' --and NameOfInstitution = 'San Marcos High School'
GROUP BY s.StudentUSI, s.FirstName, s.LastSurname, gldd.CodeValue, ssa.GraduationSchoolYear, ssec.SchoolYear, edorg.NameOfInstitution, ssec.LocalCourseCode, sc.SchoolId, sc.LocalEducationAgencyId , StudentUniqueId
) s
GROUP BY StudentUSI, FirstName, LastSurname, GradeLevel, GraduationSchoolYear, SchoolYear, NameOfInstitution, SchoolId, LocalEducationAgencyId, StudentUniqueId;
--WHERE ssec.SchoolYear=2020

--SELECT count(*) from edfi.Descriptor;
--SELECT aecd.AttendanceEventCategoryDescriptorId, d.Namespace, d.CodeValue, d.Description 
--FROM edfi.AttendanceEventCategoryDescriptor aecd 
--INNER JOIN edfi.Descriptor d on aecd.AttendanceEventCategoryDescriptorId = d.DescriptorId;
GO


