using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Persistence.Enum
{
    public class AttendanceLetterStatusEnum : Enumeration<AttendanceLetterStatusEnum>
    {
        public static readonly AttendanceLetterStatusEnum AutoCancelled = new AttendanceLetterStatusEnum(1, "Auto-Cancelled");
        public static readonly AttendanceLetterStatusEnum AdminOverride = new AttendanceLetterStatusEnum(2, "Admin Override");
        public static readonly AttendanceLetterStatusEnum Sent = new AttendanceLetterStatusEnum(3, "Sent");
        public static readonly AttendanceLetterStatusEnum Open = new AttendanceLetterStatusEnum(4, "Open");
        public static readonly AttendanceLetterStatusEnum Archived = new AttendanceLetterStatusEnum(5, "Archived");
        public AttendanceLetterStatusEnum(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
