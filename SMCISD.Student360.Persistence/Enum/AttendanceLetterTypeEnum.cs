using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Persistence.Enum
{
    public class AttendanceLetterTypeEnum : Enumeration<AttendanceLetterTypeEnum>
    {
        public static readonly AttendanceLetterTypeEnum Day3Letter = new AttendanceLetterTypeEnum(1, "3 Day Letter");
        public static readonly AttendanceLetterTypeEnum Day5Letter = new AttendanceLetterTypeEnum(2, "5 Day Letter");
        public static readonly AttendanceLetterTypeEnum Day10Letter = new AttendanceLetterTypeEnum(3, "10 Day Letter");
        public AttendanceLetterTypeEnum(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
