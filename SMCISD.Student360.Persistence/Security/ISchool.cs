using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Persistence.Security
{
    public interface ISchool
    {
        public int SchoolId { get; set; }

        public int? LocalEducationAgencyId { get; set; }
    }

    public class School : ISchool
    {
        public int SchoolId { get; set; }

        public int? LocalEducationAgencyId { get; set; }
    }
}
