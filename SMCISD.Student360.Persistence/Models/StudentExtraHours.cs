using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    [Table("StudentExtraHours", Schema = "student360")]
    public partial class StudentExtraHours
    {
        [Key]
        public int StudentExtraHoursId { get; set; }
        [Key]
        public int Version { get; set; }
        [Required]
        [StringLength(32)]
        public string StudentUniqueId { get; set; }
        [Required]
        [StringLength(50)]
        public string GradeLevel { get; set; }
        [Required]
        [StringLength(75)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(75)]
        public string LastSurname { get; set; }
        public DateTime Date { get; set; }
        public int? Hours { get; set; }
        public short SchoolYear { get; set; }
        [Required]
        [StringLength(50)]
        public string UserCreatedUniqueId { get; set; }
        [Required]
        [StringLength(100)]
        public string UserRole { get; set; }
        [StringLength(256)]
        public string Comments { get; set; }
        public int ReasonId { get; set; }
        [Required]
        [StringLength(75)]
        public string UserFirstName { get; set; }
        [Required]
        [StringLength(75)]
        public string UserLastSurname { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid Id { get; set; }

        [ForeignKey(nameof(ReasonId))]
        [InverseProperty(nameof(Reasons.StudentExtraHours))]
        public virtual Reasons Reason { get; set; }
    }
}
