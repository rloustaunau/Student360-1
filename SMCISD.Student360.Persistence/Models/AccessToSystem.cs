using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    [Table("AccessToSystem", Schema = "dbo")]
    public partial class AccessToSystem
    {
        [Key]
        public long Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        public DateTime LastLogin { get; set; }
        [Required]
        [StringLength(50)]
        public string SchoolCode { get; set; }
    }
}
