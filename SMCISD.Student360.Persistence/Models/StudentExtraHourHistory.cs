using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class StudentExtraHourHistory
    {
        public int StudentExtraHoursId { get; set; }
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
        public int Version { get; set; }
        [StringLength(30)]
        public string Date { get; set; }
        public short SchoolYear { get; set; }
        public int? Hours { get; set; }
        [Required]
        public string Reason { get; set; }
        public int ReasonId { get; set; }
        [StringLength(256)]
        public string Comments { get; set; }
        [Required]
        [StringLength(50)]
        public string UserCreatedUniqueId { get; set; }
        [Required]
        [StringLength(100)]
        public string UserRole { get; set; }
        [Required]
        [StringLength(75)]
        public string UserFirstName { get; set; }
        [Required]
        [StringLength(75)]
        public string UserLastSurname { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid Id { get; set; }
        [Column("StudentUSI")]
        public int StudentUsi { get; set; }
        public int SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
    }
}
