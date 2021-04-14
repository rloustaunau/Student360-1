using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.AttendanceLetters
{
    public class LetterFileModel
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
