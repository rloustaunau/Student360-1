using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Persistence.Grid
{
    public class WhereQueryMetadata
    {
        public string WhereString { get; set; }
        public string[] Values { get; set; } // Values to be added to the WhereString
    }
}
