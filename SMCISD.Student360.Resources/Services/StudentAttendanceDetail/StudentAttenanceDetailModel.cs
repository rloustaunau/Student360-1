using System;

namespace SMCISD.Student360.Resources.Services.StudentAttendanceDetail
{
    public class StudentAttendanceDetailModel
    {
        public int StudentUsi { get; set; }
        public DateTime Date { get; set; }
        public string Course { get; set; }
        public string Semester { get; set; }
        public string Code { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        public string State { get; set; }
        public string Period { get; set; }
        public string Description { get; set; }
        public string Local { get; set; }
    }
}
