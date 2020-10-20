using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IStudentExtraHoursQueries : IGridData
    {
        Task<GridResponse> GetStudentExtraHours(GridRequest request, IPrincipal currentUser);
        Task<List<StudentExtraHours>> ImportStudentExtraHours(List<StudentExtraHours> studentExtraHours);
        Task<GridResponse> GetHistoryDataById(GridRequest request, IPrincipal currentUser);
        Task<StudentExtraHours> CreateStudentExtraHours(StudentExtraHours data);
        Task<StudentExtraHours> UpdateStudentExtraHours(StudentExtraHours data, IPrincipal currentUser);
    }

    public class StudentExtraHoursQueries : IStudentExtraHoursQueries

    {
        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public StudentExtraHoursQueries(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }
        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser)
        {
            // Gets a list of all the last versions of the student extra hours.
            var query = _db.StudentExtraHourGrid;
            var securedQuery = _auth.ApplySecurity(query, query, currentUser);
            var gridMetadata = request.ProcessMetadata(new StudentExtraHourCurrentGrid());

            return await securedQuery.ExecuteGridQuery(gridMetadata, request.AllData);
        }
        public async Task<GridResponse> GetHistoryDataById(GridRequest request, IPrincipal currentUser)
        {
            // Gets a list of a full historic records of a student extra hour.
            // GridRequest.Filters must have a studentusi filter assigned to it.
            var query = _db.StudentExtraHourCurrentGrid;
            var securedQuery = _auth.ApplySecurity(query, query, currentUser);
            var gridMetadata = request.ProcessMetadata(new StudentExtraHourGrid());

            return await securedQuery.ExecuteGridQuery(gridMetadata, request.AllData);
        }
        public async Task<GridResponse> GetStudentExtraHours(GridRequest request, IPrincipal currentUser) 
        {
            // GridRequest.Filters must have a studentusi filter assigned to it.
            var query = _db.StudentExtraHourCurrentGrid;
            var securedQuery = _auth.ApplySecurity(query, query, currentUser);
            var gridMetadata = request.ProcessMetadata(new StudentExtraHourGrid());

            return await securedQuery.ExecuteGridQuery(gridMetadata, request.AllData);
        }
        public async Task<StudentExtraHours> CreateStudentExtraHours(StudentExtraHours data)
        {
            _db.StudentExtraHours.Add(data);

            var currentStudent = await _db.StudentHighestAbsenceCourseCount.Where(x => x.StudentUniqueId == data.StudentUniqueId).AsNoTracking().FirstOrDefaultAsync();
            
            if (currentStudent == null)
                throw new FormatException($"The Student Id: {data.StudentUniqueId} doesn't exist.");

            data.FirstName = currentStudent.FirstName;
            data.LastSurname = currentStudent.LastSurname;
            data.SchoolYear = currentStudent.SchoolYear;
            data.GradeLevel = currentStudent.GradeLevel;

            await _db.SaveChangesAsync();

            return data;
        }

        public async Task<StudentExtraHours> UpdateStudentExtraHours(StudentExtraHours data, IPrincipal currentUser)
        {
            var claims = ((ClaimsIdentity)currentUser.Identity).Claims;
            var oldRecord = await _db.StudentExtraHours.FirstOrDefaultAsync(x => x.Id == data.Id);
            
            if (oldRecord.UserCreatedUniqueId != data.UserCreatedUniqueId && Int32.Parse(claims.First(x => x.Type.Contains("level_id")).Value) >= 3)
                throw new UnauthorizedAccessException("You can only edit the records you created.");

            oldRecord.ReasonId = data.ReasonId;
            oldRecord.Hours = data.Hours;
            oldRecord.Comments = data.Comments;

            _db.StudentExtraHours.Update(oldRecord);
            await _db.SaveChangesAsync();

            return data;
        }

        public async Task<List<StudentExtraHours>> ImportStudentExtraHours(List<StudentExtraHours> studentExtraHours)
        {
            List<StudentExtraHours> conflicts = new List<StudentExtraHours>();

            // Removes any records with the required fields
            var requiredConflicts = studentExtraHours.Where(x => x.StudentUniqueId == null
            || x.Reason == null
            || x.Reason.Value == null
            || x.Comments == null
            || x.Date == DateTime.MinValue
            || x.Hours == 0
            || string.IsNullOrEmpty(x.StudentUniqueId));

            if(requiredConflicts.Count() > 0)
                conflicts.AddRange(requiredConflicts);

            studentExtraHours.RemoveAll(x => x.StudentUniqueId == null
            || x.Reason == null
            || x.Reason.Value == null
            || x.Comments == null
            || x.Date == DateTime.MinValue
            || x.Hours == 0
            || string.IsNullOrEmpty(x.StudentUniqueId));

            foreach (var record in studentExtraHours.ToList())
            {
                var reason = await _db.Reasons.Where(x => x.Value == record.Reason.Value.Trim() || x.ReasonId == record.ReasonId).AsNoTracking().FirstOrDefaultAsync();

                // The user can't upload something with dates in the future and the reason provided has to exist on the database
                if (reason == null || (record.Date - DateTime.Now.Date).Hours > 24)
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
            if(records.Count() > 0)
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
                    record.SchoolYear = currentStudent.SchoolYear;
                    record.GradeLevel = currentStudent.GradeLevel;
                    record.Comments = record.Comments.Substring(0, Math.Min(record.Comments.Length, 100));
                }
            }

            if(recordsToAdd.Count() > 0)
            {
                _db.StudentExtraHours.AddRange(recordsToAdd);
                await _db.SaveChangesAsync();
            }
               

            return conflicts; // This have to be returned for the user to know which records haven't been processed.
        }
    }
}
