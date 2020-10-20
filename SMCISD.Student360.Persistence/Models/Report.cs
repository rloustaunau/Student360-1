using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class Report
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string ReportName { get; set; }
        [StringLength(150)]
        public string ReportUri { get; set; }
        [Column("LevelID")]
        public int? LevelId { get; set; }
    }
}
