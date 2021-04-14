using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class StudentAtRisk
    {
        [Column("StudentUSI")]
        public int StudentUsi { get; set; }
        public bool IsHomeless { get; set; }
        public bool Section504 { get; set; }
        public bool Ar { get; set; }
        public bool Ssi { get; set; }
        public bool Ell { get; set; }
        public bool PREPregnant { get; set; }
        public bool PREParent { get; set; }
        public bool AEP { get; set; }
        public bool Expelled { get; set; }
        public bool Dropout { get; set; }
        public bool LEP { get; set; }
        public bool FosterCare { get; set; }
        public bool ResidentialPlacementFacility { get; set; }
        public bool Incarcerated { get; set; }
        public bool AdultEd { get; set; }
        public bool PRS { get; set; }
        public bool NotAdvanced { get; set; }
    }
}
