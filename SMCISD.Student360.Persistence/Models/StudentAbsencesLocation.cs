using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class StudentAbsencesLocation
    {
        [Column("StudentUSI")]
        public int StudentUsi { get; set; }
        [Required]
        [StringLength(32)]
        public string StudentUniqueId { get; set; }
        [Required]
        [StringLength(75)]
        public string FirstName { get; set; }
        [StringLength(75)]
        public string MiddleName { get; set; }
        [Required]
        [StringLength(75)]
        public string LastSurname { get; set; }
        [Required]
        [StringLength(50)]
        public string GradeLevel { get; set; }
        public short? GraduationSchoolYear { get; set; }
        public short? SchoolYear { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        [StringLength(20)]
        public string Latitude { get; set; }
        [StringLength(20)]
        public string Longitude { get; set; }
        public int? AdaAbsences { get; set; }
        public int? HighestCourseCount { get; set; }
        public int? DaysFromLastAbsence { get; set; }
    }
}
