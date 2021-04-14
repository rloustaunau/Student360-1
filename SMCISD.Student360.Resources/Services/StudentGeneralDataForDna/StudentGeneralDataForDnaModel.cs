using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.StudentGeneralDataForDna
{
    public class StudentGeneralDataForDnaModel
    {
        public int StudentUsi { get; set; }
        public string NameOfInstitution { get; set; }
        public string StreetNumberName { get; set; }
        public string ApartmentRoomSuiteNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public decimal? Gpa { get; set; }
        public string Sex { get; set; }
    }
}
