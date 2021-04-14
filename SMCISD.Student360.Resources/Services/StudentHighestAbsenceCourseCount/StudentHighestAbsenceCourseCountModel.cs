namespace SMCISD.Student360.Resources.Services.StudentHighestAbsenceCourseCount
{
    public class StudentHighestAbsenceCourseCountModel
    {
        public int StudentUsi { get; set; }
        public string FirstName { get; set; }
        public string LastSurname { get; set; }
        public string GradeLevel { get; set; }
        public short? GraduationSchoolYear { get; set; }
        public short SchoolYear { get; set; }
        public string NameOfInstitution { get; set; }
        public int? HighestCourseCount { get; set; }
        public int SchoolId { get; set; }
        public string StudentUniqueId { get; set; }
        public string GradeDescription { get; set; }
    }
}
