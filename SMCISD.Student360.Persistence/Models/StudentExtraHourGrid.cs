using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class StudentExtraHourGrid
    {
        [Required]
        [StringLength(32)]
        public string StudentUniqueId { get; set; }
        [Required]
        [StringLength(50)]
        public string GradeLevel { get; set; }
        [Required]
        [StringLength(75)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(75)]
        public string LastSurname { get; set; }
        public short? SchoolYear { get; set; }
        [Column("StudentUSI")]
        public int StudentUsi { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        public int? TotalHours { get; set; }
    }
}
