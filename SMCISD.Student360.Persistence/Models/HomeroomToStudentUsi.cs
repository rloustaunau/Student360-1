using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class HomeroomToStudentUsi
    {
        [Column("StaffUSI")]
        public int StaffUsi { get; set; }
        [Column("StudentUSI")]
        public int StudentUsi { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
    }
}
