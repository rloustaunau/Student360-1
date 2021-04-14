using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.StudentAbsencesForEmail
{
    public class StudentAbsencesForEmailModel
    {
        public string StudentFirstName { get; set; }
        public string StudentMiddleName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentUniqueId { get; set; }
        public string AbsenceCode { get; set; }
        public DateTime EventDate { get; set; }
        public string StaffFirstName { get; set; }
        public string StaffMiddleName { get; set; }
        public string StaffLastname { get; set; }
        public string StaffUniqueId { get; set; }  
        public int? StaffUsi { get; set; }
        public string Period { get; set; }
        public string LocalCourseTitle { get; set; }
        public string GradeLevel { get; set; }
        public string StaffEmail { get; set; }
        public string StaffHomeRoomEmail { get; set; }
        public string HomeRoomStaffFirstName { get; set; }
        public string HomeRoomStaffMiddleName { get; set; }
        public string HomeRoomStaffLastSurname { get; set; }
        
    }

    public class StudentAbsenceDaysModel
    {
        public DateTime Date { get; set; }
        public bool Absent { get; set; }
    }

    public class SecondaryEmailModel
    {
        public string StaffFirstName { get; set; }
        public string StaffMiddleName { get; set; }
        public string StaffLastname { get; set; }
        public string StaffEmail { get; set; }
        public List<SecondaryEmailPeriodCourseModel> CoursePeriods { get; set; }

    }

    public class SecondaryEmailPeriodCourseModel
    {
        public string Period { get; set; }
        public string CourseTitle { get; set; }
        public string CourseCode { get; set; }
        public List<SecondaryEmailStudentModel> Students { get; set; }
    }


    public class SecondaryEmailStudentModel
    {
        public string StudentFirstName { get; set; }
        public string StudentMiddleName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentUniqueId { get; set; }
        public string LearnLocation { get; set; }
        public List<StudentAbsenceDaysModel> AbsencesFromLastWeek { get; set; }
    }

    public class ElementaryEmailModel
    {
        public string StaffHomeRoomEmail { get; set; }
        public string HomeRoomStaffFirstName { get; set; }
        public string HomeRoomStaffMiddleName { get; set; }
        public string HomeRoomStaffLastSurname { get; set; }
        public List<ElementaryEmailStudentModel> Students { get; set; }
        
    }

    public class ElementaryEmailStudentModel
    {
        public string StudentFirstName { get; set; }
        public string StudentMiddleName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentUniqueId { get; set; }
        public string LearnLocation { get; set; }
        public List<StudentAbsenceDaysModel> AbsencesFromLastWeek { get; set; }
    }

    public class CampusEmailModel
    {
        public string ShortSchoolName { get; set; }
        public List<CampusEmailSchoolModel> Data { get; set; }
    }

    public class CampusEmailSchoolModel
    {
        public string StaffFirstName { get; set; }
        public string StaffMiddleName { get; set; }
        public string StaffLastname { get; set; }
        public int StaffUsi { get; set; }
        public int TotalAbsenceStudents { get; set; }
        public int TotalStudents { get; set; }
        public double AbsentPercentage { get { return ((double)TotalAbsenceStudents / (double)TotalStudents) * 100; } }
        
    }

    public class StudentAbsencesModel
    {
        public string StudentFirstName { get; set; }
        public string StudentMiddleName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentUniqueId { get; set; }
        public short SchoolYear { get; set; }
        public int SchoolId { get; set; }
        public string GradeLevel { get; set; }
        public List<StudentPeriodModel> Periods { get; set; }
     
    }

    public class StudentPeriodModel
    {
        public string Period { get; set; }
        public List<DateTime> Absences { get; set; }
    }
}
