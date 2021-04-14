using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    [Table("StaffAccessLevel", Schema = "dbo")]
    public partial class StaffAccessLevel
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Description { get; set; }
        public bool? IsAdmin { get; set; }
        [StringLength(50)]
        public string StaffClassificationDescriptorId { get; set; }
    }
}
