using SMCISD.Student360.Persistence.Security;

namespace SMCISD.Student360.Resources.Services.StudentAbsencesLocation
{
    public class StudentAbsencesLocationModel : IStudent
    {
        public int StudentUsi { get; set; }
        public string StudentUniqueId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastSurname { get; set; }
        public string GradeLevel { get; set; }
        public short? GraduationSchoolYear { get; set; }
        public short? SchoolYear { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int? AdaAbsences { get; set; }
        public int? HighestCourseCount { get; set; }
        public int? DaysFromLastAbsence { get; set; }
    }

}
