using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class People
    {
        [Column("USI")]
        public int Usi { get; set; }
        [Required]
        [StringLength(32)]
        public string UniqueId { get; set; }
        [Required]
        [StringLength(75)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(75)]
        public string LastSurname { get; set; }
        [Required]
        [StringLength(128)]
        public string ElectronicMailAddress { get; set; }
        [Required]
        [StringLength(5)]
        public string PersonType { get; set; }
        [Required]
        [StringLength(50)]
        public string PositionTitle { get; set; }
        public int? SchoolId { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        [StringLength(50)]
        public string AccessLevel { get; set; }
        public string LevelId { get; set; }
    }
}
