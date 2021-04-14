using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.AttendanceLetters
{
    public class AttendaceLetterTypeGridDataModel {
        public int Count { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public int MaxLetterAge { get; set; }
        public int AverageLetterAge { get; set; }
    }
}
