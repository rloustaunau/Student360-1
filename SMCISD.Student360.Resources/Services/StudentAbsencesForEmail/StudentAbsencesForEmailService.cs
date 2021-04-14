using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using SMCISD.Student360.Persistence.Models;
using SMCISD.Student360.Persistence.Queries;
using SMCISD.Student360.Persistence.Queries.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SMCISD.Student360.Resources.Services.StudentAbsencesForEmail
{
    public interface IStudentAbsencesForEmailService
    {
        Task<List<ElementaryEmailModel>> GetDataForElementaryDailyEmails();
        Task<List<SecondaryEmailModel>> GetDataForSecondaryDailyEmails();
        Task<List<CampusEmailModel>> GetDataForCampusDailyEmails();
        Task<List<StudentAbsencesModel>> GetAbsencesByStudent();
    }

    public class StudentAbsencesForEmailService : IStudentAbsencesForEmailService
    {
        private readonly IStudentAbsencesForEmailsQueries _queries;
        private readonly ICalendarMembershipDaysQueries _calendarQueries;
        private readonly ITeacherToStudentUsiQueries _teacherToStudentUsiQueries;
        private readonly IHomeroomToStudentUsiQueries _homeroomToStudentUsiQueries;
        private readonly IConfiguration _config;
        private int EmailThreshold;
        private int SecondaryDaysToLookBack;
        private int ElementaryDaysToLookBack;
        public StudentAbsencesForEmailService(IHomeroomToStudentUsiQueries homeroomToStudentUsiQueries, IStudentAbsencesForEmailsQueries queries, ICalendarMembershipDaysQueries calendarQueries, ITeacherToStudentUsiQueries teacherToStudentUsiQueries, IConfiguration config)
        {
            _config = config;
            EmailThreshold = _config.GetSection("Notifications:DailyAbsence:ThresholdAbsences").Get<int>();
            SecondaryDaysToLookBack = _config.GetSection("Notifications:DailyAbsence:SecondaryDaysToLookBack").Get<int>();
            ElementaryDaysToLookBack = _config.GetSection("Notifications:DailyAbsence:ElementaryDaysToLookBack").Get<int>();
            _queries = queries;
            _calendarQueries = calendarQueries;
            _teacherToStudentUsiQueries = teacherToStudentUsiQueries;
            _homeroomToStudentUsiQueries = homeroomToStudentUsiQueries;
        }

        //public async Task<List<StudentAbsencesForEmailModel>> Get()
        //{
        //    var entityList = await _queries.Get();

        //    return entityList.Select(x => MapSemestersEntityToSemestersModel(x)).ToList();
        //}


        public async Task<List<ElementaryEmailModel>> GetDataForElementaryDailyEmails()
        {
            var entityList = await _queries.GetAbsencesForElementaryTeacher();
            var endResult = new List<ElementaryEmailModel>();

            var groupedList = entityList.GroupBy(x => new {
                x.StaffHomeRoomEmail,
                x.HomeRoomStaffFirstName,
                x.HomeRoomStaffMiddleName,
                x.HomeRoomStaffLastSurname,
                x.SchoolId
            });


            foreach (var g in groupedList)
            {
                var caledarDays = new List<DateTime>();
                for (int i = ElementaryDaysToLookBack; i > 0; i--)
                    caledarDays.Add((await _calendarQueries.GetPastValidDateForEmails(g.Key.SchoolId, i)).Date.Date);

                var staffEmailData = new ElementaryEmailModel
                {
                    StaffHomeRoomEmail = g.Key.StaffHomeRoomEmail,
                    HomeRoomStaffFirstName = g.Key.HomeRoomStaffFirstName,
                    HomeRoomStaffMiddleName = g.Key.HomeRoomStaffMiddleName,
                    HomeRoomStaffLastSurname = g.Key.HomeRoomStaffLastSurname,
                    Students = g.GroupBy(x => new { x.StudentUniqueId, x.StudentFirstName, x.StudentLastName, x.StudentMiddleName, x.LearnLocation })
                        .Select(x => new ElementaryEmailStudentModel {
                            StudentUniqueId = x.Key.StudentUniqueId,
                            StudentFirstName = x.Key.StudentFirstName,
                            StudentMiddleName = x.Key.StudentMiddleName,
                            StudentLastName = x.Key.StudentLastName,
                            LearnLocation = x.Key.LearnLocation,
                            AbsencesFromLastWeek = CalculateLastWeekDays(caledarDays, x)
                        }).Where(x => x.AbsencesFromLastWeek.Where(d => d.Absent).Count() >= EmailThreshold).ToList(),
                };

                if (staffEmailData.Students.Count > 0)
                    endResult.Add(staffEmailData);
            }

            return endResult;
        }

        public async Task<List<SecondaryEmailModel>> GetDataForSecondaryDailyEmails()
        {
            var entityList = await _queries.GetAbsencesForSecondaryTeacher();
            var endResult = new List<SecondaryEmailModel>();

            var groupedList = entityList.GroupBy(x => new
            {
                x.StaffEmail,
                x.StaffFirstName,
                x.StaffMiddleName,
                x.StaffLastname,
                x.SchoolId,
            });

            foreach (var g in groupedList)
            {
                var caledarDays = new List<DateTime>();
                for (int i = SecondaryDaysToLookBack; i > 0; i--)
                    caledarDays.Add((await _calendarQueries.GetPastValidDateForEmails(g.Key.SchoolId, i)).Date.Date);

                var staffEmailData = new SecondaryEmailModel
                {
                    StaffEmail = g.Key.StaffEmail,
                    StaffFirstName = g.Key.StaffFirstName,
                    StaffMiddleName = g.Key.StaffMiddleName,
                    StaffLastname = g.Key.StaffLastname,
                    CoursePeriods = g.GroupBy(cp => new { cp.Period, cp.CourseCode })
                    .Select(cp => new SecondaryEmailPeriodCourseModel {
                        Period = cp.Key.Period,
                        CourseCode = cp.Key.CourseCode,
                        Students = cp.GroupBy(x => new { x.StudentUniqueId, x.StudentFirstName, x.StudentLastName, x.StudentMiddleName })
                                    .Where(x => x.Count() >= EmailThreshold)
                                    .Select(x => new SecondaryEmailStudentModel
                                    {
                                        StudentUniqueId = x.Key.StudentUniqueId,
                                        StudentFirstName = x.Key.StudentFirstName,
                                        StudentMiddleName = x.Key.StudentMiddleName,
                                        StudentLastName = x.Key.StudentLastName,
                                        LearnLocation = "RA",
                                        AbsencesFromLastWeek = CalculateLastWeekDays(caledarDays, x)
                                    }).Where(x => x.AbsencesFromLastWeek.Where(d => d.Absent).Count() >= EmailThreshold).ToList(),
                    }).Where(x => x.Students.Count() > 0).OrderBy(m=> m.Period).ToList()// Missing CourseTitle from data
                };

                if (staffEmailData.CoursePeriods.Count() > 0)
                    endResult.Add(staffEmailData);
            }

            return endResult;
        }

        public async Task<List<CampusEmailModel>> GetDataForCampusDailyEmails()
        {
            var secondaryList = await _queries.GetAbsencesForSecondaryTeacher();
            var endResult = new List<CampusEmailModel>();

            var groupedList = secondaryList.GroupBy(x => new
            {
                x.SchoolId,
                x.ShortSchoolName,
            });

            foreach (var g in groupedList)
            {
                var pastDate = await _calendarQueries.GetPastValidDateForEmails(g.Key.SchoolId, ElementaryDaysToLookBack);

                var staffEmailData = new CampusEmailModel
                {
                    ShortSchoolName = g.Key.ShortSchoolName,
                    Data = g.GroupBy(x => new
                    {
                        x.StaffFirstName,
                        x.StaffMiddleName,
                        x.StaffLastname,
                        x.StaffUsi
                    }).Select(x => new CampusEmailSchoolModel
                    {
                        StaffFirstName = x.Key.StaffFirstName,
                        StaffMiddleName = x.Key.StaffMiddleName,
                        StaffLastname = x.Key.StaffLastname,
                        StaffUsi = x.Key.StaffUsi.Value,
                        TotalAbsenceStudents = x.GroupBy(s => new { s.StudentUniqueId, s.StudentFirstName, s.StudentLastName, s.StudentMiddleName })
                       .Where(s => s.Count() >= EmailThreshold).Count(),
                    }).ToList()
                };

                foreach (var record in staffEmailData.Data)
                    record.TotalStudents = (await _homeroomToStudentUsiQueries.GetStaffHomeroomStudents(record.StaffUsi)).Count();

                endResult.Add(staffEmailData);
            };


            var elementaryList = await _queries.GetAbsencesForElementaryTeacher();

            var elementaryGroupedList = elementaryList.GroupBy(x => new
            {
                x.SchoolId,
                x.ShortSchoolName
            });

            foreach (var g in elementaryGroupedList)
            {
                var pastDate = await _calendarQueries.GetPastValidDateForEmails(g.Key.SchoolId, SecondaryDaysToLookBack);
                var staffEmailData = new CampusEmailModel
                {
                    ShortSchoolName = g.Key.ShortSchoolName,
                    Data = g.GroupBy(x => new {
                        x.HomeRoomStaffFirstName,
                        x.HomeRoomStaffLastSurname,
                        x.HomeRoomStaffMiddleName,
                        x.HomeRoomStaffUsi
                    }).Select(x => new CampusEmailSchoolModel {
                        StaffFirstName = x.Key.HomeRoomStaffFirstName,
                        StaffMiddleName = x.Key.HomeRoomStaffMiddleName,
                        StaffLastname = x.Key.HomeRoomStaffLastSurname,
                        StaffUsi = x.Key.HomeRoomStaffUsi.Value,
                        TotalAbsenceStudents = x.GroupBy(s => new { s.StudentUniqueId, s.StudentFirstName, s.StudentLastName, s.StudentMiddleName })
                       .Where(s => s.Count() >= EmailThreshold).Count(),
                    }).ToList()
                };

                foreach(var record in staffEmailData.Data)
                    record.TotalStudents = (await _homeroomToStudentUsiQueries.GetStaffHomeroomStudents(record.StaffUsi)).Count();

                staffEmailData.Data = staffEmailData.Data.OrderByDescending(x => x.AbsentPercentage).ToList();

                endResult.Add(staffEmailData);
            }

            return endResult;
        }

        public async Task<List<StudentAbsencesModel>> GetAbsencesByStudent()
        {
            var secondaryList = await _queries.GetAbsencesForSecondaryTeacher();
            var elementaryList = await _queries.GetAbsencesForElementaryTeacher();

            return secondaryList.Concat(elementaryList)
                .Where(x => x.EventDate <= DateTime.Now.Date.AddDays(-2)) // Filtering recent absences to make sure we only count the more stable ones
                .GroupBy(x => new
            {
                x.StudentUniqueId,
                x.StudentFirstName,
                x.StudentLastName,
                x.StudentMiddleName,
                x.SchoolId,
                x.GradeLevel
            }).Select(g => new StudentAbsencesModel {
                StudentFirstName = g.Key.StudentFirstName,
                StudentMiddleName = g.Key.StudentMiddleName,
                StudentLastName = g.Key.StudentLastName,
                GradeLevel = g.Key.GradeLevel,
                SchoolId = g.Key.SchoolId, 
                StudentUniqueId = g.Key.StudentUniqueId,
                Periods = g.GroupBy(x => x.Period).Select(x => new StudentPeriodModel {
                    Period = x.Key,
                    Absences = x.Select(y => y.EventDate).OrderBy(y => y.Date).ToList()
                }).ToList()
            }).ToList();
        }



        private  List<StudentAbsenceDaysModel> CalculateLastWeekDays(List<DateTime> calendarDates, IGrouping<object, StudentAbsencesForEmails> g)
        {

            var days = new List<StudentAbsenceDaysModel>();
            foreach(var day in calendarDates)
                days.Add(new StudentAbsenceDaysModel { Date = day, Absent = g.Any(x => x.EventDate.Date == day) });

            return days;
        }


        //private Persistence.Models.Semesters MapSemestersModelToSemestersEntity(SemestersModel model)
        //{
        //    return new StudentAbsencesForEmail
        //    {
        //      SessionName = model.SessionName
        //    };
        //}

        //private SemestersModel MapSemestersEntityToSemestersModel(Persistence.Models.Semesters entity)
        //{
        //    return new SemestersModel
        //    {
        //        SessionName = entity.SessionName
        //    };
        //}

    }
}
