using Microsoft.EntityFrameworkCore;
using SMCISD.Student360.Persistence.Auth;
using SMCISD.Student360.Persistence.EntityFramework;
using SMCISD.Student360.Persistence.Enum;
using SMCISD.Student360.Persistence.Grid;
using SMCISD.Student360.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace SMCISD.Student360.Persistence.Queries
{
    public interface IAttendanceLetterQueries : IGridData
    {
        Task<List<AttendanceLetters>> GetLettersByStudentId(string studentUniqueId);
        Task<List<AttendanceLetters>> GetGeneralDataForAttendanceLetters(DateTime date);

        Task<List<AttendanceLetterType>> GetAttendaceLetterTypes();

        Task<List<AttendanceLetterStatus>> GetAttendaceLetterStatus();
    }

    public class AttendanceLetterQueries : IAttendanceLetterQueries

    {
        private readonly Student360Context _db;
        private readonly IAuthenticationProvider _auth;

        public AttendanceLetterQueries(Student360Context db, IAuthenticationProvider auth)
        {
            _db = db;
            _auth = auth;
        }
        public async Task<List<AttendanceLetters>> GetLettersByStudentId(string studentUniqueId)
        {
            var firstDayOfSchool = await _db.FirstDayOfSchool.FirstOrDefaultAsync();
            return await _db.AttendanceLetters.Include(x => x.AttendanceLetterStatus).Include(x => x.AttendanceLetterType)
               .Where(x => x.AttendanceLetterStatusId != AttendanceLetterStatusEnum.AutoCancelled.Value
               && x.FirstAbsence.Date >= firstDayOfSchool.Date
               && x.StudentUniqueId == studentUniqueId).ToListAsync();
        }


        public async Task<List<AttendanceLetters>> GetGeneralDataForAttendanceLetters(DateTime date)
        {
            var firstDayOfSchool = await _db.FirstDayOfSchool.FirstOrDefaultAsync();
            date = date.AddDays(-2);
            return await _db.AttendanceLetters.Include(x => x.AttendanceLetterType)
                .Where(x => x.AttendanceLetterStatusId != AttendanceLetterStatusEnum.AutoCancelled.Value
                && x.FirstAbsence.Date >= firstDayOfSchool.Date
                && x.LastAbsence.Date <= date.Date).ToListAsync();
        }
        public async Task<List<AttendanceLetterType>> GetAttendaceLetterTypes()
        {
            return await _db.AttendanceLetterType.ToListAsync();
        }

        public async Task<List<AttendanceLetterStatus>> GetAttendaceLetterStatus()
        {
            return await _db.AttendanceLetterStatus.Where(x => x.AttendanceLetterStatusId != AttendanceLetterStatusEnum.AutoCancelled.Value
            && x.AttendanceLetterStatusId != AttendanceLetterStatusEnum.AdminOverride.Value).ToListAsync();
        }

        public async Task<GridResponse> GetGridData(GridRequest request, IPrincipal currentUser)
        {
            // Gets a list of all the last versions of the student extra hours.
            var query = _db.AttendanceLetterGrid;
            var securedQuery = _auth.ApplySecurity(query, query, currentUser);
            var gridMetadata = request.ProcessMetadata(new AttendanceLetterGrid());

            return await securedQuery.ExecuteGridQuery(gridMetadata, request.AllData);
        }
    }
}