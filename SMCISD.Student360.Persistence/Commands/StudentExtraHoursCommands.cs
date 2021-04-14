using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Commands
{
    public interface IStudentExtraHoursCommands
    {
        Task<List<StudentExtraHours>> ImportStudentExtraHours(List<StudentExtraHours> studentExtraHours);
        Task<List<StudentExtraHours>> CreateStudentExtraHourBulk(List<StudentExtraHours> studentExtraHours);
        Task<StudentExtraHours> CreateStudentExtraHours(StudentExtraHours data);
        Task<StudentExtraHours> UpdateStudentExtraHours(StudentExtraHours data, IPrincipal currentUser);
        Task<List<StudentExtraHours>> UpdateBulkStudentExtraHours(List<StudentExtraHours> list, IPrincipal currentUser);
    }

    public class StudentExtraHoursCommands : IStudentExtraHoursCommands

    {
        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public StudentExtraHoursCommands(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }

        public async Task<List<StudentExtraHours>> CreateStudentExtraHourBulk(List<StudentExtraHours> studentExtraHours)
        {

            await _db.StudentExtraHours.AddRangeAsync(studentExtraHours);
            await _db.SaveChangesAsync();

            return studentExtraHours;
        }

        public async Task<StudentExtraHours> CreateStudentExtraHours(StudentExtraHours data)
        {
            var currentStudent = await _db.StudentHighestAbsenceCourseCount
                .Where(x => x.StudentUniqueId == data.StudentUniqueId).AsNoTracking().FirstOrDefaultAsync();

            if (currentStudent == null)
                throw new FormatException($"The Student Id: {data.StudentUniqueId} doesn't exist.");

            data.FirstName = currentStudent.FirstName;
            data.LastSurname = currentStudent.LastSurname;
            data.SchoolYear = currentStudent.SchoolYear.Value;
            data.GradeLevel = currentStudent.GradeLevel;

            using (var transaction = _db.Database.BeginTransaction())
            {
                if (data.StudentExtraHoursId != 0)
                    await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [student360].[StudentExtraHours] On");

                _db.StudentExtraHours.Add(data);
                await _db.SaveChangesAsync();

                if (data.StudentExtraHoursId != 0)
                    await _db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [student360].[StudentExtraHours] Off");

                await transaction.CommitAsync();

            }



            return data;
        }

        public async Task<StudentExtraHours> UpdateStudentExtraHours(StudentExtraHours data, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;
            var oldRecord = await _db.StudentExtraHours.FirstOrDefaultAsync(x => x.Id == data.Id);

            if (oldRecord.UserCreatedUniqueId != data.UserCreatedUniqueId && Int32.Parse(claims.First(x => x.Type.Contains("level_id")).Value) >= 3)
                throw new UnauthorizedAccessException("You can only edit the records you created.");

            data.Version++;
            data.CreateDate = DateTime.Now;
            data.Reason = null;

            return await CreateStudentExtraHours(data);
        }
        public async Task<List<StudentExtraHours>> UpdateBulkStudentExtraHours(List<StudentExtraHours> list, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;

            foreach (var data in list)
            {
                var oldRecord = await _db.StudentExtraHours.FirstOrDefaultAsync(x => x.Id == data.Id);

                if (oldRecord.UserCreatedUniqueId != data.UserCreatedUniqueId && Int32.Parse(claims.First(x => x.Type.Contains("level_id")).Value) >= 3)
                    throw new UnauthorizedAccessException("You can only edit the records you created.");

                data.Version++;
                data.CreateDate = DateTime.Now;
                data.Reason = null;

                await CreateStudentExtraHours(data);
            }

            return list;
        }

        public async Task<List<StudentExtraHours>> ImportStudentExtraHours(List<StudentExtraHours> studentExtraHours)
        {
            List<StudentExtraHours> conflicts = new List<StudentExtraHours>();
            var elementarygradelevels = new string[] { "01", "02", "03", "04", "EE", "KG", "PK", "IT" };
            var firstDayOfSchool = await _db.FirstDayOfSchool.FirstOrDefaultAsync();
            // Removes any records with the required fields
            var requiredConflicts = studentExtraHours.Where(x => x.StudentUniqueId == null
            || x.Reason == null
            || x.Reason.Value == null
            || x.Comments == null
            || x.Date == DateTime.MinValue
            || x.Hours == 0
            || elementarygradelevels.Contains(x.GradeLevel)
            || string.IsNullOrEmpty(x.StudentUniqueId));

            if (requiredConflicts.Count() > 0)
                conflicts.AddRange(requiredConflicts);

            studentExtraHours.RemoveAll(x => x.StudentUniqueId == null
            || x.Reason == null
            || x.Reason.Value == null
            || x.Comments == null
            || x.Date == DateTime.MinValue
            || x.Hours == 0
            || elementarygradelevels.Contains(x.GradeLevel)
            || string.IsNullOrEmpty(x.StudentUniqueId));

            foreach (var record in studentExtraHours.ToList())
            {
                var reason = await _db.Reasons.Where(x => x.Value == record.Reason.Value.Trim() || x.ReasonId == record.ReasonId).AsNoTracking().FirstOrDefaultAsync();

                // The user can't upload something with dates in the future and the reason provided has to exist on the database
                if (reason == null || !reason.HasHours || record.Date.Date > DateTime.Now.Date.Date || firstDayOfSchool.Date > record.Date.Date)
                {
                    conflicts.Add(record);
                    studentExtraHours.Remove(record);
                }
                else
                {
                    record.ReasonId = reason.ReasonId;
                    record.Reason = null; // Cleaning object so that entity doesn't try to add a new one.
                }
            }

            if (studentExtraHours.Count() == 0)
                return conflicts;

            var minDate = studentExtraHours.Min(h => h.Date).Date.Date;
            var records = await _db.StudentExtraHours.Where(x => x.Date.Date >= minDate).AsNoTracking().ToListAsync();

            // Adding all conflicts based on table primary keys 
            if (records.Count() > 0)
                conflicts.AddRange(studentExtraHours.Where(r => records.Any(h => h.StudentUniqueId == r.StudentUniqueId && r.Date.Date == h.Date.Date && r.ReasonId == h.ReasonId && r.UserCreatedUniqueId == h.UserCreatedUniqueId && r.UserRole == h.UserRole)).ToList());

            var recordsToAdd = studentExtraHours.Where(h => !conflicts.Any(c => c.Date.Date == h.Date.Date && c.StudentUniqueId == h.StudentUniqueId && c.ReasonId == h.ReasonId && c.UserCreatedUniqueId == h.UserCreatedUniqueId && c.UserRole == h.UserRole)).ToList();


            foreach (var record in recordsToAdd.ToList())
            {
                var currentStudent = await _db.StudentHighestAbsenceCourseCount.Where(x => x.StudentUniqueId == record.StudentUniqueId).AsNoTracking().FirstOrDefaultAsync();

                if (currentStudent == null)
                {
                    conflicts.Add(record);
                    recordsToAdd.Remove(record);
                }
                else
                {
                    record.FirstName = currentStudent.FirstName;
                    record.LastSurname = currentStudent.LastSurname;
                    record.SchoolYear = currentStudent.SchoolYear.Value;
                    record.GradeLevel = currentStudent.GradeLevel;
                    record.Comments = record.Comments.Substring(0, Math.Min(record.Comments.Length, 100));
                }
            }

            if (recordsToAdd.Count() > 0)
            {
                _db.StudentExtraHours.AddRange(recordsToAdd);
                await _db.SaveChangesAsync();
            }


            return conflicts; // This have to be returned for the user to know which records haven't been processed.
        }
    }
}
