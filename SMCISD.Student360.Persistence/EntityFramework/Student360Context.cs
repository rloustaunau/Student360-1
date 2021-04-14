using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SMCISD.Student360.Persistence.Models;

namespace SMCISD.Student360.Persistence.EntityFramework
{
    public partial class Student360Context : DbContext
    {
        public Student360Context()
        {
        }

        public Student360Context(DbContextOptions<Student360Context> options)
            : base(options)
        {
        }
        public virtual DbSet<AccessLevelDefinition> AccessLevelDefinition { get; set; }
        public virtual DbSet<AccessToSystem> AccessToSystem { get; set; }
        public virtual DbSet<Adacampus> Adacampus { get; set; }
        public virtual DbSet<Adadistrict> Adadistrict { get; set; }
        public virtual DbSet<AttendanceLetterGrid> AttendanceLetterGrid { get; set; }
        public virtual DbSet<AttendanceLetterStatus> AttendanceLetterStatus { get; set; }
        public virtual DbSet<AttendanceLetterType> AttendanceLetterType { get; set; }
        public virtual DbSet<AttendanceLetters> AttendanceLetters { get; set; }
        public virtual DbSet<CalendarMembershipDays> CalendarMembershipDays { get; set; }
        public virtual DbSet<Cohort> Cohort { get; set; }
        public virtual DbSet<DistrictDailyAttendanceRate> DistrictDailyAttendanceRate { get; set; }
        public virtual DbSet<EducationOrganizationInformation> EducationOrganizationInformation { get; set; }
        public virtual DbSet<FirstDayOfSchool> FirstDayOfSchool { get; set; }
        public virtual DbSet<Grade> Grade { get; set; }
        public virtual DbSet<HomeroomToStudentUsi> HomeroomToStudentUsi { get; set; }
        public virtual DbSet<LocalEducationAgencyIdToStaffUsi> LocalEducationAgencyIdToStaffUsi { get; set; }
        public virtual DbSet<ParentUsitoSchoolId> ParentUsitoSchoolId { get; set; }
        public virtual DbSet<ParentUsitoStudentUsi> ParentUsitoStudentUsi { get; set; }
        public virtual DbSet<People> People { get; set; }
        public virtual DbSet<Reasons> Reasons { get; set; }
        public virtual DbSet<Report> Report { get; set; }
        public virtual DbSet<SchoolIdToStaffUsi> SchoolIdToStaffUsi { get; set; }
        public virtual DbSet<SchoolYears> SchoolYears { get; set; }
        public virtual DbSet<Schools> Schools { get; set; }
        public virtual DbSet<Semesters> Semesters { get; set; }
        public virtual DbSet<StaffAccessLevel> StaffAccessLevel { get; set; }
        public virtual DbSet<StaffEducationOrganizationAssignmentAssociation> StaffEducationOrganizationAssignmentAssociation { get; set; }
        public virtual DbSet<StudentAbsencesByCourse> StudentAbsencesByCourse { get; set; }
        public virtual DbSet<StudentAbsencesByPeriod> StudentAbsencesByPeriod { get; set; }
        public virtual DbSet<StudentAbsencesCodesByPeriod> StudentAbsencesCodesByPeriod { get; set; }
        public virtual DbSet<StudentAbsencesForEmails> StudentAbsencesForEmails { get; set; }
        public virtual DbSet<StudentAbsencesLocation> StudentAbsencesLocation { get; set; }
        public virtual DbSet<StudentAtRisk> StudentAtRisk { get; set; }
        public virtual DbSet<StudentAttendanceDetail> StudentAttendanceDetail { get; set; }
        public virtual DbSet<StudentCourseTranscript> StudentCourseTranscript { get; set; }
        public virtual DbSet<StudentExtraHourCurrentGrid> StudentExtraHourCurrentGrid { get; set; }
        public virtual DbSet<StudentExtraHourGrid> StudentExtraHourGrid { get; set; }
        public virtual DbSet<StudentExtraHourHistory> StudentExtraHourHistory { get; set; }
        public virtual DbSet<StudentExtraHours> StudentExtraHours { get; set; }
        public virtual DbSet<StudentGeneralDataForDna> StudentGeneralDataForDna { get; set; }
        public virtual DbSet<StudentHighestAbsenceCourseCount> StudentHighestAbsenceCourseCount { get; set; }
        public virtual DbSet<TeacherToStudentUsi> TeacherToStudentUsi { get; set; }
        public virtual DbSet<YtdgradeLevel> YtdgradeLevel { get; set; }
        public virtual DbSet<YtdschoolLevels> YtdschoolLevels { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessLevelDefinition>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("AccessLevelDefinition_PK");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<AccessToSystem>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("AccessToSystem_PK");

                entity.Property(e => e.LastLogin).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Adacampus>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ADACampus", "student360");
            });

            modelBuilder.Entity<Adadistrict>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ADADistrict", "student360");
            });

            modelBuilder.Entity<AttendanceLetterGrid>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("AttendanceLetterGrid", "student360");
            });

            modelBuilder.Entity<AttendanceLetterStatus>(entity =>
            {
                entity.HasKey(e => e.AttendanceLetterStatusId)
                    .HasName("AttendanceLetterStatus_PK");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<AttendanceLetterType>(entity =>
            {
                entity.HasKey(e => e.AttendanceLetterTypeId)
                    .HasName("AttendanceLetterType_PK");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<AttendanceLetters>(entity =>
            {
                entity.HasKey(e => e.AttendanceLetterId)
                    .HasName("AttendanceLetters_PK");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.AttendanceLetterStatus)
                    .WithMany(p => p.AttendanceLetters)
                    .HasForeignKey(d => d.AttendanceLetterStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendanceLetters_AttendanceLetterStatus");

                entity.HasOne(d => d.AttendanceLetterType)
                    .WithMany(p => p.AttendanceLetters)
                    .HasForeignKey(d => d.AttendanceLetterTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendanceLetters_AttendanceLetterType");
            });

            modelBuilder.Entity<CalendarMembershipDays>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("CalendarMembershipDays", "student360");
            });

            modelBuilder.Entity<Cohort>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Cohort", "student360");
            });

            modelBuilder.Entity<DistrictDailyAttendanceRate>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("DistrictDailyAttendanceRate", "student360");
            });

            modelBuilder.Entity<EducationOrganizationInformation>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("EducationOrganizationInformation", "student360");

                entity.Property(e => e.Phone).IsUnicode(false);
            });

            modelBuilder.Entity<FirstDayOfSchool>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("FirstDayOfSchool", "student360");
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Grade", "student360");
            });

            modelBuilder.Entity<HomeroomToStudentUsi>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("HomeroomToStudentUsi", "student360");
            });

            modelBuilder.Entity<LocalEducationAgencyIdToStaffUsi>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("LocalEducationAgencyIdToStaffUSI", "auth");
            });

            modelBuilder.Entity<ParentUsitoSchoolId>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ParentUSIToSchoolId", "auth");
            });

            modelBuilder.Entity<ParentUsitoStudentUsi>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ParentUSIToStudentUSI", "auth");
            });

            modelBuilder.Entity<People>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("People", "student360");

                entity.Property(e => e.AccessLevel).IsUnicode(false);

                entity.Property(e => e.PersonType).IsUnicode(false);
            });

            modelBuilder.Entity<Reasons>(entity =>
            {
                entity.HasKey(e => e.ReasonId)
                    .HasName("Reasons_PK");

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Report", "student360");
            });

            modelBuilder.Entity<SchoolIdToStaffUsi>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("SchoolIdToStaffUSI", "auth");
            });

            modelBuilder.Entity<SchoolYears>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("SchoolYears", "student360");
            });

            modelBuilder.Entity<Schools>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Schools", "student360");
            });

            modelBuilder.Entity<Semesters>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Semesters", "student360");
            });

            modelBuilder.Entity<StaffAccessLevel>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("AccessLevelDefinition_PK");
            });

            modelBuilder.Entity<StaffEducationOrganizationAssignmentAssociation>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("StaffEducationOrganizationAssignmentAssociation_PK");

                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<StudentAbsencesByCourse>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentAbsencesByCourse", "student360");
            });

            modelBuilder.Entity<StudentAbsencesByPeriod>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentAbsencesByPeriod", "student360");
            });

            modelBuilder.Entity<StudentAbsencesCodesByPeriod>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentAbsencesCodesByPeriod", "student360");
            });

            modelBuilder.Entity<StudentAbsencesForEmails>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentAbsencesForEmails", "student360");
            });

            modelBuilder.Entity<StudentAbsencesLocation>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentAbsencesLocation", "student360");
            });

            modelBuilder.Entity<StudentAtRisk>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentAtRisk_Discrepancies", "student360");
            });

            modelBuilder.Entity<StudentAttendanceDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentAttendanceDetail", "student360");

                entity.Property(e => e.State).IsUnicode(false);
            });

            modelBuilder.Entity<StudentCourseTranscript>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentCourseTranscript", "student360");
            });

            modelBuilder.Entity<StudentExtraHourCurrentGrid>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentExtraHourCurrentGrid", "student360");
            });

            modelBuilder.Entity<StudentExtraHourGrid>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentExtraHourGrid", "student360");
            });

            modelBuilder.Entity<StudentExtraHourHistory>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentExtraHourHistory", "student360");
            });

            modelBuilder.Entity<StudentExtraHours>(entity =>
            {
                entity.HasKey(e => new { e.StudentExtraHoursId, e.Version })
                    .HasName("StudentExtraHours_PK");

                entity.Property(e => e.StudentExtraHoursId).ValueGeneratedOnAdd();

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.HasOne(d => d.Reason)
                    .WithMany(p => p.StudentExtraHours)
                    .HasForeignKey(d => d.ReasonId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentExtraHours_Reason");
            });

            modelBuilder.Entity<StudentGeneralDataForDna>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentGeneralDataForDna", "student360");
            });

            modelBuilder.Entity<StudentHighestAbsenceCourseCount>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("StudentHighestAbsenceCourseCount", "student360");
            });

            modelBuilder.Entity<TeacherToStudentUsi>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("TeacherToStudentUsi", "auth");
            });

            modelBuilder.Entity<YtdgradeLevel>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("YTDGradeLevel", "student360");
            });

            modelBuilder.Entity<YtdschoolLevels>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("YTDSchoolLevels", "student360");

                entity.Property(e => e.SchoolLevel).IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
