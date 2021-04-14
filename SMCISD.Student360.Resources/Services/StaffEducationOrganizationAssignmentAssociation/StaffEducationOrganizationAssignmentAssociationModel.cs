using System;
using System.Collections.Generic;
using System.Text;

namespace SMCISD.Student360.Resources.Services.StaffEducationOrganizationAssignmentAssociation
{
    public class StaffEducationOrganizationAssignmentAssociationModel
    {
        public Guid Id { get; set; }
        public DateTime BeginDate { get; set; }
        public int EducationOrganizationId { get; set; }
        public int StaffClassificationDescriptorId { get; set; }
        public int StaffUSI { get; set; }
        public string PositionTitle { get; set; }
        public DateTime? EndDate { get; set; }
        public int? OrderOfAssignment { get; set; }
        public int? EmploymentEducationOrganizationId { get; set; }
        public int? EmploymentStatusDescriptorId { get; set; }
        public DateTime? EmploymentHireDate { get; set; }
        public string CredentialIdentifier { get; set; }
        public int? StateOfIssueStateAbbreviationDescriptorId { get; set; }
        public string Discriminator { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
