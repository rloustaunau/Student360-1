using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.Reasons
{
    public class ReasonsModel
    {
        public string Description { get; set; }
        public string Value { get; set; }
        public bool HasHours { get; set; }
        public int ReasonId { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
