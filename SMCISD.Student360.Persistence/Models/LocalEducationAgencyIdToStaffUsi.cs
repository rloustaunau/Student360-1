using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class LocalEducationAgencyIdToStaffUsi
    {
        public int? LocalEducationAgencyId { get; set; }
        public int StaffUsi { get; set; }
    }
}
