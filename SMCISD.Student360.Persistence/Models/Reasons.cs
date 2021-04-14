using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    [Table("Reasons", Schema = "student360")]
    public partial class Reasons
    {
        public Reasons()
        {
            StudentExtraHours = new HashSet<StudentExtraHours>();
        }

        [Required]
        public string Description { get; set; }
        [Required]
        public string Value { get; set; }
        public bool HasHours { get; set; }
        [Key]
        public int ReasonId { get; set; }
        public DateTime CreateDate { get; set; }

        [InverseProperty("Reason")]
        public virtual ICollection<StudentExtraHours> StudentExtraHours { get; set; }
    }
}
