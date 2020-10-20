namespace SMCISD.Student360.Resources.Services.StudentCourseTranscript
{
    public class StudentCourseTranscriptModel
    {
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public short SchoolYear { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        public int StudentUsi { get; set; }
        public decimal? FinalNumericGradeEarned { get; set; }
        public decimal? AttemptedCredits { get; set; }
        public decimal EarnedCredits { get; set; }
    }
}
