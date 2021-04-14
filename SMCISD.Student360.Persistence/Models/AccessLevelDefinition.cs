using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    [Table("AccessLevelDefinition", Schema = "dbo")]
    public partial class AccessLevelDefinition
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Email { get; set; }
    }
}
