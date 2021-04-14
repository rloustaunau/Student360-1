using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.AttendanceLetters
{
    public class AttendanceLetterStatusModel
    {
        public int AttendanceLetterStatusId { get; set; }
        public string CodeValue { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
    }
}
