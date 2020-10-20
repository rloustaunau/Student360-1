using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class SchoolIdToStaffUsi
    {
        public int SchoolId { get; set; }
        public int StaffUsi { get; set; }
    }
}
