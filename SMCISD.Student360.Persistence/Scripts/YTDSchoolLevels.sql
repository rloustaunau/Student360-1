create view [student360].[YTDSchoolLevels]
as
select SUM(v1.StudentAttendance) StudentAttendance,
 SUM(v1.MaxStudentAttendance) MaxStudentAttendance,
  (SUM(v1.StudentAttendance) / CONVERT(decimal(12,2), SUM(v1.MaxStudentAttendance))) * 100 AttendancePercent,
  'All SMCISD' SchoolLevel,
v1.SchoolYear from [student360].YTDGradeLevel v1 group by v1.SchoolYear
UNION ALL 
select SUM(v1.StudentAttendance) StudentAttendance,
 SUM(v1.MaxStudentAttendance) MaxStudentAttendance,
  (SUM(v1.StudentAttendance) / CONVERT(decimal(12,2), SUM(v1.MaxStudentAttendance))) * 100 AttendancePercent,
  'High School' SchoolLevel,
v1.SchoolYear from [student360].YTDGradeLevel v1 where v1.GradeLevel in ('09','10','11','12') group by v1.SchoolYear
UNION ALL 
select SUM(v1.StudentAttendance) StudentAttendance,
 SUM(v1.MaxStudentAttendance) MaxStudentAttendance,
  (SUM(v1.StudentAttendance) / CONVERT(decimal(12,2), SUM(v1.MaxStudentAttendance))) * 100 AttendancePercent,
  'Middle School' SchoolLevel,
v1.SchoolYear from [student360].YTDGradeLevel v1 where v1.GradeLevel in ('05','06','07','08') group by v1.SchoolYear
UNION ALL
select SUM(v1.StudentAttendance) StudentAttendance,
 SUM(v1.MaxStudentAttendance) MaxStudentAttendance,
  (SUM(v1.StudentAttendance) / CONVERT(decimal(12,2), SUM(v1.MaxStudentAttendance))) * 100 AttendancePercent,
  'Elementary' SchoolLevel,
v1.SchoolYear from [student360].YTDGradeLevel v1 where v1.GradeLevel in ('01','02','03','04') group by v1.SchoolYear
UNION ALL 
select SUM(v1.StudentAttendance) StudentAttendance,
 SUM(v1.MaxStudentAttendance) MaxStudentAttendance,
  (SUM(v1.StudentAttendance) / CONVERT(decimal(12,2), SUM(v1.MaxStudentAttendance))) * 100 AttendancePercent,
  'PreK' SchoolLevel,
v1.SchoolYear from [student360].YTDGradeLevel v1 where v1.GradeLevel in ('KG','PK','EE') group by v1.SchoolYear
GO


