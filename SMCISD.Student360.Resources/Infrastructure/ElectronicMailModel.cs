using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Infrastructure
{
    public class ElectronicMailModel
    {
        public int ElectronicMailTypeId { get; set; } // ElectronicMailTypeId (Primary key)
        public string ElectronicMailAddress { get; set; } // ElectronicMailAddress (length: 128)
        public bool? PrimaryEmailAddressIndicator { get; set; } // PrimaryEmailAddressIndicator
    }
}
