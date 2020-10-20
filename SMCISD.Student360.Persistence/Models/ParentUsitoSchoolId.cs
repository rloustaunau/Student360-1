using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class ParentUsitoSchoolId
    {
        public int SchoolId { get; set; }
        [Column("ParentUSI")]
        public int ParentUsi { get; set; }
        [Column(TypeName = "date")]
        public DateTime? BeginDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }
        public long? Count { get; set; }
    }
}
