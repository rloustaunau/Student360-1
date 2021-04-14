using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class StudentAbsencesForEmails
    {
        [Required]
        [StringLength(75)]
        public string StudentFirstName { get; set; }
        [StringLength(75)]
        public string StudentMiddleName { get; set; }
        [Required]
        [StringLength(75)]
        public string StudentLastName { get; set; }
        [Required]
        [StringLength(32)]
        public string StudentUniqueId { get; set; }
        [Required]
        [StringLength(50)]
        public string AbsenceCode { get; set; }
        [Column(TypeName = "date")]
        public DateTime EventDate { get; set; }
        [StringLength(75)]
        public string StaffFirstName { get; set; }
        [StringLength(75)]
        public string StaffMiddleName { get; set; }
        [StringLength(75)]
        public string StaffLastname { get; set; }
        [StringLength(32)]
        public string StaffUniqueId { get; set; }
        [Column("StaffUSI")]
        public int? StaffUsi { get; set; }
        [Required]
        [StringLength(60)]
        public string Period { get; set; }
        [StringLength(60)]
        public string LocalCourseTitle { get; set; }
        [Required]
        [StringLength(50)]
        public string GradeLevel { get; set; }
        [Required]
        [StringLength(60)]
        public string CourseCode { get; set; }
        [StringLength(75)]
        public string ShortSchoolName { get; set; }
        [StringLength(60)]
        public string LearnLocation { get; set; }
        [StringLength(128)]
        public string StaffEmail { get; set; }
        [StringLength(128)]
        public string StaffHomeRoomEmail { get; set; }
        [StringLength(75)]
        public string HomeRoomStaffFirstName { get; set; }
        [StringLength(75)]
        public string HomeRoomStaffMiddleName { get; set; }
        [StringLength(75)]
        public string HomeRoomStaffLastSurname { get; set; }
        public int? HomeRoomStaffUsi { get; set; }
        public int SchoolId { get; set; }
    }
}
