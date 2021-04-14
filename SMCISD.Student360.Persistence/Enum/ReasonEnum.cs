using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Persistence.Enum
{
    public class ReasonEnum : Enumeration<ReasonEnum>
    {
        public static readonly ReasonEnum Day3Letter = new ReasonEnum(15, "3 Day Letter");
        public static readonly ReasonEnum Day5Letter = new ReasonEnum(16, "5 Day Letter");
        public static readonly ReasonEnum Day10Letter = new ReasonEnum(17, "10 Day Letter");
        public ReasonEnum(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
