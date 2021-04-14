using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.StaffAccessLevel
{
    public class StaffAccessLevelModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public bool? IsAdmin { get; set; }
        public string StaffClassificationDescriptorId { get; set; }
    }
}
