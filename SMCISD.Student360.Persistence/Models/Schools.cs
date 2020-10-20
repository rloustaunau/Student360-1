using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class Schools
    {
        public int SchoolId { get; set; }
        [Required]
        [StringLength(75)]
        public string NameOfInstitution { get; set; }
        [Required]
        [StringLength(50)]
        public string GradeLevel { get; set; }
        public int? LocalEducationAgencyId { get; set; }
    }
}
