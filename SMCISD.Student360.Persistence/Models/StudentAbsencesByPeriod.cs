using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class StudentAbsencesByPeriod
    {
        public int? Quantity { get; set; }
        [Column("StudentUSI")]
        public int StudentUsi { get; set; }
        [Required]
        [StringLength(60)]
        public string ClassPeriodName { get; set; }
    }
}
