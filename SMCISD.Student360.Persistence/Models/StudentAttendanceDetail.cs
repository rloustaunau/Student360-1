using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class StudentAttendanceDetail
    {
        [Column("StudentUSI")]
        public int StudentUsi { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        [Required]
        [StringLength(60)]
        public string CourseCode { get; set; }
        [Required]
        [StringLength(60)]
        public string Semester { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        [StringLength(60)]
        public string Course { get; set; }
        [Required]
        [StringLength(1)]
        public string State { get; set; }
        [Required]
        [StringLength(60)]
        public string Period { get; set; }
        [StringLength(60)]
        public string Description { get; set; }
        public string Local { get; set; }
    }
}
