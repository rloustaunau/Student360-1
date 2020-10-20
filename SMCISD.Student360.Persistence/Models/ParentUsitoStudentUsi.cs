using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class ParentUsitoStudentUsi
    {
        [Column("StudentUSI")]
        public int StudentUsi { get; set; }
        [Column("ParentUSI")]
        public int ParentUsi { get; set; }
    }
}
