create view [student360].[StudentAbsencesByCourse]
as
select sched.StudentUSI, sched.SchoolId,  sched.LocalCourseCode,  sched.SectionIdentifier, sched.AvailableCredits Credits, 
  sched.CourseCode, sched.LocalCourseTitle, sched.SchoolYear,  GradeLevel, LocalEducationAgencyId,
  StudentUniqueId, StudentFirstName, StudentLastSurname, GraduationSchoolYear, NameOfInstitution,
  SessionName, TeacherLastSurname, Room = LocationClassroomIdentificationCode, ClassPeriodName,  
  	   'MARK9W1' = (select top 1 LetterGradeEarned from edfi.Grade gr
						Inner join edfi.Descriptor d on gr.GradeTypeDescriptorId = d.DescriptorId
						where gr.StudentUSI = sched.StudentUSI 
						and gr.LocalCourseCode = sched.LocalCourseCode 
						and d.CodeValue = 'Grading Period'
						and gr.GradingPeriodSequence = 1),
	   'MARK9W2' = (select top 1 LetterGradeEarned from edfi.Grade gr
						Inner join edfi.Descriptor d on gr.GradeTypeDescriptorId = d.DescriptorId
						where gr.StudentUSI = sched.StudentUSI 
						and gr.LocalCourseCode = sched.LocalCourseCode 
						and d.CodeValue = 'Grading Period'
						and gr.GradingPeriodSequence = 2),
	   'MARK9W3' = (select top 1 LetterGradeEarned from edfi.Grade gr
						Inner join edfi.Descriptor d on gr.GradeTypeDescriptorId = d.DescriptorId
						where gr.StudentUSI = sched.StudentUSI 
						and gr.LocalCourseCode = sched.LocalCourseCode 
						and d.CodeValue = 'Grading Period'
						and gr.GradingPeriodSequence = 3),
	   'MARK9W4' = (select top 1 LetterGradeEarned from edfi.Grade gr
						Inner join edfi.Descriptor d on gr.GradeTypeDescriptorId = d.DescriptorId
						where gr.StudentUSI = sched.StudentUSI 
						and gr.LocalCourseCode = sched.LocalCourseCode 
						and d.CodeValue = 'Grading Period'
						and gr.GradingPeriodSequence = 4),
		'FS1' = (select top 1 LetterGradeEarned from edfi.Grade gr 
				inner join edfi.Descriptor d
				on d.DescriptorId = gr.GradeTypeDescriptorId 
				and d.CodeValue = 'Final'
				and gr.SessionName = 'Fall Semester'
			where gr.StudentUSI = sched.StudentUSI 
				and gr.LocalCourseCode = sched.LocalCourseCode 
				and sched.SectionIdentifier = gr.SectionIdentifier ),
		'FS2' = (select top 1 LetterGradeEarned from edfi.Grade gr 
				inner join edfi.Descriptor d
				on d.DescriptorId = gr.GradeTypeDescriptorId 
				and d.CodeValue = 'Final'
				and gr.SessionName = 'Spring Semester'
			where gr.StudentUSI = sched.StudentUSI 
				and gr.LocalCourseCode = sched.LocalCourseCode 
				and sched.SectionIdentifier = gr.SectionIdentifier ),
		'YFinal' = (select top 1 LetterGradeEarned from edfi.Grade gr 
				inner join edfi.Descriptor d
				on d.DescriptorId = gr.GradeTypeDescriptorId 
				and d.CodeValue = 'Final'
				and gr.SessionName = 'Year round'
			where gr.StudentUSI = sched.StudentUSI 
				and gr.LocalCourseCode = sched.LocalCourseCode 
				and sched.SectionIdentifier = gr.SectionIdentifier),
	    'S1Abs' = (Select Count(EventDate) from edfi.StudentSectionAttendanceEvent  ssatt
					INNER JOIN edfi.Descriptor ON ssatt.AttendanceEventCategoryDescriptorId = DescriptorId
					INNER JOIN edfi.CourseOffering ico 
						on ico.LocalCourseCode = ssatt.LocalCourseCode 
						and ico.SchoolId = ssatt.SchoolId	 
						and ico.SchoolYear = ssatt.SchoolYear
						and ico.SessionName = ssatt.SessionName
					where StudentUSI = sched.StudentUSI 
						and ico.CourseCode = sched.CourseCode  
						and ssatt.SchoolId = sched.SchoolId 
						and ssatt.SchoolYear = sched.SchoolYear 
						and ssatt.SectionIdentifier = sched.SectionIdentifier
						and ssatt.SessionName = 'Fall Semester'
						and ssatt.StudentUSI = sched.StudentUSI
						and CodeValue in ('X','A','E','S','U', 'UP','PE','PS','PU','RE','RS','RU','W','M','FN')),
		'S2Abs' = (Select Count(EventDate) from edfi.StudentSectionAttendanceEvent  ssatt
					INNER JOIN edfi.Descriptor ON ssatt.AttendanceEventCategoryDescriptorId = DescriptorId
					inner join edfi.CourseOffering ico 
						on ico.LocalCourseCode = ssatt.LocalCourseCode 
						and ico.SchoolId = ssatt.SchoolId	 
						and ico.SchoolYear = ssatt.SchoolYear
						and ico.SessionName = ssatt.SessionName
					where StudentUSI = sched.StudentUSI 
						and ico.CourseCode  = sched.CourseCode 
						and ssatt.SchoolId = sched.SchoolId 
						and ssatt.SchoolYear = sched.SchoolYear 
						and ssatt.SectionIdentifier = sched.SectionIdentifier
						and ssatt.SessionName = 'Spring Semester'
						and ssatt.StudentUSI = sched.StudentUSI
						and CodeValue in ('X','A','E','S','U', 'UP','PE','PS','PU','RE','RS','RU','W','M','FN')),
		'AbsencesCount' = (Select Count(EventDate) from edfi.StudentSectionAttendanceEvent  ssatt
					INNER JOIN edfi.Descriptor ON ssatt.AttendanceEventCategoryDescriptorId = DescriptorId
					inner join edfi.CourseOffering ico 
						on ico.LocalCourseCode = ssatt.LocalCourseCode 
						and ico.SchoolId = ssatt.SchoolId	 
						and ico.SchoolYear = ssatt.SchoolYear
						and ico.SessionName = ssatt.SessionName
					where StudentUSI = sched.StudentUSI 
						and ico.CourseCode  = sched.CourseCode 
						and ssatt.SchoolId = sched.SchoolId 
						and ssatt.SchoolYear = sched.SchoolYear 
						and ssatt.SectionIdentifier = sched.SectionIdentifier
						and ssatt.StudentUSI = sched.StudentUSI
						and CodeValue in ('X','A','E','S','U', 'UP','PE','PS','PU','RE','RS','RU','W','M','FN'))
from (
		 select ssa.SchoolId,  ssa.LocalCourseCode, ssa.StudentUSI
		 , s.StudentUniqueId, s.FirstName StudentFirstName
		 ,s.LastSurname StudentLastSurname, ssca.GraduationSchoolYear
		 ,edorg.NameOfInstitution, sch.LocalEducationAgencyId
		 ,ssa.SectionIdentifier, sc.AvailableCredits, 
		  co.CourseCode, co.LocalCourseTitle, ssa.SchoolYear  , ssca.EntryGradeLevelDescriptorId        
		  , SessionName = (case when ssa.SchoolId = 1 then ssa.SessionName else '' end)
		  , LocationClassroomIdentificationCode, staff.LastSurname TeacherLastSurname
		  ,GradeLevel = gldd.CodeValue, scp.ClassPeriodName
		  from edfi.StudentSectionAssociation ssa
		  inner join edfi.Student s on ssa.StudentUSI = s.StudentUSI
		   inner join edfi.StudentSchoolAssociation ssca 
		   on ssca.StudentUSI = ssa.StudentUSI 
		   inner join edfi.Section sc 
		     on sc.SectionIdentifier = ssa.SectionIdentifier 
			 and sc.LocalCourseCode = ssa.LocalCourseCode 
			 and sc.SchoolId = ssa.SchoolId
			 and sc.SchoolYear = ssa.SchoolYear
		   inner join edfi.CourseOffering co 
				on co.LocalCourseCode = ssa.LocalCourseCode 
				and co.SchoolId = ssa.SchoolId	 
				and co.SchoolYear = ssa.SchoolYear
				and co.SessionName = ssa.SessionName
		   inner join edfi.StaffSectionAssociation staffSec 
		       ON ssa.LocalCourseCode = staffSec.LocalCourseCode 
			   and ssa.SchoolId = staffSec.SchoolId 
			   and ssa.SchoolYear = staffSec.SchoolYear 
			   and ssa.SectionIdentifier = staffSec.SectionIdentifier 
			   and ssa.SessionName = staffSec.SessionName
		   inner join edfi.Staff staff ON staffSec.StaffUSI = staff.StaffUSI
		   inner join EDFI.Descriptor posDesc 
				ON staffSec.ClassroomPositionDescriptorId = posDesc.DescriptorId 
				and posDesc.CodeValue = 'Teacher of Record'
		   INNER JOIN edfi.EducationOrganization edorg ON sc.SchoolId = edorg.EducationOrganizationId
		   inner join edfi.Descriptor gldd on ssca.EntryGradeLevelDescriptorId = gldd.DescriptorId
		   INNER JOIN edfi.School sch on ssa.SchoolId = sc.SchoolId
		   inner join  edfi.SectionClassPeriod scp ON ssa.LocalCourseCode = scp.LocalCourseCode and ssa.SchoolId = scp.SchoolId 
		       and ssa.SchoolYear = scp.SchoolYear and ssa.SectionIdentifier = scp.SectionIdentifier 
	   	       and ssa.SessionName = scp.SessionName --and substring(scp.ClassPeriodName,1,1) = '8'
		 where  ssa.SectionIdentifier  = 
		 ( 
			select top 1 a2.SectionIdentifier from  edfi.StudentSectionAssociation a2 
			 where a2.StudentUSI = ssa.StudentUSI  
			 and LEFT(a2.LocalCourseCode,CHARINDEX('-',a2.LocalCourseCode ) -1) = co.CourseCode --LEFT(a.LocalCourseCode,CHARINDEX('-',a.LocalCourseCode ) -1) 
			 order by LEFT(a2.LocalCourseCode,CHARINDEX('-',a2.LocalCourseCode ) -1), a2.BeginDate desc
			
		 )
		 group by ssa.SchoolId, ssa.LocalCourseCode, ssa.StudentUSI, ssa.SectionIdentifier, sc.AvailableCredits, co.CourseCode, co.LocalCourseTitle, ssa.SchoolYear,
			ssca.EntryGradeLevelDescriptorId ,  (case when ssa.SchoolId = 1 then ssa.SessionName else '' end), 
			staff.LastSurname, LocationClassroomIdentificationCode, gldd.CodeValue, scp.ClassPeriodName,
			s.StudentUniqueId, s.FirstName, s.LastSurname, ssca.GraduationSchoolYear, edorg.NameOfInstitution, sch.LocalEducationAgencyId
 ) sched
GO