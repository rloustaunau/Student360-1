using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.Schools
{
    public class EducationOrganiztionInformationModel
    {
        public string StreetNumberName { get; set; }
        public string NameOfInstitution { get; set; }
        public string ShortNameOfInstitution { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public int EducationOrganizationId { get; set; }
        public string PrincipalFirstName { get; set; }
        public string PrincipalMiddleName { get; set; }
        public string PrincipalLastSurname { get; set; }
    }
}
