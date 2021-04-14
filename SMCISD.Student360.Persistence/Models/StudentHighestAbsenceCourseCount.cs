using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class StudentHighestAbsenceCourseCount
    {
        [Column("StudentUSI")]
        public int StudentUsi { get; set; }
        [Required]
        [StringLength(75)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(75)]
        public string LastSurname { get; set; }
        [Required]
        [StringLength(50)]
        public string GradeLevel { get; set; }
        public short? GraduationSchoolYear { get; set; }
        public short? SchoolYear { get; set; }
        [Required]
        [StringLength(75)]
        public string NameOfInstitution { get; set; }
        public int? HighestCourseCount { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        [Required]
        [StringLength(32)]
        public string StudentUniqueId { get; set; }
        public bool? HasCredits { get; set; }
        public bool? IsHomeless { get; set; }
        public bool? Section504 { get; set; }
        public bool? Ar { get; set; }
        public bool? Sped { get; set; }
        public bool? EcoDis { get; set; }
        public bool? Ssi { get; set; }
        public bool? Ell { get; set; }
        public int? TotalInstructionalDays { get; set; }
        [Column(TypeName = "date")]
        public DateTime? EntryDate { get; set; }
        [Column(TypeName = "decimal(29, 13)")]
        public decimal? AbsencePercent { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? Gpa { get; set; }
        public string GradeDescription { get; set; }
    }
}
