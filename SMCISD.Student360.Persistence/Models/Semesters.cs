﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class Semesters
    {
        [Required]
        [StringLength(60)]
        public string SessionName { get; set; }
    }
}
