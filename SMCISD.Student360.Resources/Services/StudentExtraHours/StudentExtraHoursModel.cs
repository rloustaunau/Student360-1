using SMCISD.Student360.Resources.Services.Reasons;
using System;

namespace SMCISD.Student360.Resources.Services.StudentExtraHours
{
    public class StudentExtraHoursModel
    {
        public int StudentExtraHoursId { get; set; }
        public int Version { get; set; }
        public string StudentUniqueId { get; set; }
        public string GradeLevel { get; set; }
        public string FirstName { get; set; }
        public string LastSurname { get; set; }
        public DateTime Date { get; set; }
        public int? Hours { get; set; }
        public short SchoolYear { get; set; }
        public string UserCreatedUniqueId { get; set; }
        public string UserRole { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastSurname { get; set; }
        public string UserName { get => UserFirstName + " " + UserLastSurname; }
        public string Comments { get; set; }
        public int ReasonId { get; set; }
        public ReasonsModel Reason { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid Id { get; set; }
    }
}
