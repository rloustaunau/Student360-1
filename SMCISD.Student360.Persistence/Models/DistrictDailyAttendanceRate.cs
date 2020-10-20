using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class DistrictDailyAttendanceRate
    {
        [StringLength(4000)]
        public string Membership { get; set; }
        [StringLength(4000)]
        public string Present { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
    }
}
