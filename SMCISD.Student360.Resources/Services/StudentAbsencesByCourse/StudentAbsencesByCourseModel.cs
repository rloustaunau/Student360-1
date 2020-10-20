using SMCISD.Student360.Persistence.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.StudentAbsencesByCourse
{
    public class StudentAbsencesByCourseModel
    {
        public int StudentUsi { get; set; }
        public int SchoolId { get; set; }
        public string LocalCourseCode { get; set; }
        public string SectionIdentifier { get; set; }
        public decimal? Credits { get; set; }
        public string CourseCode { get; set; }
        public string LocalCourseTitle { get; set; }
        public short SchoolYear { get; set; }
        public string GradeLevel { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        public string StudentUniqueId { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastSurname { get; set; }
        public short? GraduationSchoolYear { get; set; }
        public string NameOfInstitution { get; set; }
        public string SessionName { get; set; }
        public string TeacherLastSurname { get; set; }
        public string Room { get; set; }
        public string ClassPeriodName { get; set; }
        public string Mark9w1 { get; set; }
        public string Mark9w2 { get; set; }
        public string Mark9w3 { get; set; }
        public string Mark9w4 { get; set; }
        public string Fs1 { get; set; }
        public string Fs2 { get; set; }
        public string Yfinal { get; set; }
        public int? S1abs { get; set; }
        public int? S2abs { get; set; }
        public int? AbsencesCount { get; set; }

    }

    public interface IStudentProfileModel
    {
        public string ImageUrl { get; set; }
        public GridResponse Grid { get; set; }
    }

    public class StudentProfileModel : IStudentProfileModel
    {
        public string ImageUrl { get; set; }
        public GridResponse Grid { get; set; }
    }
}
