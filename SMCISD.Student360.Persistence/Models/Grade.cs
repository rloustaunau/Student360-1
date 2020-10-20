using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class Grade
    {
        [Required]
        [StringLength(50)]
        public string CodeValue { get; set; }
    }
}
