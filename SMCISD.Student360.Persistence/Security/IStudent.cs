namespace SMCISD.Student360.Persistence.Security
{
    public interface IStudent
    {
        public int StudentUsi { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
    }

    public class Student : IStudent
    {
        public int StudentUsi { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
    }
}
