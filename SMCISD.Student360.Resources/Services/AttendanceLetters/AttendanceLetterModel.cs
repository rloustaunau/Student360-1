using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.AttendanceLetters
{
    public class AttendanceLetterModel
    {
        public int AttendanceLetterId { get; set; }
        public int AttendanceLetterTypeId { get; set; }
        public int AttendanceLetterStatusId { get; set; }
        public string ClassPeriodName { get; set; }
        public DateTime FirstAbsence { get; set; }
        public DateTime LastAbsence { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public string StudentUniqueId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastSurname { get; set; }
        public string GradeLevel { get; set; }
        public string Comments { get; set; }
        public short SchoolYear { get; set; }
        public int SchoolId { get; set; }
        public string UserCreatedUniqueId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastSurname { get; set; }
        public string UserRole { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        public int StudentUsi { get; set; }
        public string GradeDescription { get; set; }
    }
}
