using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class Adacampus
    {
        [Required]
        [StringLength(75)]
        public string NameOfInstitution { get; set; }
        public int? StudentAttendance { get; set; }
        public int? MaxStudentAttendance { get; set; }
        [Column(TypeName = "decimal(29, 13)")]
        public decimal? AttendancePercent { get; set; }
    }
}
