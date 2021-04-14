using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SMCISD.Student360.Persistence.Models
{
    
    [Table("StaffEducationOrganizationAssignmentAssociation", Schema = "edfi")]
    public partial class StaffEducationOrganizationAssignmentAssociation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime BeginDate { get; set; }
        [Required]
        public int EducationOrganizationId { get; set; }
        [Required]
        public int StaffClassificationDescriptorId { get; set; }
        [Required]
        public int StaffUSI { get; set; }        
        [StringLength(100)]
        public string PositionTitle { get; set; }
        public DateTime? EndDate { get; set; }
        public int? OrderOfAssignment { get; set; }
        public int? EmploymentEducationOrganizationId { get; set; }
        public int? EmploymentStatusDescriptorId { get; set; }
        public DateTime? EmploymentHireDate { get; set; }
        [StringLength(60)]
        public string CredentialIdentifier { get; set; }
        public int? StateOfIssueStateAbbreviationDescriptorId { get; set; }
        [StringLength(128)]
        public string Discriminator { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public DateTime LastModifiedDate { get; set; }

    }
}
