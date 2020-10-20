using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMCISD.Student360.Persistence.Models
{
    public partial class StudentAbsencesByCourse
    {
        [Column("StudentUSI")]
        public int StudentUsi { get; set; }
        public int SchoolId { get; set; }
        [Required]
        [StringLength(60)]
        public string LocalCourseCode { get; set; }
        [Required]
        [StringLength(255)]
        public string SectionIdentifier { get; set; }
        [Column(TypeName = "decimal(9, 3)")]
        public decimal? Credits { get; set; }
        [Required]
        [StringLength(60)]
        public string CourseCode { get; set; }
        [StringLength(60)]
        public string LocalCourseTitle { get; set; }
        public short SchoolYear { get; set; }
        [Required]
        [StringLength(50)]
        public string GradeLevel { get; set; }
        public int? LocalEducationAgencyId { get; set; }
        [Required]
        [StringLength(32)]
        public string StudentUniqueId { get; set; }
        [Required]
        [StringLength(75)]
        public string StudentFirstName { get; set; }
        [Required]
        [StringLength(75)]
        public string StudentLastSurname { get; set; }
        public short? GraduationSchoolYear { get; set; }
        [Required]
        [StringLength(75)]
        public string NameOfInstitution { get; set; }
        [Required]
        [StringLength(60)]
        public string SessionName { get; set; }
        [Required]
        [StringLength(75)]
        public string TeacherLastSurname { get; set; }
        [StringLength(60)]
        public string Room { get; set; }
        [Required]
        [StringLength(60)]
        public string ClassPeriodName { get; set; }
        [Column("MARK9W1")]
        [StringLength(20)]
        public string Mark9w1 { get; set; }
        [Column("MARK9W2")]
        [StringLength(20)]
        public string Mark9w2 { get; set; }
        [Column("MARK9W3")]
        [StringLength(20)]
        public string Mark9w3 { get; set; }
        [Column("MARK9W4")]
        [StringLength(20)]
        public string Mark9w4 { get; set; }
        [Column("FS1")]
        [StringLength(20)]
        public string Fs1 { get; set; }
        [Column("FS2")]
        [StringLength(20)]
        public string Fs2 { get; set; }
        [Column("YFinal")]
        [StringLength(20)]
        public string Yfinal { get; set; }
        [Column("S1Abs")]
        public int? S1abs { get; set; }
        [Column("S2Abs")]
        public int? S2abs { get; set; }
        public int? AbsencesCount { get; set; }
    }
}
